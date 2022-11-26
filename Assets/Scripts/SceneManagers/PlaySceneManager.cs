using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{


    // MonoBehaviour manager components
    public ProceduralEnvironmentManager proceduralEnvironmentManager;
    public PlayerInputManager playerInputManager;

    public int inputMode;


    // the static reference to the singleton instance
    public static PlaySceneManager instance;


    // UNITY HOOKS

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        this.proceduralEnvironmentManager.GenerateGrid();
        this.inputMode = GameSettings.INPUT_MODE_INIT;
    }

    void Update() { }

    // INTF METHODS

    public void RestartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // IMPL METHODS


}
