using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{


    // general settings
    public const bool ENTITY_POSITIONS_DISCRETE = true;

    // camera settings
    public const float CAMERA_SIZE_MIN = 4f;
    public const float CAMERA_SIZE_MAX = 40f;
    public const float CAMERA_ZOOM_AMOUNT_SMALL = 1f;
    public const float CAMERA_ZOOM_AMOUNT_NORMAL = 10f;
    public const float CAMERA_ZOOM_AMOUNT_LARGE = 100f;
    public const float CAMERA_ZOOM_SPEED = 8f;
    public const float CAMERA_MOVE_SPEED = 4f;

    // grid settings
    public const int GRID_SIZE = 50;

    // input modes
    public const int INPUT_MODE_INIT = 1;
    public const int INPUT_MODE_MENU = 2;

    // player input mappings
    public const KeyCode MENU_KEY = KeyCode.Escape;
    public const KeyCode SMALL_ZOOM_KEY = KeyCode.LeftControl;
    public const KeyCode LARGE_ZOOM_KEY = KeyCode.LeftShift;
    public const KeyCode ROTATE_ENTITIES_LEFT_KEY = KeyCode.Q;
    public const KeyCode ROTATE_ENTITIES_RIGHT_KEY = KeyCode.E;
    public const KeyCode DELETE_ENTITIES_KEY = KeyCode.Backspace;


}
