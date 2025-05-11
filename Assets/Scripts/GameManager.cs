using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gm;
    public GameObject movePlate;

    public GameObject capturedIconPrefab;
    public Transform whiteCapturedContainer;
    public Transform blackCapturedContainer;


    private int xBoard = -1;
    private int yBoard = -1;

    private string player;
    // variable to check whether a piece has made a move or not yet (for castling conditions)
    public bool hasMoved = false;

    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public void Activate() {

        gm = GameObject.FindGameObjectWithTag("GameController");

        // take the instantiate location and adjust the transform
        SetCoordinates();

        // set each object to its respective sprite
        switch(this.name) {
            case "black_queen": 
                this.GetComponent<SpriteRenderer>().sprite = black_queen;
                player = "black";
                break;
            case "black_knight": 
                this.GetComponent<SpriteRenderer>().sprite = black_knight;
                player = "black";
                break;
            case "black_bishop": 
                this.GetComponent<SpriteRenderer>().sprite = black_bishop; 
                player = "black";
                break;
            case "black_king": 
                this.GetComponent<SpriteRenderer>().sprite = black_king; 
                player = "black";
                break;
            case "black_rook": 
                this.GetComponent<SpriteRenderer>().sprite = black_rook; 
                player = "black";
                break;
            case "black_pawn": 
                this.GetComponent<SpriteRenderer>().sprite = black_pawn; 
                player = "black";
                break;
            case "white_queen": 
                this.GetComponent<SpriteRenderer>().sprite = white_queen;
                player = "white";
                break;
            case "white_knight":
                this.GetComponent<SpriteRenderer>().sprite = white_knight;
                player = "white";
                break;
            case "white_bishop": 
                this.GetComponent<SpriteRenderer>().sprite = white_bishop;
                player = "white";
                break;
            case "white_king": 
                this.GetComponent<SpriteRenderer>().sprite = white_king;
                player = "white";
                break;
            case "white_rook":
                this.GetComponent<SpriteRenderer>().sprite = white_rook;
                player = "white";
                break;
            case "white_pawn":
                this.GetComponent<SpriteRenderer>().sprite = white_pawn;
                player = "white";
                break;
        }

    }

    public void SetCoordinates()
    {
        this.transform.position = BoardMath.GetBoardWorldPosition(xBoard, yBoard, -1.0f);
    }

    public int getXBoard() {
        return xBoard;
    }

    public int getYBoard() {
        return yBoard;
    }

    public void setXBoard (int x) {
        xBoard = x;
    }

    public void setYBoard (int y) {
        yBoard = y;
    }

    public void setPlayer (string p) {
        player = p;
    }

    public string getPlayer() {
        return player;
    }

    public bool getHasMoved() {
        return hasMoved;
    }

    public void MarkAsMoved() {
        hasMoved = true;
    }

    private void OnMouseUp() {
        // if the game is still going and it's the current player's turn, let them move their pieces
        if (!gm.GetComponent<Game>().IsGameOver() && gm.GetComponent<Game>().GetCurrentPlayer() == player) {
            DestroyMovePlates();
            InitiateMovePlates();
        }

    }

    public void DestroyMovePlates() {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i =0; i < movePlates.Length; i++) {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlates() {
        Game sc = gm.GetComponent<Game>();

        string currentPlayer = sc.GetCurrentPlayer();

        // check variable
        bool inCheck = sc.IsKingInCheck(currentPlayer);

        List<Vector2Int> moves = GetAttackMoves(false);

        // if the current player is in check, require them to end the check before moving normally.
        if (inCheck) {
            List<Vector2Int> checkEscapeMoves = new List<Vector2Int>();

            foreach (Vector2Int move in moves) {

                // simulate move
                GameObject[,] originalBoard = (GameObject[,])sc.positions.Clone();
                GameObject target = sc.GetPosition(move.x, move.y);
                int originalX = xBoard;
                int originalY = yBoard;

                sc.SetPositionEmpty(xBoard, yBoard);
                sc.positions[move.x, move.y] = this.gameObject;
                xBoard = move.x;
                yBoard = move.y;

                bool safe = !sc.IsKingInCheck(currentPlayer);

                // undo the move
                sc.positions = originalBoard;
                xBoard = originalX;
                yBoard = originalY;

                if (safe) {
                    checkEscapeMoves.Add(move);
                }
            }

            foreach (Vector2Int move in checkEscapeMoves) {
                GameObject target = sc.GetPosition(move.x, move.y);
                if (target == null)
                    MovePlateSpawn(move.x, move.y);
                else
                    MovePlateAttackSpawn(move.x, move.y);
            }
            // returns so that the move ends and prevents normal movement
            return;
        }


        // normal move logic to setup movePlates for valid moves
        switch (this.name) {

            case "black_queen":
            case "white_queen":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(1,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                LineMovePlate(-1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(1,-1);
                break;

            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;

            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1,1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(-1,-1);
                break;

            case "black_king":
            case "white_king":
                SurroundMovePlate();

                Debug.Log("King has moved? " + getHasMoved());
                
                // if the king hasn't moved, check for castling conditions
                if (!getHasMoved() && player == "white") {
                    Debug.Log("KING WHITE: Checking for castling options...");
                    TryCastling(0, 0, 2);
                    TryCastling(7, 0, 6);
                }
                if (!getHasMoved() && player == "black") {
                    Debug.Log("KING BLACK: Checking for castling options...");
                    TryCastling(0, 7, 2);
                    TryCastling(7, 7, 6);
                }

                break;

            case "black_rook":
            case "white_rook":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                break;

            case "black_pawn":
                // move one square if the square in front is empty
                if (sc.GetPosition(xBoard, yBoard - 1) == null)
                {
                    PawnMovePlate(xBoard, yBoard - 1);

                    // if at the start and the second square in front is empty, move 2 square option is available
                    if (yBoard == 6 && sc.GetPosition(xBoard, yBoard - 2) == null)
                    {
                        PawnMovePlate(xBoard, yBoard - 2);
                    }
                }

                // check the left diagonal attack (independent of moving forward)
                if (xBoard - 1 >= 0 && yBoard - 1 >= 0)
                {
                    GameObject leftTarget = sc.GetPosition(xBoard - 1, yBoard - 1);
                    if (leftTarget != null && leftTarget.GetComponent<GameManager>().player == "white")
                    {
                        MovePlateAttackSpawn(xBoard - 1, yBoard - 1);
                    }
                }

                // check the right diagonal attack (independent of moving forward)
                if (xBoard + 1 <= 7 && yBoard - 1 >= 0)
                {
                    GameObject rightTarget = sc.GetPosition(xBoard + 1, yBoard - 1);
                    if (rightTarget != null && rightTarget.GetComponent<GameManager>().player == "white")
                    {
                        MovePlateAttackSpawn(xBoard + 1, yBoard - 1);
                    }
                }
                break;

            case "white_pawn":
                // move one square if the square in front is empty
                if (sc.GetPosition(xBoard, yBoard + 1) == null)
                {
                    PawnMovePlate(xBoard, yBoard + 1);

                    // if at the start and the second square in front is empty, move 2 square option
                    if (yBoard == 1 && sc.GetPosition(xBoard, yBoard + 2) == null)
                    {
                        PawnMovePlate(xBoard, yBoard + 2);
                    }
                }

                // check the left diagonal attack (independent of moving forward)
                if (xBoard - 1 >= 0 && yBoard + 1 <= 7)
                {
                    GameObject leftTarget = sc.GetPosition(xBoard - 1, yBoard + 1);
                    if (leftTarget != null && leftTarget.GetComponent<GameManager>().player == "black")
                    {
                        MovePlateAttackSpawn(xBoard - 1, yBoard + 1);
                    }
                }

                // check the right diagonal attack (independent of moving forward)
                if (xBoard + 1 <= 7 && yBoard + 1 <= 7)
                {
                    GameObject rightTarget = sc.GetPosition(xBoard + 1, yBoard + 1);
                    if (rightTarget != null && rightTarget.GetComponent<GameManager>().player == "black")
                    {
                        MovePlateAttackSpawn(xBoard + 1, yBoard + 1);
                    }
                }
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement) {
        Game sc = gm.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.positionOnBoard(x,y) && sc.GetPosition(x,y) == null) {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        } 

        if (sc.positionOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<GameManager>().player != player) {
            MovePlateAttackSpawn(x,y);
        }
    }

    public void LMovePlate() {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate() {
        SafePointMovePlate(xBoard, yBoard + 1);
        SafePointMovePlate(xBoard, yBoard - 1);
        SafePointMovePlate(xBoard - 1, yBoard - 1);
        SafePointMovePlate(xBoard - 1, yBoard + 0);
        SafePointMovePlate(xBoard - 1, yBoard + 1);
        SafePointMovePlate(xBoard + 1, yBoard - 1);
        SafePointMovePlate(xBoard + 1, yBoard + 0);
        SafePointMovePlate(xBoard + 1, yBoard + 1);
    }

    private void TryCastling(int rookX, int rookY, int kingTargetX) {
        Debug.Log($"TRYING CASTLING from king at ({xBoard}, {yBoard}) to {kingTargetX}, rook at ({rookX}, {rookY})");
        Game sc = gm.GetComponent<Game>();
        GameObject rook = sc.GetPosition(rookX, rookY);

        if (rook != null) {
            GameManager rookScript = rook.GetComponent<GameManager>();

            if (rookScript != null && !rookScript.getHasMoved()) {
                // check that the squares between the king and rook are empty
                int direction = (rookX < xBoard) ? -1 : 1;
                bool clear = true;

                for (int x = xBoard + direction; x != rookX; x += direction) {
                    if (sc.GetPosition(x, yBoard) != null) {
                        clear = false;
                        break;
                    }
                }
                // spawn a move plate showing that castling is an option if the conditions meet
                if (clear) {
                    Debug.Log("Castling move plate should now be spawned at: " + kingTargetX + ", " + yBoard);
                    MovePlateSpawn(kingTargetX, yBoard);
                }
            }
        }
    }
    

    public void PointMovePlate(int x, int y) {
        Game sc = gm.GetComponent<Game>();
        if (sc.positionOnBoard(x,y)) {
            GameObject cp = sc.GetPosition(x,y);

            if (cp == null) {
                MovePlateSpawn(x,y);
            } else if (cp.GetComponent<GameManager>().player != player) {
                MovePlateAttackSpawn(x,y);
            }
        }
    }

    // valid moves to check if a king is currently in check (king only version of PointMovePlate function)
    // will not let the king move into a check position
    public void SafePointMovePlate(int x, int y) {
        Game game = gm.GetComponent<Game>();
        if (!game.positionOnBoard(x, y)) return;

        GameObject target = game.GetPosition(x, y);
        bool isEnemyOrEmpty = target == null || target.GetComponent<GameManager>().player != player;
        if (!isEnemyOrEmpty) return;

        // Simulate the move
        GameObject[,] originalBoard = (GameObject[,])game.positions.Clone();
        GameObject captured = game.GetPosition(x, y);

        int originalX = xBoard;
        int originalY = yBoard;

        game.SetPositionEmpty(xBoard, yBoard);
        game.positions[x, y] = this.gameObject;
        xBoard = x;
        yBoard = y;

        bool safe = !game.IsKingInCheck(player);

        // Undo the move
        game.positions = originalBoard;
        xBoard = originalX;
        yBoard = originalY;

        if (safe) {
            if (captured == null)
                MovePlateSpawn(x, y);
            else
                MovePlateAttackSpawn(x, y);
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = gm.GetComponent<Game>();

        // Only allow forward movement here.
        if (sc.positionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
        }

    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        Debug.Log("MovePlate board coords: (" + matrixX + ", " + matrixY + ")");
        Vector3 position = BoardMath.GetBoardWorldPosition(matrixX, matrixY, -3.0f);
        GameObject mp = Instantiate(movePlate, position, Quaternion.identity);

        Debug.Log("Instantiated movePlate at: " + mp.transform.position);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoordinates(matrixX, matrixY);

        var global = GlobalGameManager.Instance;
        // get the prefabs and containers for displaying the captured pieces on the side

        mpScript.capturedIconPrefab = global.capturedIconPrefab;
        mpScript.whiteCapturedContainer = global.whiteCapturedContainer;
        mpScript.blackCapturedContainer = global.blackCapturedContainer;

    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        Vector3 position = BoardMath.GetBoardWorldPosition(matrixX, matrixY, -3.0f);
        GameObject mp = Instantiate(movePlate, position, Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoordinates(matrixX, matrixY);

        var global = GlobalGameManager.Instance;
        // get the prefabs and containers for displaying the captured pieces on the side

        mpScript.capturedIconPrefab = global.capturedIconPrefab;
        mpScript.whiteCapturedContainer = global.whiteCapturedContainer;
        mpScript.blackCapturedContainer = global.blackCapturedContainer;
    }

    public static class BoardMath
    {
        public static float tileSize = 0.642f;

        public static Vector3 GetBoardWorldPosition(int x, int y, float z = -1f)
        {
            float offset = tileSize * 3.5f;
            return new Vector3(x * tileSize - offset, y * tileSize - offset, z);
        }
    }

    // logic for enforcing valid moves during checks (making it so each piece can check the king)
    public List<Vector2Int> GetAttackMoves(bool skipSelfCheck = false) {
        // create a list for the moves that are allowed while in check
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // calculate the possible moves for a queen
        if (this.name.Contains("queen")) {
            // diagonal check
            ValidLineMoves(validMoves, 1, 1, skipSelfCheck);
            ValidLineMoves(validMoves, 1, -1, skipSelfCheck);
            ValidLineMoves(validMoves, -1, 1, skipSelfCheck);
            ValidLineMoves(validMoves, -1, -1, skipSelfCheck);

            // vertical and horizontal check
            ValidLineMoves(validMoves, 1, 0, skipSelfCheck);  // right
            ValidLineMoves(validMoves, -1, 0, skipSelfCheck); // left
            ValidLineMoves(validMoves, 0, 1, skipSelfCheck);  // up
            ValidLineMoves(validMoves, 0, -1, skipSelfCheck); // down
        }

        // calculate the possible moves for a knight
        if (this.name.Contains("knight")) {
                int[,] knightMoves = new int[,] {
                { 1, 2 }, { 2, 1 }, { 2, -1 }, { 1, -2 },
                { -1, -2 }, { -2, -1 }, { -2, 1 }, { -1, 2 }
            };

            Game sc = gm.GetComponent<Game>();

            for (int i = 0; i < knightMoves.GetLength(0); i++) {
                int dx = knightMoves[i, 0];
                int dy = knightMoves[i, 1];
                int newX = xBoard + dx;
                int newY = yBoard + dy;

                if (!sc.positionOnBoard(newX, newY)) continue;

                GameObject target = sc.GetPosition(newX, newY);
                bool isEnemyOrEmpty = target == null || target.GetComponent<GameManager>().player != player;

                if (!isEnemyOrEmpty) continue;

                if (skipSelfCheck) {
                    validMoves.Add(new Vector2Int(newX, newY));
                } else {
                    // simulate move
                    GameObject[,] originalBoard = (GameObject[,])sc.positions.Clone();

                    int originalX = xBoard;
                    int originalY = yBoard;

                    sc.SetPositionEmpty(originalX, originalY);
                    sc.positions[newX, newY] = this.gameObject;
                    xBoard = newX;
                    yBoard = newY;

                    bool legal = !sc.IsKingInCheck(player);

                    // undo
                    sc.positions = originalBoard;
                    xBoard = originalX;
                    yBoard = originalY;

                    if (legal) {
                        validMoves.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        // calculate the possible moves for a bishop (lines diagonal)
        if (this.name.Contains("bishop")) {
            ValidLineMoves(validMoves, 1, 1, skipSelfCheck);
            ValidLineMoves(validMoves, 1, -1, skipSelfCheck);
            ValidLineMoves(validMoves, -1, 1, skipSelfCheck);
            ValidLineMoves(validMoves, -1, -1, skipSelfCheck);
        }
        // calculate the possible king moves
        if (this.name.Contains("king")) {
            ValidPointMove(validMoves, xBoard + 1, yBoard, skipSelfCheck);
            ValidPointMove(validMoves, xBoard - 1, yBoard, skipSelfCheck);
            ValidPointMove(validMoves, xBoard, yBoard + 1, skipSelfCheck);
            ValidPointMove(validMoves, xBoard, yBoard - 1, skipSelfCheck);
            ValidPointMove(validMoves, xBoard + 1, yBoard + 1, skipSelfCheck);
            ValidPointMove(validMoves, xBoard + 1, yBoard - 1, skipSelfCheck);
            ValidPointMove(validMoves, xBoard - 1, yBoard + 1, skipSelfCheck);
            ValidPointMove(validMoves, xBoard - 1, yBoard - 1, skipSelfCheck);
        }

        // calculate the possible moves for a rook piece (moves in lines)
        if (this.name.Contains("rook")) {
            ValidLineMoves(validMoves, 1, 0, skipSelfCheck);  // right
            ValidLineMoves(validMoves, -1, 0, skipSelfCheck); // left
            ValidLineMoves(validMoves, 0, 1, skipSelfCheck);  // up
            ValidLineMoves(validMoves, 0, -1, skipSelfCheck); // down
        }

        // calculate possible moves for a pawn to check
        if (this.name.Contains("pawn")) {
            int direction = (player == "white") ? 1 : -1;

            Game sc = gm.GetComponent<Game>();

            Vector2Int[] diagonals = new Vector2Int[] {
                new Vector2Int(xBoard - 1, yBoard + direction),
                new Vector2Int(xBoard + 1, yBoard + direction)
            };

            foreach (var move in diagonals)
            {
                int x = move.x;
                int y = move.y;
                if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
                {
                    GameObject target = sc.GetPosition(x, y);
                    if (target != null && target.GetComponent<GameManager>().player != player)
                    {
                        if (skipSelfCheck)
                        {
                            Debug.Log(this.name + " can legally capture on " + x + "," + y);
                            validMoves.Add(new Vector2Int(x, y));
                        }
                        else
                        {
                            // Simulate the move
                            GameObject[,] originalBoard = (GameObject[,])sc.positions.Clone();
                            //GameObject captured = sc.GetPosition(x, y);
                            int originalX = xBoard;
                            int originalY = yBoard;

                            sc.SetPositionEmpty(originalX, originalY);
                            sc.positions[x, y] = this.gameObject;
                            xBoard = x;
                            yBoard = y;

                            bool legal = !sc.IsKingInCheck(player);

                            // Undo move
                            sc.positions = originalBoard;
                            xBoard = originalX;
                            yBoard = originalY;

                            if (legal)
                                validMoves.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }

        // returns the full list of allowed moves for the pieces
        return validMoves;
    }


    // valid moves in check for pieces that move in a line
    private void ValidLineMoves(List<Vector2Int> list, int xDirection, int yDirection, bool skipSelfCheck)
    {
        Game game = gm.GetComponent<Game>();
        int x = xBoard + xDirection;
        int y = yBoard + yDirection;

        while (game.positionOnBoard(x, y)) {
            GameObject target = game.GetPosition(x, y);
            Debug.Log($"{this.name} at ({xBoard},{yBoard}) scanning ({x},{y})");

            if (target == null) {
                // Empty square
                if (skipSelfCheck) {
                    list.Add(new Vector2Int(x, y));
                } else {
                    // Simulate move
                    GameObject[,] originalBoard = (GameObject[,])game.positions.Clone();

                    game.SetPositionEmpty(xBoard, yBoard);
                    game.positions[x, y] = this.gameObject;

                    int originalX = xBoard;
                    int originalY = yBoard;
                    xBoard = x;
                    yBoard = y;

                    bool isLegal = !game.IsKingInCheck(player);

                    game.positions = originalBoard;
                    xBoard = originalX;
                    yBoard = originalY;

                    if (isLegal)
                    {
                        list.Add(new Vector2Int(x, y));
                    }
                }
            } else {
                string targetPlayer = target.GetComponent<GameManager>().player;

                if (targetPlayer != player) {
                    // Enemy piece — can capture, then stop
                    if (skipSelfCheck) {
                        list.Add(new Vector2Int(x, y));
                    } else {
                        // Simulate move
                        GameObject[,] originalBoard = (GameObject[,])game.positions.Clone();

                        game.SetPositionEmpty(xBoard, yBoard);
                        game.positions[x, y] = this.gameObject;

                        int originalX = xBoard;
                        int originalY = yBoard;
                        xBoard = x;
                        yBoard = y;

                        bool isLegal = !game.IsKingInCheck(player);

                        game.positions = originalBoard;
                        xBoard = originalX;
                        yBoard = originalY;

                        if (isLegal)
                        {
                            list.Add(new Vector2Int(x, y));
                        }
                    }

                    Debug.Log($"{this.name} sees {target.name} at ({x},{y}) — stopping after capture");
                    break;
                } else {
                    // Friendly piece — block
                    Debug.Log($"{this.name} blocked by friendly {target.name} at ({x},{y})");
                    break;
                }
            }

            x += xDirection;
            y += yDirection;
        }
    }

    
    // valid moves in check for pieces that move one square
    private void ValidPointMove(List<Vector2Int> list, int x, int y, bool skipSelfCheck)
    {
        Game game = gm.GetComponent<Game>();

        if (!game.positionOnBoard(x, y)) return;

        GameObject target = game.GetPosition(x, y);
        bool isEnemyOrEmpty = (target == null || target.GetComponent<GameManager>().player != player);
        if (!isEnemyOrEmpty) return;

        if (skipSelfCheck)
        {
            list.Add(new Vector2Int(x, y));
            return;
        }

        // Simulate the move
        GameObject[,] originalBoard = (GameObject[,])game.positions.Clone();
        GameObject captured = game.GetPosition(x, y);

        int originalX = xBoard;
        int originalY = yBoard;

        game.SetPositionEmpty(xBoard, yBoard);
        game.positions[x, y] = this.gameObject;
        xBoard = x;
        yBoard = y;

        bool legal = !game.IsKingInCheck(player);

        // Undo
        game.positions = originalBoard;
        xBoard = originalX;
        yBoard = originalY;

        if (legal)
        {
            list.Add(new Vector2Int(x, y));
        }
    }

    public void CheckPawnPromotion() {
        // if the pawn is white and on the last square of the black side, promote the white pawn
        if (this.name == "white_pawn" && yBoard == 7) {
            PromoteToQueen("white");
        // same but w/ the black pawn
        } else if (this.name == "black_pawn" && yBoard == 0) {
            PromoteToQueen("black");
        }
    }

    public void PromoteToQueen(string pieceColor) {
        // changing the pawn name to queen instead
        this.name = pieceColor + "_queen";
        // changes the sprite to the white queen
        if (pieceColor == "white") {
            this.GetComponent<SpriteRenderer>().sprite = white_queen;
        // changes the sprite to the black queen
        } else if (pieceColor == "black") {
            this.GetComponent<SpriteRenderer>().sprite = black_queen;
        }
    }
}
