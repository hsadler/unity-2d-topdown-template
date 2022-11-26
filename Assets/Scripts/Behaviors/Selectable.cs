using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{

    public GameObject selectionIndicator;
    public List<GameObject> renders;

    private bool isSelected = false;

    private const string DEFAULT_SORTING_LAYER = "Default";
    private const string ENTITY_SELECTED_LAYER = "EntitySelected";


    // UNITY HOOKS

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
        string sortingLayer = isSelected ? ENTITY_SELECTED_LAYER : DEFAULT_SORTING_LAYER;
        this.selectionIndicator.GetComponent<LineRenderer>().sortingLayerName = sortingLayer;
        foreach (GameObject rend in this.renders)
        {
            rend.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
        }
    }

    public void ToggleSelected()
    {
        this.SetSelected(!this.isSelected);
    }

    // IMPL METHODS


}
