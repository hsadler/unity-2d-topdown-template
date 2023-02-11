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

    // telemetry UI
    private Rect guiSceneTelemetryRect = new Rect(10, 10, 800, 2000);

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

    void OnGUI()
    {
        // show scene telemetry
        GUI.contentColor = Color.green;
        int fps = (int)(1.0f / Time.smoothDeltaTime);
        string displayText =
            "FPS: " + fps.ToString();
        // "\nmessage #2: hi there";
        GUI.Label(
            this.guiSceneTelemetryRect,
            displayText
        );
    }

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
