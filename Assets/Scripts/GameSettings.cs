using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{


    // camera settings
    public const float CAMERA_SIZE_MIN = 4f;
    public const float CAMERA_SIZE_MAX = 40f;
    public const float CAMERA_ZOOM_AMOUNT_SMALL = 5f;
    public const float CAMERA_ZOOM_AMOUNT_NORMAL = 15f;
    public const float CAMERA_ZOOM_AMOUNT_LARGE = 150f;
    public const float CAMERA_ZOOM_SPEED = 5f;
    public const float CAMERA_MOVE_SPEED = 4f;

    // grid settings
    public const int GRID_SIZE = 50;

    // player input mappings

    public const KeyCode MENU_KEY = KeyCode.Escape;

    public const KeyCode SMALL_ZOOM_KEY = KeyCode.LeftControl;
    public const KeyCode LARGE_ZOOM_KEY = KeyCode.LeftShift;


}
