using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class MovePlate : MonoBehaviour
{
   public GameObject GameManager;
   // preparing for the captured pieces to be displayed on the sides
   public GameObject capturedIconPrefab;
    public Transform whiteCapturedContainer;
    public Transform blackCapturedContainer;

    private static int whiteCapturedCount = 0;
    private static int blackCapturedCount = 0;


    // reference to the chess piece the move plate was created by
   GameObject reference = null;

    // board positions
    int matrixX;
    int matrixY;
    
    // false = movement | true = attacking
    public bool attack = false;

    public void Start() {
        if (attack) {
            // change to red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.717f, 0.686f, 0.89f, 1.0f);
        }

        Debug.Log("MovePlate actual runtime position: " + transform.position);
    }

    // clicking on the move plate (aka right after making a move with a piece)
    public void OnMouseUp() {
        GameManager = GameObject.FindGameObjectWithTag("GameController");

        // if attacking, destroy the enemy piece
        if (attack) {
            // get the piece that is on that square
            GameObject cp = GameManager.GetComponent<Game>().GetPosition(matrixX, matrixY);
            // get the sprite of the captured piece and display it on the side
            Sprite capturedSprite = cp.GetComponent<SpriteRenderer>().sprite;
            string capturedPlayer = cp.GetComponent<GameManager>().getPlayer();

            GameObject icon = Instantiate(capturedIconPrefab);
            icon.GetComponent<SpriteRenderer>().sprite = capturedSprite;

            float verticalSpacing = 0.6f;

            if (capturedPlayer == "white") {
                icon.transform.SetParent(blackCapturedContainer);
                icon.transform.localPosition = new Vector3(0, -blackCapturedCount * verticalSpacing, 0);
                blackCapturedCount++;
            } else {
                icon.transform.SetParent(whiteCapturedContainer);
                icon.transform.localPosition = new Vector3(0, -whiteCapturedCount * verticalSpacing, 0);
                whiteCapturedCount++;
            }
            // destroy the piece
            Destroy(cp);
        } 

        // get the starting x position (for later castling check)
        int startX = reference.GetComponent<GameManager>().getXBoard();
        int startY = reference.GetComponent<GameManager>().getYBoard();

        Debug.Log("Piece clicked: " + reference.name);

        // do castling logic before moving the piece
        bool isCastling = reference.name.Contains("king") && Mathf.Abs(matrixX - startX) == 2;

        // move the piece:

        // set the square that it occupied on the board as empty, since the piece is deleted
        GameManager.GetComponent<Game>().SetPositionEmpty(startX, startY);

        // update the piece's location
        reference.GetComponent<GameManager>().setXBoard(matrixX);
        reference.GetComponent<GameManager>().setYBoard(matrixY);
        reference.GetComponent<GameManager>().SetCoordinates();

        // castling check
         if (isCastling) {
        Debug.Log("CASTLING triggered — king moved from " + startX + " to " + matrixX);
        int rookStartX = (matrixX > startX) ? 7 : 0;
        int rookTargetX = (matrixX > startX) ? matrixX - 1 : matrixX + 1;

        GameObject rook = GameManager.GetComponent<Game>().GetPosition(rookStartX, startY);
        if (rook != null) {
            GameManager.GetComponent<Game>().SetPositionEmpty(rookStartX, startY);
            rook.GetComponent<GameManager>().setXBoard(rookTargetX);
            rook.GetComponent<GameManager>().SetCoordinates();
            GameManager.GetComponent<Game>().SetPosition(rook);
            Debug.Log("Rook moved from " + rookStartX + " to " + rookTargetX);
        }
    }

        // updating it in the game manager too
        GameManager.GetComponent<Game>().SetPosition(reference);

        // mark the piece as moved
        reference.GetComponent<GameManager>().MarkAsMoved();

        // check for any new conditions after the move:

        // checking for potential checks
        string enemyColor = (reference.GetComponent<GameManager>().getPlayer() == "white") ? "black" : "white";
        string player = reference.GetComponent<GameManager>().getPlayer();
        // if it returns true, the enemy king is in check (the current player's opponent)
        if (GameManager.GetComponent<Game>().IsKingInCheck(enemyColor)) {
            // once in check, checks for checkmate (no way to exit check)
            if (GameManager.GetComponent<Game>().IsKingInCheckmate(enemyColor)) {
                Debug.Log(enemyColor + " king is in CHECKMATE!");
                // debug messages
                Debug.Log("Checkmate confirmed for " + enemyColor);
                Debug.Log("Calling Winner with: " + player);

                GameManager.GetComponent<Game>().Winner(player);
            } else {
                // normal check!
                Debug.Log(enemyColor + " king is in check!");
            }
        }
        // checking for pawn promotion conditions
        reference.GetComponent<GameManager>().CheckPawnPromotion();

        // if the current player's move is complete, move to the next player's turn
        GameManager.GetComponent<Game>().NextTurn();
        // update the UI if the new player is in check bc of the previous player's move
        string currentTurn = GameManager.GetComponent<Game>().GetCurrentPlayer();
        GameManager.GetComponent<Game>().IsKingInCheck(currentTurn);
        reference.GetComponent<GameManager>().DestroyMovePlates();
    }

    public void SetCoordinates(int x, int y) {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj) {
        reference = obj;
    }

    public GameObject GetReference() {
        return reference;
    }
}
