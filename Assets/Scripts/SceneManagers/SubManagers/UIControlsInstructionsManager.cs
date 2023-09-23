using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlsInstructionsManager : MonoBehaviour
{


    // Manages UI display of game control instructions.


    private Rect guiRect = new Rect(10, 10, 800, 2000);


    // UNITY HOOKS

    void Awake() { }

    void Start() { }

    void Update() { }

    void OnGUI()
    {
        int fps = (int)(1.0f / Time.smoothDeltaTime);
        string displayText =
            "Select: LMB" +
            "\n" +
            "Rotate: 'Q' / 'E'" +
            "\n" +
            "Delete: Backspace" +
            "\n" +
            "Undo/Redo: '<' / '>'" +
            "\n" +
            "Map move: RMB + drag" +
            "\n" +
            "Map zoom: Mouse Wheel" +
            "\n" +
            "Menu: Esc";
        var style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = Color.green;
        style.fontSize = GameSettings.GUI_FONT_SIZE;
        GUI.Label(
            this.guiRect,
            displayText,
            style
        );
    }

    // INTF METHODS

    // IMPL METHODS


}