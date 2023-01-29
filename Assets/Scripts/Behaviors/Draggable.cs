using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{


    private bool isDragging = false;
    public GameObject invalidDragIndicator;

    private const float IS_DRAGGING_ALPHA = 0.5f;
    private const float IS_NOT_DRAGGING_ALPHA = 1f;

    public Vector3 preDragPosition;


    // UNITY HOOKS

    void Start()
    {
        this.invalidDragIndicator.SetActive(false);
    }

    void Update()
    {
        if (this.isDragging)
        {
            this.CheckDragValidity();
        }
    }

    // INTF METHODS

    public void SetDragging(bool isDragging)
    {
        // Debug.Log("setting entity dragging state to: " + isDragging.ToString());
        if (this.isDragging != isDragging)
        {
            // has started dragging
            if (isDragging)
            {
                PlaySceneManager.instance.gameEntityManager.RemoveGameEntityAtPosition(this.transform.position, this.gameObject);
            }
            // has been dropped
            else
            {
                PlaySceneManager.instance.gameEntityManager.AddGameEntityAtPosition(this.transform.position, this.gameObject);
                this.CheckDragValidity();
            }
            // sorting layer for renderers
            string sortingLayer = isDragging ? GameSettings.SORTING_LAYER_ENTITY_DRAGGING : GameSettings.SORTING_LAYER_ENTITY_SELECTED;
            this.GetComponent<GameEntity>().SetRenderersSortingLayer(sortingLayer);
            // display based on drag state
            this.SetDraggingDisplay(isDragging);
        }
        this.isDragging = isDragging;
    }

    public bool PositionIsValid()
    {
        // check if drag position is valid by checking if another game entity is occupying the same position
        GameObject occupyingGameEntity = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(this.transform.position);
        return occupyingGameEntity == null || occupyingGameEntity == this.gameObject;
    }

    // IMPL METHODS

    private void CheckDragValidity()
    {
        // position is already occupied
        if (this.PositionIsValid())
        {
            this.invalidDragIndicator.SetActive(false);
        }
        // position is unoccupied
        else
        {
            this.invalidDragIndicator.SetActive(true);
        }
    }

    private void SetDraggingDisplay(bool isDragging)
    {
        float alpha = isDragging ? IS_DRAGGING_ALPHA : IS_NOT_DRAGGING_ALPHA;
        this.GetComponent<GameEntity>().SetSpriteRenderersOpacity(alpha);
    }


}
