using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITelemetryManager : MonoBehaviour
{


    // Manages telemetry display for development purposes.


    public int gameEntityCount = 0;

    private Rect guiRect = new Rect(Screen.width - 810, 10, 800, 2000);


    // UNITY HOOKS

    void Awake() { }

    void Start() { }

    void Update() { }

    void OnGUI()
    {
        if (GameSettings.DISPLAY_TELEMETRY)
        {
            int fps = (int)(1.0f / Time.smoothDeltaTime);
            string displayText =
                "FPS: " + fps.ToString() +
                "\n" +
                "Game Entity Count: " + this.gameEntityCount.ToString();
            var style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            style.normal.textColor = Color.green;
            GUI.Label(
                this.guiRect,
                displayText,
                style
            );
        }
    }

    // INTF METHODS

    // IMPL METHODS


}