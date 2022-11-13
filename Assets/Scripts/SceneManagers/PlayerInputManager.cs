using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{


    public GameObject MenuGO;
    private float cameraSize;


    // UNITY HOOKS

    void Start()
    {
        this.MenuGO.SetActive(false);
        this.cameraSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        // player input
        this.CheckPlayerInput();
        // camera
        if (PlaySceneManager.instance.inputMode != GameSettings.INPUT_MODE_MENU)
        {
            this.HandleCameraMovement();
            this.HandleCameraZoom();
        }
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void CheckPlayerInput()
    {
        if (Input.GetKeyDown(GameSettings.MENU_KEY))
        {
            this.MenuGO.SetActive(!this.MenuGO.activeSelf);
            PlaySceneManager.instance.inputMode = this.MenuGO.activeSelf ? GameSettings.INPUT_MODE_MENU : GameSettings.INPUT_MODE_INIT;
        }
    }

    // camera controls
    private void HandleCameraMovement()
    {
        // right click held
        if (Input.GetMouseButton(1))
        {
            // scale camera move amount with size of camera view
            float vert = Input.GetAxis("Mouse Y") * Time.deltaTime * Camera.main.orthographicSize * GameSettings.CAMERA_MOVE_SPEED;
            float horiz = Input.GetAxis("Mouse X") * Time.deltaTime * Camera.main.orthographicSize * GameSettings.CAMERA_MOVE_SPEED;
            Camera.main.transform.Translate(new Vector3(-horiz, -vert, 0));
        }
    }

    private void HandleCameraZoom()
    {
        float zoomMultiplier = GameSettings.CAMERA_ZOOM_AMOUNT_NORMAL;
        if (Input.GetKey(GameSettings.SMALL_ZOOM_KEY))
        {
            zoomMultiplier = GameSettings.CAMERA_ZOOM_AMOUNT_SMALL;
        }
        else if (Input.GetKey(GameSettings.LARGE_ZOOM_KEY))
        {
            zoomMultiplier = GameSettings.CAMERA_ZOOM_AMOUNT_LARGE;
        }
        float currCameraSize = Camera.main.orthographicSize;
        if (Input.mouseScrollDelta.y != 0)
        {
            this.cameraSize = currCameraSize - (Input.mouseScrollDelta.y * zoomMultiplier);
            // clamp
            if (this.cameraSize < GameSettings.CAMERA_SIZE_MIN)
            {
                this.cameraSize = GameSettings.CAMERA_SIZE_MIN;
            }
            else if (this.cameraSize > GameSettings.CAMERA_SIZE_MAX)
            {
                this.cameraSize = GameSettings.CAMERA_SIZE_MAX;
            }
        }
        Camera.main.orthographicSize = Mathf.Lerp(currCameraSize, this.cameraSize, Time.deltaTime * GameSettings.CAMERA_ZOOM_SPEED);
    }


}
