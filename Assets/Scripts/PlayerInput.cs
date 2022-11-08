using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{


    private float cameraSize;


    // UNITY HOOKS

    void Start()
    {
        this.cameraSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        // camera
        this.HandleCameraMovement();
        this.HandleCameraZoom();
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

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
        if (Input.GetKey(ConstPlayerInput.SMALL_ZOOM_KEY))
        {
            zoomMultiplier = GameSettings.CAMERA_ZOOM_AMOUNT_SMALL;
        }
        else if (Input.GetKey(ConstPlayerInput.LARGE_ZOOM_KEY))
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
