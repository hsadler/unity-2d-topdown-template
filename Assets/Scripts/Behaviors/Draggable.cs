using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{


    private bool isDragging = false;
    public bool isDragValid = true;
    public GameObject invalidDragIndicator;


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
            // has stopped dragging
            else
            {
                PlaySceneManager.instance.gameEntityManager.AddGameEntityAtPosition(this.transform.position, this.gameObject);
            }
            // sorting layer for renderers
            string sortingLayer = isDragging ? GameSettings.SORTING_LAYER_ENTITY_DRAGGING : GameSettings.SORTING_LAYER_ENTITY_SELECTED;
            this.GetComponent<GameEntity>().SetRenderersSortingLayer(sortingLayer);
            // display based on drag state
            this.SetDraggingDisplay(isDragging);
        }
        this.isDragging = isDragging;
    }

    // IMPL METHODS

    private void CheckDragValidity()
    {
        // check if drag position is valid by checking if another game entity is occupying the same position
        GameObject occupyingGameEntity = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(this.transform.position);
        if (occupyingGameEntity != null && occupyingGameEntity != this.gameObject)
        {
            this.invalidDragIndicator.SetActive(true);
            this.isDragValid = false;
        }
        else
        {
            this.invalidDragIndicator.SetActive(false);
            this.isDragValid = true;
        }
    }

    private void SetDraggingDisplay(bool isDragging)
    {
        float alpha = isDragging ? 0.5f : 1f;
        this.GetComponent<GameEntity>().SetSpriteRenderersOpacity(alpha);
    }


}
