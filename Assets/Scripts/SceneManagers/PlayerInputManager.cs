using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{


    // Manages Player->Game interaction processing.


    // input mode
    public int inputMode;

    // menu
    public GameObject MenuGO;

    // camera
    private float targetCameraSize;
    private Vector3 targetCameraPositionWorld;
    private float[] cameraBounds;

    // mouse interaction
    private Vector3 currentMousePositionWorld;
    private Vector3 initialMultiselectMousePosition;
    public GameObject selectionBoxPrefab;
    public GameObject selectionBoxGO;
    private bool mouseIsUIHovered;

    // entity selection
    private GameObject hoveredEntity;
    private List<GameObject> currentEntitiesSelected = new List<GameObject>();
    private IDictionary<int, Vector3> entityIdToMouseOffset;

    // inventory hotkey
    public List<InventoryItem> inventoryItemScripts = new List<InventoryItem>();
    private GameObject inventoryHotkeyPrefab;
    public Quaternion inventoryHotkeyMemRotation = Quaternion.identity;

    // inventory canvas
    public GameObject inventoryCanvas;


    // UNITY HOOKS

    void Start()
    {
        this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
        this.MenuGO.SetActive(false);
        this.targetCameraSize = Camera.main.orthographicSize;
        this.targetCameraPositionWorld = Camera.main.transform.position;
        this.selectionBoxGO = Instantiate(this.selectionBoxPrefab, Vector3.zero, Quaternion.identity);
        this.selectionBoxGO.SetActive(false);
    }

    void Update()
    {
        // set state
        this.currentMousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // player input
        this.CheckEscPress();
        // camera
        if (this.inputMode != GameSettings.INPUT_MODE_MENU)
        {
            // camera interaction
            this.HandleCameraMovement();
            this.HandleCameraZoom();
            // entity interaction
            this.HandleEntityDeleteByKeyDown();
            this.HandleEntityRotation();
            this.HandleEntityStateUndoRedo();
            this.HandleMouseEntityInteraction();
        }
    }

    // INTF METHODS

    public void InitEntitySelect()
    {
        // Debug.Log("initializing entity select for: " + this.currentEntitiesSelected.Count.ToString() + " entities");
        // init selection and dragging on all currently selected entities
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            e.GetComponent<Selectable>().SetSelected(false);
            e.GetComponent<Draggable>().SetDragging(false);
        }
        this.currentEntitiesSelected = new List<GameObject>();
        this.entityIdToMouseOffset = new Dictionary<int, Vector3>();
        // init selection box as well
        this.selectionBoxGO.SetActive(false);
    }

    public void SelectSingleEntity(GameObject entity)
    {
        this.TrySelectEntities(new List<GameObject>() { entity });
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.entityIdToMouseOffset.Add(entity.GetInstanceID(), entity.transform.position - mousePosition);
    }

    public void DisplayImpendingDeleteForSelectedEntities(bool status)
    {
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            e.GetComponent<Selectable>().SetPendingDelete(status);
        }
    }

    public void DeleteSelectedEntities()
    {
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(e, trackHistory: true);
            Destroy(e);
        }
        this.InitEntitySelect();
    }

    public bool AreCurrentSelectedEntitiesNewlyCreated()
    {
        bool areNewlyCreated = true;
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            if (!e.GetComponent<GameEntity>().isNewlyCreated)
            {
                areNewlyCreated = false;
            }
        }
        return areNewlyCreated;
    }

    public void SetInventoryHotkeyPrefab(GameObject entityPrefab)
    {
        this.inventoryHotkeyPrefab = entityPrefab;
    }

    public GameObject CreateInventoryHotkeyEntity(Quaternion rotation)
    {
        Vector3 quantizedPosition = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        quantizedPosition.z = 0;
        GameObject spawned = Instantiate(this.inventoryHotkeyPrefab, quantizedPosition, rotation);
        if (spawned != null)
        {
            spawned.GetComponent<GameEntity>().isNewlyCreated = true;
        }
        else
        {
            Debug.LogWarning("Could not create inventory hotkey entity at position: " + quantizedPosition.ToString());
        }
        return spawned;
    }

    public void ClearInventoryHotkeyEntity()
    {
        this.inventoryHotkeyPrefab = null;
        this.inventoryHotkeyMemRotation = Quaternion.identity;
    }

    public void DoEntitySelectionWithSelectionBox()
    {
        this.HandleBoxSelection();
    }

    // IMPL METHODS

    private void CheckEscPress()
    {
        if (Input.GetKeyDown(GameSettings.ESC_KEY))
        {
            // exit inventory hotkey mode
            if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
            {
                this.ClearInventoryHotkeyEntity();
                this.DeleteSelectedEntities();
                this.InitEntitySelect();
                this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
                // manually deactivate all inventory item flags
                foreach (GameObject inventoryItem in GameObject.FindGameObjectsWithTag("InventoryItem"))
                {
                    inventoryItem.GetComponent<InventoryItem>().DeactivateHotkey();
                }
            }
            // deselect entities
            else if (this.currentEntitiesSelected.Count > 0)
            {
                this.InitEntitySelect();
            }
            // should enter menu mode, if on any other mode
            else
            {
                bool isEnteringMenuMode = !(this.inputMode == GameSettings.INPUT_MODE_MENU);
                this.MenuGO.SetActive(isEnteringMenuMode);
                this.inputMode = isEnteringMenuMode ? GameSettings.INPUT_MODE_MENU : GameSettings.INPUT_MODE_DEFAULT;
                this.InitEntitySelect();
            }
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
        // detect mouse at edge of viewport
        else
        {
            float cameraMovePaddingY = Screen.height - (Screen.height * 0.995f);
            float cameraMovePaddingX = Screen.width - (Screen.width * 0.995f);
            Vector3 cameraMoveDirection = Vector3.zero;
            if (Input.mousePosition.y > Screen.height - cameraMovePaddingY)
            {
                cameraMoveDirection += Vector3.up;
            }
            if (Input.mousePosition.y < 0 + cameraMovePaddingY)
            {
                cameraMoveDirection += Vector3.down;
            }
            if (Input.mousePosition.x > Screen.width - cameraMovePaddingX)
            {
                cameraMoveDirection += Vector3.right;
            }
            if (Input.mousePosition.x < 0 + cameraMovePaddingX)
            {
                cameraMoveDirection += Vector3.left;
            }
            if (cameraMoveDirection != Vector3.zero)
            {
                Camera.main.transform.Translate(cameraMoveDirection * Time.deltaTime * Camera.main.orthographicSize * GameSettings.CAMERA_MOVE_SPEED * 0.6f);
            }
        }
        this.ClampCameraToPlayzone();
    }

    private void ClampCameraToPlayzone()
    {
        // set camera bounds
        float camOffsetX = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).x - Camera.main.transform.position.x;
        float camOffsetY = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).y - Camera.main.transform.position.y;
        float xBound = (GameSettings.GRID_SIZE / 2) + camOffsetX;
        float yBound = (GameSettings.GRID_SIZE / 2) + camOffsetY;
        // clamp cam position
        var camPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(camPos.x, -xBound, xBound),
            Mathf.Clamp(camPos.y, -yBound, yBound),
            camPos.z
        );
    }

    private void HandleCameraZoom()
    {
        float currCameraSize = Camera.main.orthographicSize;
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                this.targetCameraPositionWorld = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + Camera.main.transform.position) / 2;
            }
            else
            {
                this.targetCameraPositionWorld = Camera.main.transform.position;
            }
            this.targetCameraSize = currCameraSize - (Input.mouseScrollDelta.y * GameSettings.CAMERA_ZOOM_AMOUNT);
            // clamp camera size
            if (this.targetCameraSize < GameSettings.CAMERA_SIZE_MIN)
            {
                this.targetCameraSize = GameSettings.CAMERA_SIZE_MIN;
            }
            else if (this.targetCameraSize > GameSettings.CAMERA_SIZE_MAX)
            {
                this.targetCameraSize = GameSettings.CAMERA_SIZE_MAX;
            }
        }
        if (Mathf.Abs(currCameraSize - this.targetCameraSize) > 0.1f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(currCameraSize, this.targetCameraSize, Time.deltaTime * GameSettings.CAMERA_ZOOM_SPEED);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, this.targetCameraPositionWorld, Time.deltaTime * (GameSettings.CAMERA_ZOOM_SPEED));
        }
    }

    private void HandleMouseEntityInteraction()
    {
        this.mouseIsUIHovered = this.MouseIsUIHovered();
        Collider2D[] mousePointHits = Physics2D.OverlapPointAll(this.currentMousePositionWorld);
        this.hoveredEntity = this.GetHoveredSelectableEntity(mousePointHits);
        // button down
        if (Input.GetMouseButtonDown(0))
        {
            if (this.mouseIsUIHovered)
            {
                // noop
            }
            else if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT)
            {
                // entity click
                if (this.hoveredEntity != null)
                {
                    this.HandleEntityClicked(this.hoveredEntity);
                }
                // initialize the selection-box
                else
                {
                    this.HandleStartSelectionBox();
                }
            }
            // place inventory-hotkey entity
            else if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
            {
                this.HandleHotkeyEntityPlacement();
            }
        }
        // button held
        else if (Input.GetMouseButton(0))
        {
            if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT)
            {
                // update the position and shape of the selection-box
                if (this.selectionBoxGO.activeSelf)
                {
                    this.HandleSelectionBoxSizing();
                }
                // drag selected entities
                else
                {
                    this.HandleEntityDrag();
                }
            }
            // continue dropping inventory-hotkey entities
            else if (!this.mouseIsUIHovered && this.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
            {
                this.HandleEntityDrag();
                this.HandleHotkeyEntityPlacement();
            }
        }
        // button up
        else if (Input.GetMouseButtonUp(0))
        {
            if (this.mouseIsUIHovered)
            {
                // noop
            }
            else if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT)
            {
                // box selection
                if (this.selectionBoxGO.activeSelf)
                {
                    this.HandleBoxSelection();
                }
                // end of drag
                else
                {
                    this.HandleEntityDrop();
                }
            }
            // drop final entity from inventory-hotkey drag
            else if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
            {
                this.HandleHotkeyEntityPlacement();
                PlaySceneManager.instance.gameEntityManager.StartNewEntityStateHistoryStep();
            }
        }
        // mouse move
        else
        {
            if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
            {
                this.HandleEntityDrag();
            }
        }
    }

    private void HandleEntityClicked(GameObject clickedEntity)
    {
        this.entityIdToMouseOffset = new Dictionary<int, Vector3>();
        // multi entity start drag
        if (this.currentEntitiesSelected.Count > 0 && this.currentEntitiesSelected.Contains(clickedEntity))
        {
            // set selected entity initial offsets from mouse position to prepare for entity drag
            foreach (GameObject e in this.currentEntitiesSelected)
            {
                int instanceId = e.GetInstanceID();
                if (!this.entityIdToMouseOffset.ContainsKey(instanceId))
                {
                    this.entityIdToMouseOffset.Add(e.GetInstanceID(), e.transform.position - Functions.RoundVector(this.currentMousePositionWorld));
                }
            }
        }
        // single entity selection
        else
        {
            if (Input.GetKey(GameSettings.ADDITIVE_SELECTION_KEY))
            {
                this.TrySelectEntities(new List<GameObject>() { clickedEntity });
            }
            else
            {
                this.InitEntitySelect();
                this.SelectSingleEntity(clickedEntity);
            }
        }
        // set pre-drag positions for currently selected entities
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            Draggable draggable = e.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.preDragPosition = draggable.transform.position;
            }
        }
    }

    private void HandleStartSelectionBox()
    {
        this.selectionBoxGO.SetActive(true);
        this.selectionBoxGO.transform.localScale = Vector3.zero;
        this.initialMultiselectMousePosition = this.currentMousePositionWorld;
    }

    private void HandleSelectionBoxSizing()
    {
        Vector3 mPos1 = this.currentMousePositionWorld;
        Vector3 mPos2 = this.initialMultiselectMousePosition;
        float width = Mathf.Abs(mPos1.x - mPos2.x);
        float height = Mathf.Abs(mPos1.y - mPos2.y);
        Vector3 midpoint = (mPos1 - mPos2) / 2;
        this.selectionBoxGO.transform.localScale = new Vector3(width, height, 0);
        Vector3 boxPos = mPos1 - midpoint;
        this.selectionBoxGO.transform.position = new Vector3(boxPos.x, boxPos.y, 0);
    }

    private void HandleBoxSelection()
    {
        if (!Input.GetKey(GameSettings.ADDITIVE_SELECTION_KEY))
        {
            this.InitEntitySelect();
        }
        if (this.currentMousePositionWorld != this.initialMultiselectMousePosition)
        {
            var entitiesToSelect = new List<GameObject>();
            Vector3 mPos1 = this.currentMousePositionWorld;
            Vector3 mPos2 = this.initialMultiselectMousePosition;
            Collider2D[] selectionBoxHits = Physics2D.OverlapAreaAll(mPos1, mPos2);
            foreach (Collider2D col in selectionBoxHits)
            {
                if (col.gameObject.GetComponent<Selectable>())
                {
                    entitiesToSelect.Add(col.gameObject);
                }
            }
            this.TrySelectEntities(entitiesToSelect);
        }
        this.selectionBoxGO.SetActive(false);
    }

    private void HandleEntityDrag()
    {
        // get draggables from selected entities and move with respect to mouse position
        Vector3 quantizedMousePos = Functions.RoundVector(this.currentMousePositionWorld);
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            Draggable draggable = e.GetComponent<Draggable>();
            if (draggable != null && this.entityIdToMouseOffset.ContainsKey(e.GetInstanceID()))
            {
                Vector3 offset = this.entityIdToMouseOffset[e.GetInstanceID()];
                e.transform.position = new Vector3(
                    quantizedMousePos.x + offset.x,
                    quantizedMousePos.y + offset.y,
                    e.transform.position.z
                );
                if (GameSettings.ENTITY_POSITIONS_DISCRETE)
                {
                    e.transform.position = Functions.RoundVector(e.transform.position);
                }
                // is already dragging
                if (draggable.isDragging)
                {
                    // noop
                }
                // drag initiated
                else
                {
                    draggable.SetDragging(true);
                    if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT)
                    {
                        PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(e, trackHistory: true);
                    }
                }
            }
        }
    }

    private void HandleEntityDrop()
    {
        // TODO: make impl more efficient

        // collect draggables subset
        List<GameObject> draggables = new List<GameObject>();
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            if (e.GetComponent<Draggable>() != null)
            {
                draggables.Add(e);
            }
        }
        // check if there are any invalid drop positions
        bool invalidDragDetected = false;
        foreach (GameObject e in draggables)
        {
            if (!e.GetComponent<Draggable>().PositionIsValid())
            {
                invalidDragDetected = true;
            }
        }
        // if any invalid drags detected
        // delete newly created entities and init select
        // otherwise, roll-back positions to pre-drag positions
        if (invalidDragDetected)
        {
            if (this.AreCurrentSelectedEntitiesNewlyCreated())
            {
                foreach (GameObject e in draggables)
                {
                    PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(e);
                    Destroy(e);
                }
                this.InitEntitySelect();
            }
            else
            {
                foreach (GameObject e in draggables)
                {
                    e.transform.position = e.GetComponent<Draggable>().preDragPosition;
                }
            }
        }
        // commit drops and mark any newly created entities as no longer newly created
        foreach (GameObject e in draggables)
        {
            e.GetComponent<Draggable>().SetDragging(false);
            e.GetComponent<GameEntity>().isNewlyCreated = false;
            PlaySceneManager.instance.gameEntityManager.AddGameEntity(e, trackHistory: true);
        }
    }

    private void HandleEntityRotation()
    {
        int rot = 0;
        if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_LEFT_KEY))
        {
            rot += 90;
        }
        if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_RIGHT_KEY))
        {
            rot -= 90;
        }
        if (rot != 0)
        {
            foreach (GameObject e in this.currentEntitiesSelected)
            {
                e.transform.Rotate(new Vector3(0, 0, rot));
            }
        }
    }

    private void HandleEntityDeleteByKeyDown()
    {
        if (this.currentEntitiesSelected.Count > 0 && Input.GetKeyDown(GameSettings.DELETE_ENTITIES_KEY))
        {
            this.DeleteSelectedEntities();
            PlaySceneManager.instance.gameEntityManager.StartNewEntityStateHistoryStep();
        }
    }

    private void HandleEntityStateUndoRedo()
    {
        // STUB
    }

    private void HandleHotkeyEntityPlacement()
    {
        // BUG: something is wrong with the validation here
        // check if there are any invalid drop positions
        bool dropIsValid = true;
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            if (!e.GetComponent<Draggable>().PositionIsValid())
            {
                dropIsValid = false;
            }
        }
        if (dropIsValid)
        {
            // commit placement, init, and create another entity
            foreach (GameObject e in this.currentEntitiesSelected)
            {
                this.inventoryHotkeyMemRotation = e.transform.rotation;
                e.GetComponent<Draggable>().SetDragging(false);
                e.GetComponent<GameEntity>().isNewlyCreated = false;
                PlaySceneManager.instance.gameEntityManager.AddGameEntity(e, trackHistory: true);
            }
            this.InitEntitySelect();
            GameObject spawned = this.CreateInventoryHotkeyEntity(this.inventoryHotkeyMemRotation);
            this.SelectSingleEntity(spawned);
        }
        else
        {
            // Debug.Log("inventory hotkey entity drop not valid");
        }
    }

    private bool MouseIsUIHovered()
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem es = this.inventoryCanvas.GetComponent<EventSystem>();
        PointerEventData ped = new PointerEventData(es);
        ped.position = Input.mousePosition;
        this.inventoryCanvas.GetComponent<GraphicRaycaster>().Raycast(ped, raycastResults);
        return raycastResults.Count > 0;
    }

    private GameObject GetHoveredSelectableEntity(Collider2D[] hits)
    {
        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.gameObject.GetComponent<Selectable>() != null)
            {
                return hit.gameObject;
            }
        }
        return null;
    }

    private void TrySelectEntities(List<GameObject> entities)
    {
        foreach (GameObject entity in entities)
        {
            var selectable = entity.GetComponent<Selectable>();
            if (selectable != null)
            {
                this.currentEntitiesSelected.Add(entity);
                selectable.SetSelected(true);
                Draggable draggable = selectable.gameObject.GetComponent<Draggable>();
                if (draggable != null)
                {
                    draggable.preDragPosition = draggable.transform.position;
                }
            }
        }
    }


}
