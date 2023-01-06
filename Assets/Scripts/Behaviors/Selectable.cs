using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{


    public GameObject selectionIndicator;
    private bool isSelected = false;

    // UNITY HOOKS

    void Awake()
    {
        this.selectionIndicator.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // INTF METHODS

    public void SetSelected(bool isSelected)
    {
        // Debug.Log("setting entity selected state to: " + isSelected.ToString());
        this.isSelected = isSelected;
        this.selectionIndicator.SetActive(isSelected);
        string sortingLayer = isSelected ? GameSettings.SORTING_LAYER_ENTITY_SELECTED : GameSettings.SORTING_LAYER_DEFAULT;
        this.GetComponent<GameEntity>().SetSortingLayer(sortingLayer);
    }

    // IMPL METHODS


}
