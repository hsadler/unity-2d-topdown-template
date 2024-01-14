using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{


    public GameObject selectionIndicator;

    private Color32 selectedColor = new(255, 255, 255, 255);
    private Color32 pendingDeleteColor = new(0, 0, 0, 255);


    // UNITY HOOKS

    void Awake()
    {
        this.selectionIndicator.SetActive(false);
    }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void SetSelected(bool isSelected)
    {
        this.selectionIndicator.SetActive(isSelected);
        // TODO: delete if not needed
        // GameEntity geScript = this.GetComponent<GameEntity>();
        // string sortingLayer = isSelected ? GameSettings.SORTING_LAYER_ENTITY_SELECTED : geScript.gridLayer;
        // this.GetComponent<GameEntity>().SetRenderersSortingLayer(sortingLayer);
    }

    public void SetPendingDelete(bool status)
    {
        var lr = this.selectionIndicator.GetComponent<LineRenderer>();
        lr.startColor = status ? this.pendingDeleteColor : this.selectedColor;
        lr.endColor = status ? this.pendingDeleteColor : this.selectedColor;
    }

    // IMPL METHODS


}
