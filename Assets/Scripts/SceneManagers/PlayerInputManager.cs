using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool isEntityDragging;

    // entity selection
    private GameObject hoveredEntity;
    public GameObject currentEntitiesSelectedContainer;
    private List<GameObject> currentEntitiesSelected = new List<GameObject>();
    private IDictionary<int, Vector3> entityIdToMouseOffset;

    // entity copy + paste
    private List<GameObject> copyPasteEntities = new List<GameObject>();
    private IDictionary<int, Vector3> entityIdToCopyPastePointOffset;

    // inventory multi-placement
    public List<InventoryItem> inventoryItemScripts = new List<InventoryItem>();
    private GameObject inventoryMultiPlacementPrefab;
    public Quaternion inventoryMultiPlacementMemRotation = Quaternion.identity;

    // inventory canvas
    public GameObject inventoryCanvas;


    // UNITY HOOKS

    void Start()
    {
        this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
        this.mouseIsUIHovered = false;
        this.isEntityDragging = false;
        this.MenuGO.SetActive(false);
        this.targetCameraSize = Camera.main.orthographicSize;
        this.targetCameraPositionWorld = Camera.main.transform.position;
        this.selectionBoxGO = Instantiate(this.selectionBoxPrefab, Vector3.zero, Quaternion.identity);
        this.selectionBoxGO.SetActive(false);
        this.entityIdToMouseOffset = new Dictionary<int, Vector3>();
        this.entityIdToCopyPastePointOffset = new Dictionary<int, Vector3>();
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
            this.HandleMouseEntityInteraction();
            this.HandleEntityDeleteByKeyDown();
            // TODO: testing
            this.HandleEntityRotation_NEW();
            this.HandleEntityCopyPaste();
            this.HandleEntityStateUndoRedo();
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
            // TODO: this is for the selection refactor
            e.transform.SetParent(null);

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
            PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(e);
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

    public void SetInventoryMultiPlacementPrefab(GameObject entityPrefab)
    {
        this.inventoryMultiPlacementPrefab = entityPrefab;
    }

    public GameObject CreateInventoryMultiPlacementEntity(Quaternion rotation)
    {
        Vector3 quantizedPosition = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        quantizedPosition.z = 0;
        GameObject spawned = Instantiate(this.inventoryMultiPlacementPrefab, quantizedPosition, rotation);
        if (spawned != null)
        {
            var geScript = spawned.GetComponent<GameEntity>();
            geScript.isNewlyCreated = true;
        }
        else
        {
            Debug.LogWarning("Could not create inventory multi-placement entity at position: " + quantizedPosition.ToString());
        }
        return spawned;
    }

    public void ClearInventoryMultiPlacementEntity()
    {
        this.inventoryMultiPlacementPrefab = null;
        this.inventoryMultiPlacementMemRotation = Quaternion.identity;
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
            // exit inventory multi-placement mode
            if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT)
            {
                this.ClearInventoryMultiPlacementEntity();
                this.DeleteSelectedEntities();
                this.InitEntitySelect();
                this.inputMode = GameSettings.INPUT_MODE_DEFAULT;
                // manually deactivate all inventory item flags
                foreach (GameObject inventoryItem in GameObject.FindGameObjectsWithTag("InventoryItem"))
                {
                    inventoryItem.GetComponent<InventoryItem>().DeactivateInventoryKey();
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
                // entity click and start drag
                if (this.hoveredEntity != null)
                {
                    this.HandleEntityClicked(this.hoveredEntity);
                    // NOTE: this is required in order to make sure the entity 
                    // is in a drag state for the next frame update
                    this.HandleEntityDrag();
                    this.isEntityDragging = true;
                }
                // initialize the selection-box
                else
                {
                    this.HandleStartSelectionBox();
                }
            }
            // inventory multi-placement
            else if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT)
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
                else
                {
                    this.HandleEntityDrag();
                    this.isEntityDragging = true;
                }
            }
            // continue multi-placement of entities
            else if (!this.mouseIsUIHovered && this.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT)
            {
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
                else
                {
                    this.HandleEntityDrop();
                    this.isEntityDragging = false;
                }
            }
            // drop final entity from inventory multi-placement drag
            else if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT)
            {
                this.HandleMultiEntityPlacement();
                PlaySceneManager.instance.gameEntityManager.TryPushEntityStateHistoryStep();
            }
        }
        // mouse move
        else
        {
            if (this.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT)
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
                        PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(e);
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
            PlaySceneManager.instance.gameEntityManager.AddGameEntity(e);
        }
        PlaySceneManager.instance.gameEntityManager.TryPushEntityStateHistoryStep();
    }

    private void HandleEntityRotation()
    {
        // TODO: implement multi rotation handling here
        int rot = 0;
        if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_LEFT_KEY))
        {
            rot += 90;
        }
        if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_RIGHT_KEY))
        {
            rot -= 90;
        }
        if (rot != 0 && this.currentEntitiesSelected.Count > 0)
        {
            foreach (GameObject e in this.currentEntitiesSelected)
            {
                e.transform.Rotate(new Vector3(0, 0, rot));
            }
            // push history step only if input mode is default and entities are not currently being dragged
            if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT && !this.isEntityDragging)
            {
                PlaySceneManager.instance.gameEntityManager.TryPushEntityStateHistoryStep();
            }
        }
    }

    private void HandleEntityRotation_NEW()
    {
        // TODO: implement multi rotation handling here
        if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_LEFT_KEY))
        {
            foreach (var e in this.currentEntitiesSelected)
            {
                PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(e);
            }
            this.currentEntitiesSelectedContainer.transform.Rotate(new Vector3(0, 0, 90));
            foreach (var e in this.currentEntitiesSelected)
            {
                PlaySceneManager.instance.gameEntityManager.AddGameEntity(e);
            }
        }

        // int rot = 0;
        // if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_LEFT_KEY))
        // {
        //     rot += 90;
        // }
        // if (Input.GetKeyDown(GameSettings.ROTATE_ENTITIES_RIGHT_KEY))
        // {
        //     rot -= 90;
        // }
        // if (rot != 0 && this.currentEntitiesSelected.Count > 0)
        // {
        //     foreach (GameObject e in this.currentEntitiesSelected)
        //     {
        //         e.transform.Rotate(new Vector3(0, 0, rot));
        //     }
        //     // push history step only if input mode is default and entities are not currently being dragged
        //     if (this.inputMode == GameSettings.INPUT_MODE_DEFAULT && !this.isEntityDragging)
        //     {
        //         PlaySceneManager.instance.gameEntityManager.TryPushEntityStateHistoryStep();
        //     }
        // }
    }

    private void HandleEntityDeleteByKeyDown()
    {
        if (this.currentEntitiesSelected.Count > 0 && Input.GetKeyDown(GameSettings.DELETE_ENTITIES_KEY))
        {
            this.DeleteSelectedEntities();
            PlaySceneManager.instance.gameEntityManager.TryPushEntityStateHistoryStep();
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
            PlaySceneManager.instance.gameEntityManager.GoStateHistoryStep(backOrForward);
        }
    }

    private void HandleEntityCopyPaste()
    {
        if (Input.GetKey(GameSettings.CTL_Key) || Input.GetKey(GameSettings.CMD_Key))
        {
            if (Input.GetKeyDown(GameSettings.COPY_Key))
            {
                this.copyPasteEntities = new List<GameObject>(this.currentEntitiesSelected);
                Vector3 midpoint = Functions.VectorMidpoint(this.copyPasteEntities.ConvertAll<Vector3>(x => x.transform.position));
                this.entityIdToCopyPastePointOffset = new Dictionary<int, Vector3>();
                foreach (GameObject e in this.copyPasteEntities)
                {
                    this.entityIdToCopyPastePointOffset.Add(e.GetInstanceID(), e.transform.position - midpoint);
                }
            }
            else if (Input.GetKeyDown(GameSettings.PASTE_Key))
            {
                foreach (GameObject e in this.copyPasteEntities)
                {
                    string prefabName = e.GetComponent<GameEntity>().prefabName;
                    GameObject prefab = PlaySceneManager.instance.playerInventoryManager.GetInventoryPrefabByName(prefabName);
                    Vector3 position = this.currentMousePositionWorld + this.entityIdToCopyPastePointOffset[e.GetInstanceID()];
                    Vector3 quantizedPosition = Functions.RoundVector(position);
                    GameObject spawned = Instantiate(
                        prefab,
                        new Vector3(quantizedPosition.x, quantizedPosition.y, 0),
                        e.transform.rotation
                    );
                    PlaySceneManager.instance.gameEntityManager.AddGameEntity(spawned);
                }
            }
        }
    }

    private void HandleMultiEntityPlacement()
    {
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
                this.inventoryMultiPlacementMemRotation = e.transform.rotation;
                e.GetComponent<Draggable>().SetDragging(false);
                e.GetComponent<GameEntity>().isNewlyCreated = false;
                PlaySceneManager.instance.gameEntityManager.AddGameEntity(e);
            }
            this.InitEntitySelect();
            GameObject spawned = this.CreateInventoryMultiPlacementEntity(this.inventoryMultiPlacementMemRotation);
            this.SelectSingleEntity(spawned);
        }
        else
        {
            // Debug.Log("inventory multi-placement entity drop not valid");
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
                // TODO: this is for the selection refactor
                entity.transform.SetParent(this.currentEntitiesSelectedContainer.transform);
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
