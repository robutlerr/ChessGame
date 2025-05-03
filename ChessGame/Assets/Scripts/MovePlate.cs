using UnityEngine;

public class MovePlate : MonoBehaviour
{
   public GameObject GameManager;

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
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        Debug.Log("MovePlate actual runtime position: " + transform.position);
    }

    // clicking on the move plate
    public void OnMouseUp() {
        GameManager = GameObject.FindGameObjectWithTag("GameController");

        // if attacking, destroy the enemy piece
        if (attack) {
            // get the piece that is on that square
            GameObject cp = GameManager.GetComponent<Game>().GetPosition(matrixX, matrixY);

            // if the white king is getting destroyed, black wins!
            if (cp.name == "white_king") GameManager.GetComponent<Game>().Winner("black");

            // if the black king is getting destroyed, white wins!
            if (cp.name == "black_king") GameManager.GetComponent<Game>().Winner("white");

            // destroy it
            Destroy(cp);
        }
        // set the square that it occupied on the board as empty, since the piece is deleted
        GameManager.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<GameManager>().getXBoard(), 
                                                                reference.GetComponent<GameManager>().getYBoard());
        reference.GetComponent<GameManager>().setXBoard(matrixX);
        reference.GetComponent<GameManager>().setYBoard(matrixY);
        reference.GetComponent<GameManager>().SetCoordinates();

        // updating it in the game manager too
        GameManager.GetComponent<Game>().SetPosition(reference);

        // if the current player's move is complete, move to the next player's turn
        GameManager.GetComponent<Game>().NextTurn();

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
