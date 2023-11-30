using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryModalUI : MonoBehaviour
{


    public GameObject inventoryItemUIPrefab;
    public GameObject panelContainer;


    // debug
    private readonly bool useLogging = true;


    // UNITY HOOKS

    void Start()
    {
        this.ClearInventoryItems();
        this.PopulateInventoryItems();
    }

    void Update() { }


    // INTF METHODS

    public void HandleInventoryItemClick(GameObject prefab)
    {
        if (this.useLogging)
        {
            Debug.Log("Inventory item clicked: " + prefab.name);
        }
    }

    // IMPL METHODS

    private void PopulateInventoryItems()
    {
        if (this.useLogging)
        {
            Debug.Log("Populating inventory items...");
        }
        foreach (var gameEntityInventoryItem in PlaySceneManager.instance.gameEntityRepoManager.items)
        {
            if (gameEntityInventoryItem.isInventoryAvailable)
            {
                if (this.useLogging)
                {
                    Debug.Log("Adding inventory item: " + gameEntityInventoryItem.prefab.name);
                }
                GameObject inventoryItem = Instantiate(this.inventoryItemUIPrefab);
                inventoryItem.transform.SetParent(this.panelContainer.transform, false);
                inventoryItem.GetComponent<InventoryItemUI>().SetEntityPrefabAndIcon(gameEntityInventoryItem.prefab, gameEntityInventoryItem.icon);
            }
        }
    }

    private void ClearInventoryItems()
    {
        if (this.useLogging)
        {
            Debug.Log("Clearing inventory items...");
        }
        foreach (Transform child in this.panelContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }


}
