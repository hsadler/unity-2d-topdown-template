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

    }

    // INTF METHODS

    public void SetDragging(bool isDragging)
    {
        // Debug.Log("setting entity dragging state to: " + isDragging.ToString());
        this.isDragging = isDragging;
        string sortingLayer = isDragging ? GameSettings.SORTING_LAYER_ENTITY_DRAGGING : GameSettings.SORTING_LAYER_ENTITY_SELECTED;
        this.GetComponent<GameEntity>().SetSortingLayer(sortingLayer);
    }

    // IMPL METHODS

    private void CheckDragValidity()
    {

    }


}
