using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Globals;


public class GameManager : MonoBehaviour
{
    public GameMode gameMode;

    // Start is called before the first frame update
    void Start()
    {
        // Temp
        gameMode = GameMode.Development;
        switch (gameMode)
        {
            case GameMode.Singleplayer:
            case GameMode.Multiplayer:
            case GameMode.Development:
                // Spawn player
                break;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
