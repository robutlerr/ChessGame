using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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

    // variables for the end game text
    public TMP_Text WinnerText;
    public TMP_Text RestartText;

    
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
        if (currentPlayer == "white") {
            currentPlayer = "black";
        } else {
            currentPlayer = "white";
        }
    }

    public void Winner(string playerWinner) {
        gameOver = true;
        WinnerText.gameObject.SetActive(true);
        WinnerText.text = playerWinner.ToUpper() + " Is\nThe Winner!";
        RestartText.gameObject.SetActive(true);
    }

}
