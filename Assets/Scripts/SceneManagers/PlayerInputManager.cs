using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{


    // menu
    public GameObject MenuGO;

    // camera
    private float cameraSize;

    // multiselect
    public GameObject selectionBoxPrefab;
    private GameObject selectionBoxGO;
    private Vector3 initialMultiselectMousePosition;
    private List<GameObject> currentEntitiesSelected = new List<GameObject>();


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
            this.HandleEntitySelectionOrMove();
        }
    }

    // INTERFACE METHODS

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

    private void HandleEntitySelectionOrMove()
    {
        if (true) // can select
        {
            // initial mouse button press
            if (Input.GetMouseButtonDown(0))
            {
                this.DeselectAllEntities();
                GameObject hoveredEntity = this.GetHoveredEntity();
                // single entity selection
                if (hoveredEntity != null)
                {
                    this.SelectEntities(new List<GameObject>() { hoveredEntity });
                }
                // activate and initialize the selection-box
                else
                {
                    this.selectionBoxGO.SetActive(true);
                    this.selectionBoxGO.transform.localScale = Vector3.zero;
                    this.initialMultiselectMousePosition = Input.mousePosition;
                }
            }
            // mouse button up
            else if (Input.GetMouseButtonUp(0) && this.selectionBoxGO.activeSelf)
            {
                this.DeselectAllEntities();
                this.selectionBoxGO.SetActive(false);
                // selection box handling
                if (Input.mousePosition != this.initialMultiselectMousePosition)
                {
                    var entitiesToSelect = new List<GameObject>();
                    // detect what entities are within selection box
                    Vector3 mPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 mPos2 = Camera.main.ScreenToWorldPoint(this.initialMultiselectMousePosition);
                    Collider2D[] hits = Physics2D.OverlapAreaAll(mPos1, mPos2);
                    foreach (Collider2D col in hits)
                    {
                        entitiesToSelect.Add(col.gameObject);
                    }
                    this.SelectEntities(entitiesToSelect);
                }
            }
            // mouse button held down
            else if (Input.GetMouseButton(0))
            {
                // update the position and shape of the selection-box
                if (this.selectionBoxGO.activeSelf)
                {
                    Vector3 mPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 mPos2 = Camera.main.ScreenToWorldPoint(this.initialMultiselectMousePosition);
                    float width = Mathf.Abs(mPos1.x - mPos2.x);
                    float height = Mathf.Abs(mPos1.y - mPos2.y);
                    Vector3 midpoint = (mPos1 - mPos2) / 2;
                    this.selectionBoxGO.transform.localScale = new Vector3(width, height, 0);
                    Vector3 boxPos = mPos1 - midpoint;
                    this.selectionBoxGO.transform.position = new Vector3(boxPos.x, boxPos.y, 0);
                }
                else
                {
                    // STUB: move selected entities
                }
            }
        }
    }

    private GameObject GetHoveredEntity()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);
        foreach (Collider2D hit in hits)
        {
            if (hit != null && this.EntityIsSelectable(hit.gameObject))
            {
                return hit.gameObject;
            }
        }
        return null;
    }

    private bool EntityIsSelectable(GameObject entity)
    {
        return entity.GetComponent<Selectable>() != null;
    }

    private void SelectEntities(List<GameObject> entities)
    {
        foreach (GameObject entity in entities)
        {
            var selectable = entity?.GetComponent<Selectable>();
            if (selectable != null)
            {
                this.currentEntitiesSelected.Add(entity);
                selectable.SetSelected(true);
            }
        }
    }

    private void DeselectAllEntities()
    {
        foreach (GameObject e in this.currentEntitiesSelected)
        {
            e.GetComponent<Selectable>().SetSelected(false);
        }
        this.currentEntitiesSelected = new List<GameObject>();
    }


}
