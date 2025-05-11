using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    // spaces on the board
    public GameObject[,] positions = new GameObject[8, 8];
    // black's pieces
    public GameObject[] playerBlack = new GameObject[16];
    // white's pieces
    public GameObject[] playerWhite = new GameObject[16];

    // variable to keep track of which player is moving (true = white, false = black)
    // can turn into an int variable if needed
    private string currentPlayer = "white";
    private bool gameOver = false;
    
    public TMP_Text CheckText;
    // variables for the end game text
    public TMP_Text WinnerText;
    public TMP_Text RestartText;
    public TMP_Text WhitePlayerText;
    public TMP_Text BlackPlayerText;    


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Entered the start function");
        setupGame();
    }

    // Update is called once per frame
    void Update()
    {
        // start new game if it's over and the user clicks
        if (gameOver == true && Input.GetMouseButtonDown(0)) {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }

    // check if the piece is a valid move, if so the square will light up when hovered
    // if player chooses square that is an invalid move, it will beep NO

    void setupGame() {
        Debug.Log("Starting setup game");
        // sets game as active
        gameOver = false;
        // sets the first player to White
        currentPlayer = "white";
        BlackPlayerText.gameObject.SetActive(false);
        // show the current player as white
        WhitePlayerText.gameObject.SetActive(true);
        // array of white's pieces
        playerWhite = new GameObject[] {
            Create("white_rook", 0, 0),
            Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0),
            Create("white_queen", 3, 0),
            Create("white_king", 4, 0),
            Create("white_bishop", 5, 0),
            Create("white_knight", 6, 0),
            Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1),
            Create("white_pawn", 1, 1),
            Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1),
            Create("white_pawn", 4, 1),
            Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1),
            Create("white_pawn", 7, 1),
        };
        // array of black's pieces
        playerBlack = new GameObject[] {
            Create("black_rook", 0, 7),
            Create("black_knight", 1, 7),
            Create("black_bishop", 2, 7),
            Create("black_queen", 3, 7),
            Create("black_king", 4, 7),
            Create("black_bishop", 5, 7),
            Create("black_knight", 6, 7),
            Create("black_rook", 7, 7),
            Create("black_pawn", 0, 6),
            Create("black_pawn", 1, 6),
            Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6),
            Create("black_pawn", 4, 6),
            Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6),
            Create("black_pawn", 7, 6),
        };
        // setting up all the piece positions on the board
        for (int i=0; i<playerBlack.Length; i++) {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y) {
        GameObject piece = Instantiate(chesspiece, new Vector3(0,0,-1), Quaternion.identity);
        GameManager cc = piece.GetComponent<GameManager>();
        cc.name = name;
        cc.setXBoard(x);
        cc.setYBoard(y);
        cc.SetCoordinates();
        cc.Activate();
        return piece;
    }

    public void SetPosition(GameObject piece) {
        GameManager cc = piece.GetComponent<GameManager>();
        positions[cc.getXBoard(), cc.getYBoard()] = piece;
    }

    public void SetPositionEmpty(int x, int y) {
        positions[x,y] = null;
    }

    public GameObject GetPosition(int x, int y) {
        return positions[x, y];
    }

    public bool positionOnBoard(int x, int y) {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(0)) {
            return false;
        } else {
            return true;
        }
    }

    // switch between players
    public string GetCurrentPlayer() {
        return currentPlayer;
    }

    public bool IsGameOver() {
        return gameOver;
    }

    public void NextTurn() {
        // only process the next turn if the game is still actively going
        if (gameOver == false) {
            if (currentPlayer == "white") {
                currentPlayer = "black";

                WhitePlayerText.gameObject.SetActive(false);
                // show the current player as black
                BlackPlayerText.gameObject.SetActive(true);
            } else {
                currentPlayer = "white";

                BlackPlayerText.gameObject.SetActive(false);
                // show the current player as white
                WhitePlayerText.gameObject.SetActive(true);
            }

            if (IsKingInCheck(currentPlayer)) {
                // activating the text to show the check
                CheckText.text = $"{currentPlayer.ToUpper()} KING IS IN CHECK!";
                CheckText.gameObject.SetActive(true);
                Debug.Log(currentPlayer + " king is in CHECK!");
            }
        }
    }

    public GameObject FindKing(string color) {
        GameObject[] pieces = (color == "white") ? playerWhite : playerBlack;

        // find the king in the array
        foreach (GameObject piece in pieces)
        {
            if (piece != null && piece.name == color + "_king")
            {
                // if found, return the GameObject
                return piece;
            }
        }

        return null;

    }

    public bool IsKingInCheck(string playerColor) {
        string enemyColor = "";
        // setting the enemy color based on the player
        if (playerColor == "white") {
            enemyColor = "black";
        } else if (playerColor == "black") {
            enemyColor = "white";
        }

        Debug.Log("Checking if enemy king is in check: " + enemyColor);

        // find the position of the current playerKing
        GameObject king = FindKing(playerColor);
        if (king == null) {
            Debug.Log("No king (NULL)");
            return false;
        }

        Vector2Int kingPosition = new Vector2Int (king.GetComponent<GameManager>().getXBoard(),
                                            king.GetComponent<GameManager>().getYBoard());



        GameObject[] enemyPieces = (enemyColor == "white") ? playerWhite : playerBlack;

        foreach (GameObject piece in enemyPieces)
        {
            if (piece == null) continue;

            GameManager gm = piece.GetComponent<GameManager>();
            List<Vector2Int> possibleMoves = gm.GetAttackMoves(true);

            // debugging
            foreach (Vector2Int m in possibleMoves)
            {
                if (m == kingPosition)
                {
                    Debug.Log(piece.name + " CAN attack king at: " + kingPosition);
                    return true;
                }
                else
                {
                    Debug.Log(piece.name + " CANNOT attack king. King is at: " + kingPosition);
                }
                Debug.Log(piece.name + " threatens: " + m);
            }

            if (possibleMoves.Contains(kingPosition))
            {
                // if the enemy piece can attack the king, or attack the king if the king moves into that square, the king is in check
                return true;
            }
        }
        // deactivate the text if it is active
        CheckText.gameObject.SetActive(false);
        // the king isn't in check
        return false;

    }

    public bool IsKingInCheckmate(string playerColor) {
        Debug.Log("Checking legal moves for: " + playerColor);
        GameObject[] playerPieces = (playerColor == "white") ? playerWhite : playerBlack;

        foreach (GameObject piece in playerPieces)
        {
            if (piece == null) continue;

            GameManager gm = piece.GetComponent<GameManager>();
            List<Vector2Int> possibleMoves = gm.GetAttackMoves(false);

            foreach (Vector2Int move in possibleMoves)
            {
                // save the original/real piece location on board
                int originalX = gm.getXBoard();
                int originalY = gm.getYBoard();
                GameObject[,] originalBoard = (GameObject[,])positions.Clone();
                GameObject captured = GetPosition(move.x, move.y);

                // simulate the move
                SetPositionEmpty(originalX, originalY);
                positions[move.x, move.y] = piece;
                gm.setXBoard(move.x);
                gm.setYBoard(move.y);

                bool stillInCheck = IsKingInCheck(playerColor);

                // undo the simulated test move
                positions = originalBoard;
                gm.setXBoard(originalX);
                gm.setYBoard(originalY);

                if (!stillInCheck) {
                    Debug.Log(piece.name + " can prevent checkmate by moving to " + move);
                    return false; // found a legal move or legal escape 
                }
                
            }
        }

        // no escape moves from check found
        return true;

    }

    public void Winner(string playerWinner) {
        // end the game
        gameOver = true;

        // hide the in-game text
        WhitePlayerText.gameObject.SetActive(false);
        BlackPlayerText.gameObject.SetActive(false);
        CheckText.gameObject.SetActive(false);


        // unhide all of the game over text
        WinnerText.gameObject.SetActive(true);
        RestartText.gameObject.SetActive(true);

        // show the winner
        WinnerText.text = $"<color=#dd61ff>{playerWinner.ToUpper()}</color> Is\nThe Winner!";
    }

}
