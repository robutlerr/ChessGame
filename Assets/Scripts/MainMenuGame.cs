using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class MainMenuGame : MonoBehaviour
{
    public void loadLevel(string levelToLoad)
	{
		SceneManager.LoadScene(levelToLoad);
	}
}
