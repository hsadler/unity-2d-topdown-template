using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InventoryItem : MonoBehaviour
{


    public int activeInventoryItem;
    public List<GameObject> inventoryItems;
    public List<GameObject> itemPrefabs;


    // UNITY HOOKS

    void Start()
    {
        if (this.activeInventoryItem > 0)
        {
            this.inventoryItems[this.activeInventoryItem - 1].SetActive(true);
        }
    }

    void Update()
    {

    }

    // INTERFACE METHODS

    public GameObject CreateEntityFromInventory()
    {
        GameObject prefab = this.itemPrefabs[this.activeInventoryItem - 1];
        Debug.Log("Creating entity from inventory: " + prefab.name);
        return Instantiate(
            prefab,
            new Vector3(this.transform.position.x, this.transform.position.y, 0),
            Quaternion.identity
        );
    }

    // IMPLEMENTATION METHODS


}
