using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITelemetryManager : MonoBehaviour
{


    // Manages telemetry display for development purposes.


    public int gameEntityCount = 0;
    private Rect guiRect = new(Screen.width - 810, 10, 800, 2000);

    // lowest fps calculation
    public float timeWindow = 10f;
    private float elapsedTime = 0f;
    private float lowestFPS = float.MaxValue;

    // refs
    private PlayerInputManager playerInputManager;


    // UNITY HOOKS

    void Awake() { }

    void Start()
    {
        this.playerInputManager = PlaySceneManager.instance.playerInputManager;
    }

    void Update()
    {
        this.CalculateLowestFPS();
    }

    void OnGUI()
    {
        if (GameSettings.DISPLAY_TELEMETRY)
        {
            int fps = (int)(1.0f / Time.smoothDeltaTime);
            int lowestFps = (int)this.lowestFPS;
            // hovered entity data
            GameObject gameEntityHovered = this.playerInputManager.GetHoveredEntity();
            string gameEntityHoveredName = gameEntityHovered == null ? "None" : gameEntityHovered.name;
            // drag containter entities data
            GameObject dragContainer = this.playerInputManager.GetEntityDragContainer();
            int dragContainterEntitiesCount = 0;
            for (int i = 0; i < dragContainer.transform.childCount; i++)
            {
                GameObject child = dragContainer.transform.GetChild(i).gameObject;
                if (child.TryGetComponent<GameEntity>(out GameEntity _))
                {
                    dragContainterEntitiesCount += 1;
                }
            }
            string displayText =
                "Game Name: " + SaveGameSignal.fileName +
                "\n" +
                "FPS: " + fps.ToString() +
                "\n" +
                "Lowest FPS within last " + this.timeWindow.ToString() + " seconds: " + lowestFps.ToString() +
                "\n" +
                "Input Mode: " + this.playerInputManager.inputMode.ToString() +
                "\n" +
                "Entity Total Count: " + this.gameEntityCount.ToString() +
                "\n" +
                "Entity Hovered: " + gameEntityHoveredName +
                "\n" +
                "Entities Selected Count: " + this.playerInputManager.GetEntitiesSelected().Count.ToString() +
                "\n" +
                "Entities MultiPlacement Count: " + this.playerInputManager.GetMulitPlacementEntities().Count.ToString() +
                "\n" +
                "Drag Container Entities Count: " + dragContainterEntitiesCount.ToString() +
                "\n" +
                "Camera Position: " + this.playerInputManager.GetCameraPosition() +
                "\n" +
                "Camera Zoom: " + this.playerInputManager.GetCameraZoom().ToString() +
                "\n" +
                "Mouse Position: " + Functions.QuantizeVector(Camera.main.ScreenToWorldPoint(Input.mousePosition)).ToString();
            GUIStyle style = new()
            {
                alignment = TextAnchor.UpperRight,
                normal = { textColor = Color.green },
                fontSize = GameSettings.GUI_FONT_SIZE
            };
            GUI.Label(
                this.guiRect,
                displayText,
                style
            );
        }
    }

    // INTF METHODS

    // IMPL METHODS

    private void CalculateLowestFPS()
    {
        // From ChatGPT
        // Update elapsed time and frame count
        elapsedTime += Time.deltaTime;
        // Calculate FPS
        float currentFPS = 1f / Time.deltaTime;
        // Update lowest FPS if needed
        if (currentFPS < lowestFPS)
        {
            lowestFPS = currentFPS;
        }
        // Check if the time window has passed
        if (elapsedTime >= timeWindow)
        {
            // Reset variables for the next time window
            elapsedTime = 0f;
            lowestFPS = float.MaxValue;
        }
    }


}