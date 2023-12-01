
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class InventoryItemUI : MonoBehaviour, IPointerDownHandler
{


    public GameObject entityPrefab;
    public Image entityImage;


    // debug settings
    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleInventoryItemClick();
    }

    // INTF METHODS

    public void SetEntityPrefabAndIcon(GameObject prefab, Sprite icon)
    {
        this.entityPrefab = prefab;
        this.entityImage.sprite = icon;
    }

    public void HandleInventoryItemClick()
    {
        if (this.useLogging)
        {
            Debug.Log("Inventory item clicked: " + this.entityPrefab.name);
        }
    }

    // IMPL METHODS


}
