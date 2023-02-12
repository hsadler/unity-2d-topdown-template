using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{


    // Manages telemetry display for development purposes.


    public int gameEntityCount = 0;

    private Rect guiSceneTelemetryRect = new Rect(10, 10, 800, 2000);


    // UNITY HOOKS

    void Awake() { }

    void Start() { }

    void Update() { }

    void OnGUI()
    {
        // show scene telemetry
        GUI.contentColor = Color.green;
        int fps = (int)(1.0f / Time.smoothDeltaTime);
        string displayText =
            "FPS: " + fps.ToString() +
            "\n" +
            "Game Entity Count: " + this.gameEntityCount.ToString();
        GUI.Label(
            this.guiSceneTelemetryRect,
            displayText
        );
    }

    // INTF METHODS

    // IMPL METHODS


}