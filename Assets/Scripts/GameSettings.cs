using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{


    // dev settings
    public const bool DISPLAY_TELEMETRY = false;
    public const bool DISPLAY_UI_DEBUG = false;
    public const bool IS_ADMIN = false;

    // general settings
    public const int GRID_SIZE = 101; // must be odd
    public const int ENTITY_STATE_MAX_HISTORY = 1000;
    public const float DEFAULT_TICK_DURATION = 0.4f;
    public const float FAST_ANIMATION_DURATION = 0.07f;
    public const float AUTOSAVE_SECONDS = 30f;
    public const bool CAMERA_EDGE_SCROLL_ON = true;

    // camera settings
    public const float CAMERA_SIZE_MIN = 2f;
    public const float CAMERA_SIZE_MAX = 20f;
    public const float CAMERA_ZOOM_AMOUNT = 6f;
    public const float CAMERA_ZOOM_SPEED = 8f;
    public const float CAMERA_MOVE_SPEED = 4f;

    // input modes
    public const string INPUT_MODE_DEFAULT = "INPUT_MODE_DEFAULT";
    public const string INPUT_MODE_MULTIPLACEMENT = "INPUT_MODE_MULTIPLACEMENT";
    public const string INPUT_MODE_MENU = "INPUT_MODE_MENU";
    public const string INPUT_MODE_INVENTORY = "INPUT_MODE_INVENTORY";

    // game entity grid layers
    public const string GAME_ENTITY_GRID_LAYER_GROUND = "Ground";
    public const string GAME_ENTITY_GRID_LAYER_OBJECTS = "Objects";

    // player input mappings
    public const KeyCode ESC_KEY = KeyCode.Escape;
    public const KeyCode ROTATE_ENTITIES_LEFT_KEY = KeyCode.Q;
    public const KeyCode ROTATE_ENTITIES_RIGHT_KEY = KeyCode.E;
    public const KeyCode DELETE_ENTITIES_KEY = KeyCode.Backspace;
    public const KeyCode ADDITIVE_SELECTION_KEY = KeyCode.LeftShift;
    public const KeyCode UNDO_KEY = KeyCode.Comma;
    public const KeyCode REDO_KEY = KeyCode.Period;
    public const KeyCode CTL_KEY = KeyCode.LeftControl;
    public const KeyCode CMD_KEY = KeyCode.LeftCommand;
    public const KeyCode COPY_KEY = KeyCode.C;
    public const KeyCode PLAY_PAUSE_KEY = KeyCode.Space;
    public const KeyCode INVENTORY_KEY = KeyCode.Tab;

    // renderer sorting layers
    public const string SORTING_LAYER_DEFAULT = "Default";
    public const string SORTING_LAYER_ENTITY_SELECTED = "EntitySelected";
    public const string SORTING_LAYER_ENTITY_DRAGGING = "EntityDragging";

    // styling
    public const int GUI_FONT_SIZE = 20;


}
