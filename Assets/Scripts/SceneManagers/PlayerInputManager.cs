using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{


    // menu
    public GameObject MenuGO;

    // camera
    private float cameraSize;

    // entity mouse interaction
    public GameObject selectionBoxPrefab;
    private GameObject selectionBoxGO;
    private Vector3 initialMultiselectMousePosition;
    private List<GameObject> currentEntitiesSelected = new List<GameObject>();
    private IDictionary<int, Vector3> entityIdToMouseOffset;
    private Vector3 currentMousePosition;
    private bool mouseIsUIHovered;
    private GameObject hoveredEntity;

    // inventory canvas
    public GameObject inventoryCanvas;


    // UNITY HOOKS

    void Start()
    {
        this.MenuGO.SetActive(false);
        this.cameraSize = Camera.main.orthographicSize;
        this.selectionBoxGO = Instantiate(this.selectionBoxPrefab, Vector3.zero, Quaternion.identity);
        this.selectionBoxGO.SetActive(false);
    }

    void Update()
    {
        // player input
        this.CheckMenuOpen();
        // camera
        if (PlaySceneManager.instance.inputMode != GameSettings.INPUT_MODE_MENU)
        {
            this.HandleCameraMovement();
            this.HandleCameraZoom();
            this.HandleMouseEntityInteraction();
            this.HandleEntityRotation();
        }
    }

    // INTERFACE METHODS

    public void InitEntitySelect()
    {
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            e.GetComponent<Selectable>().SetSelected(false);
        }
        this.currentEntitiesSelected = new List<GameObject>();
        this.entityIdToMouseOffset = new Dictionary<int, Vector3>();
    }

    public void SelectSingleEntity(GameObject entity)
    {
        // Debug.Log("Selecting single entity: " + entity.name);
        this.SelectEntities(new List<GameObject>() { entity });
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.entityIdToMouseOffset.Add(entity.GetInstanceID(), entity.transform.position - mousePosition);
    }

    public void DeleteSelectedEntities()
    {
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            Destroy(e);
        }
        this.InitEntitySelect();
    }

    // IMPLEMENTATION METHODS

    private void CheckMenuOpen()
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

    private void HandleMouseEntityInteraction()
    {
        this.mouseIsUIHovered = this.MouseIsUIHovered();
        this.currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] mousePointHits = Physics2D.OverlapPointAll(this.currentMousePosition);
        this.hoveredEntity = this.GetHoveredSelectableEntity(mousePointHits);
        // button down
        if (Input.GetMouseButtonDown(0))
        {
            if (!this.MouseIsUIHovered())
            {
                // entity click
                if (this.hoveredEntity != null)
                {
                    this.HandleEntityClicked();
                }
                // initialize the selection-box
                else
                {
                    this.InitEntitySelect();
                    this.selectionBoxGO.SetActive(true);
                    this.selectionBoxGO.transform.localScale = Vector3.zero;
                    this.initialMultiselectMousePosition = this.currentMousePosition;
                }
            }
        }
        // button held
        else if (Input.GetMouseButton(0))
        {
            // update the position and shape of the selection-box
            if (this.selectionBoxGO.activeSelf)
            {
                Vector3 mPos1 = this.currentMousePosition;
                Vector3 mPos2 = this.initialMultiselectMousePosition;
                float width = Mathf.Abs(mPos1.x - mPos2.x);
                float height = Mathf.Abs(mPos1.y - mPos2.y);
                Vector3 midpoint = (mPos1 - mPos2) / 2;
                this.selectionBoxGO.transform.localScale = new Vector3(width, height, 0);
                Vector3 boxPos = mPos1 - midpoint;
                this.selectionBoxGO.transform.position = new Vector3(boxPos.x, boxPos.y, 0);
            }
            // drag selected entities
            else
            {
                this.HandleEntityDrag();
            }
        }
        // button up
        else if (Input.GetMouseButtonUp(0))
        {
            // box selection
            if (this.selectionBoxGO.activeSelf)
            {
                this.InitEntitySelect();
                this.selectionBoxGO.SetActive(false);
                if (this.currentMousePosition != this.initialMultiselectMousePosition)
                {
                    var entitiesToSelect = new List<GameObject>();
                    Vector3 mPos1 = this.currentMousePosition;
                    Vector3 mPos2 = this.initialMultiselectMousePosition;
                    Collider2D[] selectionBoxHits = Physics2D.OverlapAreaAll(mPos1, mPos2);
                    foreach (Collider2D col in selectionBoxHits)
                    {
                        entitiesToSelect.Add(col.gameObject);
                    }
                    this.SelectEntities(entitiesToSelect);
                }
            }
        }
    }

    private void HandleEntityClicked()
    {
        this.entityIdToMouseOffset = new Dictionary<int, Vector3>();
        // multi entity start drag
        if (hoveredEntity != null && this.currentEntitiesSelected.Count > 0 && this.currentEntitiesSelected.Contains(hoveredEntity))
        {
            // set selected entity initial offsets from mouse position to prepare for entity drag
            foreach (GameObject e in this.currentEntitiesSelected)
            {
                this.entityIdToMouseOffset.Add(e.GetInstanceID(), e.transform.position - this.currentMousePosition);
            }
        }
        // single entity selection
        else if (hoveredEntity != null)
        {
            this.InitEntitySelect();
            this.SelectSingleEntity(hoveredEntity);
        }

    }

    private void HandleEntityDrag()
    {
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            Vector3 offset = this.entityIdToMouseOffset[e.GetInstanceID()];
            e.transform.position = new Vector3(
                this.currentMousePosition.x + offset.x,
                this.currentMousePosition.y + offset.y,
                e.transform.position.z
            );
            if (GameSettings.ENTITY_POSITIONS_DISCRETE)
            {
                e.transform.position = Functions.RoundVector(e.transform.position);
            }
        }
    }

    private void HandleEntityRotation()
    {
        int rot = 0;
        if (Input.GetKeyDown(GameSettings.ROTATE_ITEM_LEFT_KEY))
        {
            rot += 90;
        }
        if (Input.GetKeyDown(GameSettings.ROTATE_ITEM_RIGHT_KEY))
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

    private void SelectEntities(List<GameObject> entities)
    {
        foreach (GameObject entity in entities)
        {
            var selectable = entity.GetComponent<Selectable>();
            if (selectable != null)
            {
                // Debug.Log("Setting entity as selected: " + entity.name);
                this.currentEntitiesSelected.Add(entity);
                selectable.SetSelected(true);
            }
        }
    }


}
