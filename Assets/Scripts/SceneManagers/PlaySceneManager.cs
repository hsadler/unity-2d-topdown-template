using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{

    // The main gameplay singleton manager. Holds references to sub-managers.


    // MonoBehaviour manager components
    public ProceduralEnvironmentManager proceduralEnvironmentManager;
    public GameEntityManager gameEntityManager;
    public PlayerInputManager playerInputManager;
    public PlayerInventoryManager playerInventoryManager;
    public TelemetryManager telemetryManager;

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
