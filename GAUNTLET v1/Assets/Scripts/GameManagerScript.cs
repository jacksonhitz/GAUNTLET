using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);

        // Unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void tryAgain()
    {
        // Ensure time scale is set to 1 before scene reload
        Time.timeScale = 1;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Unpause the game after the scene reloads
        // This is to ensure that in case Time.timeScale is not reset, the game is unpaused
        Time.timeScale = 1;
    }
}
