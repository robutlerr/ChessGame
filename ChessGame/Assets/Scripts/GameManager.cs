using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gm;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;

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

    public void SetCoordinates() {
        float x = xBoard * 0.66f - 2.3f;
        float y = yBoard * 0.66f - 2.3f;

        this.transform.position = new Vector3(x, y, -1.0f);
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
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
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
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
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

    public void PawnMovePlate(int x, int y) {
        Game sc = gm.GetComponent<Game>();
        if (sc.GetPosition(x,y) == null) {
            MovePlateSpawn(x,y);
        }

        if (sc.positionOnBoard(x + 1, y) && sc.GetPosition(x+1, y) != null && sc.GetPosition(x+1, y).GetComponent<GameManager>().player != player) {
            MovePlateAttackSpawn(x+1, y);
        }

        if (sc.positionOnBoard(x - 1, y) && sc.GetPosition(x-1, y) != null && sc.GetPosition(x-1, y).GetComponent<GameManager>().player != player) {
            MovePlateAttackSpawn(x-1, y);
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY) {
        float x = matrixX * 0.66f - 2.3f;
        float y = matrixY * 0.66f - 2.3f;

        Debug.Log("MovePlate board coords: (" + matrixX + ", " + matrixY + ")");
        Debug.Log("Calculated world position: (" + x + ", " + y + ")");

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        Debug.Log("Instantiated movePlate at: " + mp.transform.position);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoordinates(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY) {
        float x = matrixX * 0.66f - 2.3f;
        float y = matrixY * 0.66f - 2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x,y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoordinates(matrixX, matrixY);
    }

}
