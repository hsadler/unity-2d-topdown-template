using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        this.PopulateInventoryItems();
    }

    void Update() { }

    void OnDisable()
    {
        PlaySceneManager.instance.inventoryClosedEvent.Invoke();
    }

    // INTF METHODS

    // IMPL METHODS

    private void PopulateInventoryItems()
    {
        if (this.useLogging)
        {
            Debug.Log("Populating inventory items...");
        }
        foreach (GameEntityRepoItem item in PlaySceneManager.instance.gameEntityRepoManager.items)
        {
            if (item.isInventoryAvailable)
            {
                if (this.useLogging)
                {
                    Debug.Log("Adding inventory item: " + item.prefab.name);
                }
                GameObject inventoryItem = Instantiate(this.inventoryItemUIPrefab);
                inventoryItem.transform.SetParent(this.panelContainer.transform, false);
                inventoryItem.GetComponent<InventoryItemUI>().SetGameEntityRepoItem(item);
            }
        }
    }


}
