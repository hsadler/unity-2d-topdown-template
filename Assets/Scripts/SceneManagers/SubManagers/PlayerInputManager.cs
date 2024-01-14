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
    public string inputMode;

    // modals
    public GameObject menuModalGO;
    public GameObject inventoryModalGO;

    // camera
    private float targetCameraSize;
    private Vector3 targetCameraPositionWorld;

    // mouse interaction
    private Vector3 currentMousePositionWorld;
    private Vector3 quantizedMousePos;
    private Vector3 initialMultiselectMousePosition;
    public GameObject selectionBoxPrefab;
    [HideInInspector]
    public GameObject selectionBoxGO;
    private bool mouseIsUIHovered;

    // entity selection
    private GameObject hoveredEntity;
    private List<GameObject> currentEntitiesSelected = new();

    // entity drag
    public bool isEntityDragging;
    public GameObject entityDragContainer;

    // entity group rotation
    private Quaternion entityGroupRotationTarget = Quaternion.identity;
    private Coroutine entityGroupRotationCoroutine = null;

    // entity copy + paste
    private List<GameObject> copyPasteEntities = new();

    // multi-placement
    private List<GameObject> multiPlacementEntities = new();

    // inventory canvas
    public GameObject hotbarCanvas;

    // manager references
    [HideInInspector]
    public GameEntityRepoManager gameEntityRepoManager;
    [HideInInspector]
    public GameEntityManager gameEntityManager;
    [HideInInspector]
    public TickManager tickManager;

    // debug
    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
        this.mouseIsUIHovered = false;
        this.isEntityDragging = false;
        this.menuModalGO.SetActive(false);
        this.inventoryModalGO.SetActive(false);
        this.targetCameraSize = Camera.main.orthographicSize;
        this.targetCameraPositionWorld = Camera.main.transform.position;
        this.selectionBoxGO = Instantiate(this.selectionBoxPrefab, Vector3.zero, Quaternion.identity);
        this.selectionBoxGO.SetActive(false);
        // set manager refs
        this.gameEntityRepoManager = PlaySceneManager.instance.gameEntityRepoManager;
        this.gameEntityManager = PlaySceneManager.instance.gameEntityManager;
        this.tickManager = PlaySceneManager.instance.tickManager;
    }

    void Update()
    {
        // set state
        this.currentMousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.quantizedMousePos = Functions.QuantizeVector(this.currentMousePositionWorld);
        // move drag group container to match quantized mouse position
        this.entityDragContainer.transform.position = new Vector3(
            this.quantizedMousePos.x,
            this.quantizedMousePos.y,
            0
        );
        // player input
        this.CheckEscKeyPress();
        if (this.inputMode != GameSettings.INPUT_MODE_MENU)
        {
            this.CheckInventoryKeyPress();
        }
        if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT || this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT)
        {
            // camera interaction
            this.HandleCameraMovement();
            this.HandleCameraZoom();
            // entity interaction
            this.HandleMouseEntityInteraction();
            if (!this.tickManager.tickIsRunning)
            {
                this.HandleEntityDeleteByKeyDown();
                this.HandleEntityRotation();
                this.HandleEntityCopyPaste();
                if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT)
                {
                    this.HandleEntityStateUndoRedo();
                }
            }
        }
    }

    // INTF METHODS

    public List<GameObject> GetEntitiesSelected()
    {
        var entitiesSelected = new List<GameObject>();
        foreach (var go in this.currentEntitiesSelected)
        {
            if (go != null)
            {
                entitiesSelected.Add(go);
            }
        }
        return entitiesSelected;
    }

    public List<GameObject> GetMulitPlacementEntities()
    {
        return this.multiPlacementEntities;
    }

    public GameObject GetHoveredEntity()
    {
        return this.hoveredEntity;
    }

    public GameObject GetEntityDragContainer()
    {
        return this.entityDragContainer;
    }

    public void InitEntitySelect()
    {
        // init selection and dragging on all currently selected entities
        var entitiesSelected = this.GetEntitiesSelected();
        if (this.useLogging) { Debug.Log("Init entity select for entity count: " + entitiesSelected.Count.ToString()); }
        foreach (GameObject e in entitiesSelected)
        {
            var selectable = e.GetComponent<GameEntity>().GetSelectable();
            if (selectable != null)
            {
                selectable.SetSelected(false);
            }
            var draggable = e.GetComponent<GameEntity>().GetDraggable();
            if (draggable != null)
            {
                draggable.SetDragging(false);
            }
        }
        this.currentEntitiesSelected = new List<GameObject>();
        // init selection box as well
        this.selectionBoxGO.SetActive(false);
    }

    public void SelectSingleEntity(GameObject entity)
    {
        this.TrySelectEntities(new List<GameObject>() { entity });
    }

    public void DisplayImpendingDeleteForSelectedEntities(bool status)
    {
        foreach (GameObject e in this.GetEntitiesSelected())
        {
            var selectable = e.GetComponent<GameEntity>().GetSelectable();
            if (selectable != null)
            {
                selectable.SetPendingDelete(status);
            }
        }
    }

    public void DeleteSelectedEntities(bool forceDelete = false)
    {
        foreach (GameObject e in this.GetEntitiesSelected())
        {
            GameEntity geScript = e.GetComponent<GameEntity>();
            var selectable = geScript.GetSelectable();
            if (forceDelete || selectable != null)
            {
                this.gameEntityManager.RemoveGameEntity(geScript.gridLayer, e);
                Destroy(e);
            }
        }
        this.InitEntitySelect();
    }

    public bool AreCurrentSelectedEntitiesNewlyCreated()
    {
        bool areNewlyCreated = true;
        foreach (GameObject e in this.GetEntitiesSelected())
        {
            if (!e.GetComponent<GameEntity>().isNewlyCreated)
            {
                areNewlyCreated = false;
            }
        }
        return areNewlyCreated;
    }

    public void StartMultiPlacement(List<GameObject> entities)
    {
        if (this.useLogging) { Debug.Log("Starting multi-placement for entity count: " + entities.Count.ToString()); }
        this.multiPlacementEntities = entities;
        List<GameObject> spawned = this.CreateMultiPlacementEntities();
        if (this.useLogging) { Debug.Log("Spawned multi-placement entities: " + spawned.Count.ToString()); }
        // reposition using most center entity position
        Vector3 mostCenterPos = Functions.MostCenterGameObject(spawned).transform.position;
        if (this.useLogging) { Debug.Log("Most center pos: " + mostCenterPos.ToString()); }
        this.entityDragContainer.transform.position = Functions.QuantizeVector(mostCenterPos);
        foreach (var e in spawned)
        {
            if (e != null)
            {
                e.transform.SetParent(this.entityDragContainer.transform);
            }
        }
        // reposition to mouse cursor
        this.entityDragContainer.transform.position = this.quantizedMousePos;
        this.InitEntitySelect();
        this.TrySelectEntities(spawned);
        this.inputMode = GameSettings.INPUT_MODE_MULTIPLACEMENT;
    }

    public void ExitMultiPlacement()
    {
        this.multiPlacementEntities = new List<GameObject>();
        this.DeleteSelectedEntities(forceDelete: true);
        this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
    }

    public List<GameObject> CreateMultiPlacementEntities()
    {
        var spawned = new List<GameObject>();
        foreach (var e in this.multiPlacementEntities)
        {
            GameObject prefab = this.gameEntityRepoManager.GetGameEntityPrefabByName(e.GetComponent<GameEntity>().prefabName);
            GameObject newEntity = Instantiate(
                prefab,
                Functions.QuantizeVector(e.transform.position),
                Functions.QuantizeQuaternion(e.transform.rotation)
            );
            spawned.Add(newEntity);
        }
        return spawned;
    }

    public void DoEntitySelectionWithSelectionBox()
    {
        this.HandleBoxSelection();
    }

    public void CancelEntityDrag()
    {
        // if any invalid drags detected
        // delete newly created entities and init select
        // otherwise, roll-back positions to pre-drag positions
        var draggables = this.GetCurrentSelectedDraggables();
        if (this.useLogging)
        {
            Debug.Log("Cancelling entity drag for entity count: " + draggables.Count.ToString());
        }
        if (this.AreCurrentSelectedEntitiesNewlyCreated())
        {
            foreach (GameObject e in draggables)
            {
                GameEntity geScript = e.GetComponent<GameEntity>();
                this.gameEntityManager.RemoveGameEntity(geScript.gridLayer, e);
                Destroy(e);
            }
            this.InitEntitySelect();
        }
        else
        {
            foreach (GameObject e in draggables)
            {
                e.transform.SetPositionAndRotation(
                    e.GetComponent<Draggable>().preDragPosition,
                    e.GetComponent<Draggable>().preDragRotation
                );
            }
        }
    }

    public void CommitEntityDrop()
    {
        // commit drops and mark any newly created entities as no longer newly created
        var draggables = this.GetCurrentSelectedDraggables();
        if (this.useLogging)
        {
            Debug.Log("Committing entity drop for entity count: " + draggables.Count.ToString());
        }
        foreach (GameObject e in draggables)
        {
            GameEntity geScript = e.GetComponent<GameEntity>();
            e.GetComponent<Draggable>().SetDragging(false);
            geScript.isNewlyCreated = false;
            this.gameEntityManager.AddGameEntity(geScript.gridLayer, e, e.transform.position);
        }
        this.UngroupDraggingEntities(this.GetEntitiesSelected());
        this.isEntityDragging = false;
    }

    // Camera position and zoom

    public void SetCameraPosition(Vector3 position)
    {
        Camera.main.transform.position = position;
        this.targetCameraPositionWorld = position;
    }

    public Vector3 GetCameraPosition()
    {
        return Camera.main.transform.position;
    }

    public void SetCameraZoom(float zoom)
    {
        Camera.main.orthographicSize = zoom;
        this.targetCameraSize = zoom;
    }

    public float GetCameraZoom()
    {
        return Camera.main.orthographicSize;
    }

    // IMPL METHODS

    private void CheckEscKeyPress()
    {
        if (Input.GetKeyDown(GameSettings.ESC_KEY))
        {
            // exit inventory multi-placement mode (must be first if check)
            if (this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT)
            {
                this.ExitMultiPlacement();
                this.DeselectHotbarItems();
            }
            // cancel entity drag if any entities are currently being dragged
            else if (this.isEntityDragging)
            {
                this.CancelEntityDrag();
                this.CommitEntityDrop();
                this.InitEntitySelect();
            }
            // deselect entities if any are selected
            else if (this.currentEntitiesSelected.Count > 0)
            {
                this.InitEntitySelect();
            }
            // exit inventory mode
            else if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY)
            {
                this.inventoryModalGO.SetActive(false);
                this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
            }
            // otherwise try stop tick play and enter menu mode
            else
            {
                PlaySceneManager.instance.tickManager.SetTickPlayState(false);
                bool isEnteringMenuMode = !(this.inputMode == GameSettings.INPUT_MODE_MENU);
                this.menuModalGO.SetActive(isEnteringMenuMode);
                this.inputMode = isEnteringMenuMode ? GameSettings.INPUT_MODE_MENU : GameSettings.INPUT_MODE_DEFAULT;
                this.InitEntitySelect();
            }
        }
    }

    private void CheckInventoryKeyPress()
    {
        if (Input.GetKeyDown(GameSettings.INVENTORY_KEY))
        {
            if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY)
            {
                this.inventoryModalGO.SetActive(false);
                this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
                return;
            }
            else if (this.inputMode == GameSettings.INPUT_MODE_MENU)
            {
                // noop
                return;
            }
            else if (this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT)
            {
                this.ExitMultiPlacement();
                this.DeselectHotbarItems();
            }
            this.inventoryModalGO.SetActive(true);
            this.inputMode = GameSettings.INPUT_MODE_INVENTORY;
        }
    }

    private void DeselectHotbarItems()
    {
        foreach (GameObject inventoryItem in GameObject.FindGameObjectsWithTag("HotbarItem"))
        {
            inventoryItem.GetComponent<HotbarItemUI>().DeactivateHotbarItem();
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
        // detect mouse at edge of viewport and scroll if setting is on
#pragma warning disable 0162
        else if (GameSettings.CAMERA_EDGE_SCROLL_ON)
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
                Camera.main.transform.Translate(0.6f * Camera.main.orthographicSize * GameSettings.CAMERA_MOVE_SPEED * Time.deltaTime * cameraMoveDirection);
            }
        }
#pragma warning restore 0162
        this.ClampCameraToPlayzone();
    }

    private void ClampCameraToPlayzone()
    {
        // camera center cannot go beyond the play area
        float xBound = GameSettings.GRID_SIZE / 2;
        float yBound = GameSettings.GRID_SIZE / 2;
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
        //
        // Top-level mouse interaction handler. Allows for entity selection,
        // drag, and placement. Drag and placement are not allowed while the
        // tick is running.
        //
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
                // entity click and start drag
                if (this.hoveredEntity != null)
                {
                    this.HandleEntityClicked(this.hoveredEntity);
                    if (!this.tickManager.tickIsRunning)
                    {
                        this.isEntityDragging = true;
                        this.TryGroupingDraggableEntities(this.GetEntitiesSelected());
                        this.HandleEntityDrag();
                    }
                }
                // initialize the selection box
                else
                {
                    this.HandleStartSelectionBox();
                }
            }
            // multi-placement
            else if (this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT && !this.tickManager.tickIsRunning)
            {
                this.HandleMultiEntityPlacement();
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
                else if (!this.tickManager.tickIsRunning)
                {
                    this.HandleEntityDrag();
                    this.isEntityDragging = true;
                }
            }
            // continue multi-placement of entities
            else if (!this.mouseIsUIHovered && this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT && !this.tickManager.tickIsRunning)
            {
                if (this.useLogging) { Debug.Log("Continuing multi-placement"); }
                this.HandleEntityDrag();
                this.HandleMultiEntityPlacement();
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
                else if (!this.tickManager.tickIsRunning)
                {
                    this.HandleEntityDrop();
                }
            }
            // drop final entity from multi-placement drag
            else if (this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT && !this.tickManager.tickIsRunning)
            {
                this.HandleMultiEntityPlacement();
                this.gameEntityManager.TryPushEntityStateHistoryStep();
            }
        }
        // mouse move + multi-placement
        else if (this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT && !this.tickManager.tickIsRunning)
        {
            this.HandleEntityDrag();
        }
    }

    private void HandleEntityClicked(GameObject clickedEntity)
    {
        //
        // handles multi-entity drag, single entity selection, and additive selection
        //
        if (this.useLogging) { Debug.Log("Entity clicked: " + clickedEntity.name); }
        // multi entity start drag
        var entitiesSelected = this.GetEntitiesSelected();
        if (entitiesSelected.Count > 0 && entitiesSelected.Contains(clickedEntity))
        {
            if (this.useLogging) { Debug.Log("Multi entity start drag"); }
        }
        // single entity selection
        else
        {
            if (Input.GetKey(GameSettings.ADDITIVE_SELECTION_KEY))
            {
                if (this.useLogging) { Debug.Log("Additive selection"); }
                this.TrySelectEntities(new List<GameObject>() { clickedEntity });
            }
            else
            {
                if (this.useLogging) { Debug.Log("Single entity selection"); }
                this.InitEntitySelect();
                this.SelectSingleEntity(clickedEntity);
            }
        }
        // set pre-drag positions and rotations for currently selected entities
        foreach (GameObject e in this.GetEntitiesSelected())
        {
            var draggable = e.GetComponent<GameEntity>().GetDraggable();
            if (draggable != null)
            {
                draggable.preDragPosition = draggable.transform.position;
                draggable.preDragRotation = draggable.transform.rotation;
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
                var selectable = col.gameObject.GetComponent<GameEntity>().GetSelectable();
                if (selectable != null)
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
        // get draggables subset from selected entities
        List<GameObject> draggables = this.GetCurrentSelectedDraggables();
        // set draggable state on entities and remove from game-entity-manager if needed
        foreach (var e in draggables)
        {
            GameEntity geScript = e.GetComponent<GameEntity>();
            Draggable draggable = geScript.GetDraggable();
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
                    this.gameEntityManager.RemoveGameEntity(geScript.gridLayer, e);
                }
            }
        }
    }

    private void HandleEntityDrop()
    {
        // check if there are any invalid drop positions among the selected draggable entities
        bool invalidDragDetected = false;
        foreach (GameObject e in this.GetCurrentSelectedDraggables())
        {
            if (!e.GetComponent<Draggable>().PositionIsValid())
            {
                invalidDragDetected = true;
            }
        }
        if (invalidDragDetected)
        {
            this.CancelEntityDrag();
        }
        this.CommitEntityDrop();
        this.gameEntityManager.TryPushEntityStateHistoryStep();
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
        var entitiesSelected = this.GetEntitiesSelected();
        if (rot != 0 && entitiesSelected.Count > 0)
        {
            if (this.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT || this.isEntityDragging)
            {
                // rotate selected entities as a group
                this.RotateSelectedEntitiesAsGroup(rotationAmount: rot);
            }
            else
            {
                if (this.useLogging) { Debug.Log("Rotating selected entities as individuals"); }
                // rotate selected entities as individuals
                foreach (GameObject e in entitiesSelected)
                {
                    var rotatable = e.GetComponent<GameEntity>().GetRotatable();
                    if (rotatable != null)
                    {
                        rotatable.AddRotation(rot);
                        rotatable.CommitRotations(animationDuration: GameSettings.FAST_ANIMATION_DURATION);
                    }
                }
                // push history step only if input mode is default and entities are not currently being dragged
                if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT)
                {
                    this.gameEntityManager.TryPushEntityStateHistoryStep();
                }
            }
        }
    }

    private IEnumerator AnimateEntityGroupRotation(int rotationAmount)
    {
        // animate rotation of selected entities as a group
        if (this.useLogging) { Debug.Log("Animating entity group rotation"); }
        this.entityGroupRotationTarget = Quaternion.Euler(0, 0, this.entityDragContainer.transform.rotation.eulerAngles.z + rotationAmount);
        yield return Functions.RotateOverTime(
            go: this.entityDragContainer,
            startRotation: this.entityDragContainer.transform.rotation,
            endRotation: this.entityGroupRotationTarget,
            duration: GameSettings.FAST_ANIMATION_DURATION
        );
    }

    private void TryFastForwardEntityGroupRotation()
    {
        // fast-forward rotation of selected entities as a group
        if (this.useLogging) { Debug.Log("Trying to fast-forwarding entity group rotation"); }
        if (this.entityGroupRotationCoroutine != null)
        {
            StopCoroutine(this.entityGroupRotationCoroutine);
            this.entityDragContainer.transform.rotation = this.entityGroupRotationTarget;
        }
    }

    private void RotateSelectedEntitiesAsGroup(int rotationAmount)
    {
        // rotate selected entities as a group
        if (this.useLogging) { Debug.Log("Rotating selected entities as group"); }
        this.TryFastForwardEntityGroupRotation();
        this.entityGroupRotationCoroutine = StartCoroutine(this.AnimateEntityGroupRotation(rotationAmount));
    }

    private void HandleEntityDeleteByKeyDown()
    {
        if (this.GetEntitiesSelected().Count > 0 && Input.GetKeyDown(GameSettings.DELETE_ENTITIES_KEY))
        {
            this.DeleteSelectedEntities();
            this.gameEntityManager.TryPushEntityStateHistoryStep();
        }
    }

    private void HandleEntityStateUndoRedo()
    {
        string backOrForward = null;
        if (Input.GetKeyDown(GameSettings.UNDO_KEY))
        {
            backOrForward = "back";
        }
        else if (Input.GetKeyDown(GameSettings.REDO_KEY))
        {
            backOrForward = "forward";
        }
        if (backOrForward != null)
        {
            this.InitEntitySelect();
            this.gameEntityManager.GoStateHistoryStep(backOrForward);
        }
    }

    private void HandleEntityCopyPaste()
    {
        List<GameObject> entitiesSelected = this.GetEntitiesSelected();
        if (
            (Input.GetKey(GameSettings.CTL_KEY) || Input.GetKey(GameSettings.CMD_KEY)) &&
            Input.GetKeyDown(GameSettings.COPY_KEY) &&
            entitiesSelected.Count > 0
        )
        {
            if (this.useLogging) { Debug.Log("Handling Copy/Paste for entity amount: " + entitiesSelected.Count.ToString()); }
            this.copyPasteEntities = new List<GameObject>(entitiesSelected);
            this.InitEntitySelect();
            this.StartMultiPlacement(this.copyPasteEntities);
        }
    }

    private void HandleMultiEntityPlacement()
    {
        bool dropIsValid = true;
        var draggables = this.GetCurrentSelectedDraggables();
        foreach (GameObject e in draggables)
        {
            if (!e.GetComponent<GameEntity>().GetDraggable().PositionIsValid())
            {
                dropIsValid = false;
            }
        }
        if (dropIsValid)
        {
            // commit placement and re-init multi-placement
            foreach (GameObject e in draggables)
            {
                var geScript = e.GetComponent<GameEntity>();
                this.TryFastForwardEntityGroupRotation();
                geScript.GetDraggable().SetDragging(false);
                geScript.isNewlyCreated = false;
                this.gameEntityManager.AddGameEntity(geScript.gridLayer, e, e.transform.position);
                e.transform.SetParent(null);
            }
            this.StartMultiPlacement(this.GetEntitiesSelected());
        }
        else
        {
            if (this.useLogging) { Debug.Log("multi-placement entity drop not valid"); }
        }
    }

    private bool MouseIsUIHovered()
    {
        List<RaycastResult> raycastResults = new();
        EventSystem es = this.hotbarCanvas.GetComponent<EventSystem>();
        PointerEventData ped = new(es)
        {
            position = Input.mousePosition
        };
        this.hotbarCanvas.GetComponent<GraphicRaycaster>().Raycast(ped, raycastResults);
        return raycastResults.Count > 0;
    }

    private GameObject GetHoveredSelectableEntity(Collider2D[] hits)
    {
        foreach (Collider2D hit in hits)
        {
            if (hit != null)
            {
                var selectable = hit.gameObject.GetComponent<GameEntity>().GetSelectable();
                if (selectable != null)
                {
                    return hit.gameObject;
                }
            }
        }
        return null;
    }

    private void TrySelectEntities(List<GameObject> entities)
    {
        if (this.useLogging) { Debug.Log("Trying to select entities: " + entities.Count.ToString()); }
        foreach (GameObject entity in entities)
        {
            var selectable = entity.GetComponent<GameEntity>().GetSelectable();
            if (selectable != null)
            {
                this.currentEntitiesSelected.Add(entity);
                selectable.SetSelected(true);
                var draggable = entity.GetComponent<GameEntity>().GetDraggable();
                if (draggable != null)
                {
                    draggable.preDragPosition = draggable.transform.position;
                    draggable.preDragRotation = draggable.transform.rotation;
                }
            }
        }
    }

    private List<GameObject> GetCurrentSelectedDraggables()
    {
        var draggables = new List<GameObject>();
        foreach (GameObject e in this.GetEntitiesSelected())
        {
            var draggable = e.GetComponent<GameEntity>().GetDraggable();
            if (draggable != null)
            {
                draggables.Add(e);
            }
        }
        return draggables;
    }

    private void TryGroupingDraggableEntities(List<GameObject> entities)
    {
        //
        // groups draggable entities under a single container
        //
        foreach (GameObject e in entities)
        {
            var draggable = e.GetComponent<GameEntity>().GetDraggable();
            if (draggable != null)
            {
                draggable.transform.SetParent(this.entityDragContainer.transform);
            }
        }
    }

    private void UngroupDraggingEntities(List<GameObject> entities)
    {
        foreach (GameObject e in entities)
        {
            e.transform.SetParent(null);
        }
    }


}
