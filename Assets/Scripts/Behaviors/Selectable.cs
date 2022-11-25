using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{

    public GameObject selectionIndicator;

    private bool isSelected = false;


    // UNITY HOOKS

    void Start()
    {

    }

    void Update()
    {

    }

    // INTERFACE METHODS

    public void Select()
    {
        this.SetSelected(true);
    }

    public void Deselect()
    {
        this.SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        // Debug.Log("setting entity selected state to: " + isSelected.ToString());
        this.isSelected = isSelected;
        this.selectionIndicator.SetActive(isSelected);
    }

    public void ToggleSelected()
    {
        this.SetSelected(!this.isSelected);
    }

    // IMPLEMENTATION METHODS


}
