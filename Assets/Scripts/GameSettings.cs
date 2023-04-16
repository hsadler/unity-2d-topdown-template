using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{


    // general settings
    public const int GRID_SIZE = 200;
    public const bool ENTITY_POSITIONS_DISCRETE = true;
    public const int ENTITY_STATE_MAX_HISTORY = 100;


    // camera settings
    public const float CAMERA_SIZE_MIN = 4f;
    public const float CAMERA_SIZE_MAX = 40f;
    public const float CAMERA_ZOOM_AMOUNT = 10f;
    public const float CAMERA_ZOOM_SPEED = 8f;
    public const float CAMERA_MOVE_SPEED = 4f;


    // input modes
    public const int INPUT_MODE_DEFAULT = 1;
    public const int INPUT_MODE_INVENTORY_MULTIPLACEMENT = 2;
    public const int INPUT_MODE_MENU = 3;


    // player input mappings
    public const KeyCode ESC_KEY = KeyCode.Escape;
    public const KeyCode SMALL_ZOOM_KEY = KeyCode.LeftControl;
    public const KeyCode LARGE_ZOOM_KEY = KeyCode.LeftShift;
    public const KeyCode ROTATE_ENTITIES_LEFT_KEY = KeyCode.Q;
    public const KeyCode ROTATE_ENTITIES_RIGHT_KEY = KeyCode.E;
    public const KeyCode DELETE_ENTITIES_KEY = KeyCode.Backspace;
    public const KeyCode ADDITIVE_SELECTION_KEY = KeyCode.LeftShift;
    public const KeyCode UNDO_KEY = KeyCode.Comma;
    public const KeyCode REDO_KEY = KeyCode.Period;
    public const KeyCode CTL_Key = KeyCode.LeftControl;
    public const KeyCode CMD_Key = KeyCode.LeftCommand;
    public const KeyCode COPY_Key = KeyCode.C;
    public const KeyCode PASTE_Key = KeyCode.V;


    // renderer sorting layers
    public const string SORTING_LAYER_DEFAULT = "Default";
    public const string SORTING_LAYER_ENTITY_SELECTED = "EntitySelected";
    public const string SORTING_LAYER_ENTITY_DRAGGING = "EntityDragging";


    // styling
    public const int GUI_FONT_SIZE = 20;


    // dev settings
    public const bool DISPLAY_TELEMETRY = true;


}
