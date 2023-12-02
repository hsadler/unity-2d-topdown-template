using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HotbarItemUI : MonoBehaviour, IPointerDownHandler
{


    public KeyCode keyCode;
    private GameEntityRepoItem gameEntityRepoItem;
    private bool isSelected;

    // child refs
    public GameObject itemIcon;
    public GameObject emptyIcon;
    public GameObject inventoryKeyDisplay;
    public GameObject selectionIndicator;

    // debug
    private readonly bool useLogging = true;


    // UNITY HOOKS

    void Start()
    {

    }

    void Update()
    {
        this.HandleHotbarItemKeyPress();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleHotbarItemClick();
    }

    // INTF METHODS

    public void DeactivateHotbarItem()
    {
        if (this.useLogging)
        {
            Debug.Log("Deactivating inventory key");
        }
    }

    // IMPL METHODS

    private void HandleHotbarItemClick()
    {
        if (this.useLogging)
        {
            Debug.Log("Hotbar item clicked. KeyCode: " + this.keyCode);
        }
    }

    private void HandleHotbarItemKeyPress()
    {
        if (Input.GetKeyDown(this.keyCode))
        {
            if (this.useLogging)
            {
                Debug.Log("Hotbar item key pressed. KeyCode: " + this.keyCode);
            }
            this.isSelected = true;
        }
    }


}
