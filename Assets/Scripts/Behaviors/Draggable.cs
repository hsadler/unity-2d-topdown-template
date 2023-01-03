using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{


    private bool isDragging = false;
    private bool isDragValid = true;
    public GameObject invalidDragIndicator;


    // UNITY HOOKS

    void Start()
    {

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
                // TODO: set game entity display to ghost mode
            }
            // has stopped dragging
            else
            {
                PlaySceneManager.instance.gameEntityManager.UpdateGameEntityPosition(this.transform.position, this.gameObject);
            }
            string sortingLayer = isDragging ? GameSettings.SORTING_LAYER_ENTITY_DRAGGING : GameSettings.SORTING_LAYER_ENTITY_SELECTED;
            this.GetComponent<GameEntity>().SetSortingLayer(sortingLayer);
        }
        this.isDragging = isDragging;
    }

    // IMPL METHODS

    private void CheckDragValidity()
    {
        // check if drag position is valid
        if (PlaySceneManager.instance.gameEntityManager.PositionIsOccupied(this.transform.position))
        {
            this.isDragValid = false;
            this.invalidDragIndicator.SetActive(true);
        }
        else
        {
            this.isDragValid = true;
            this.invalidDragIndicator.SetActive(false);
        }
    }


}
