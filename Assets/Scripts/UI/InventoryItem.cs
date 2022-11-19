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

    public void CreateItemFromInventory()
    {
        // STUB
        Debug.Log("Creating item from inventory");
        Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(
            this.itemPrefabs[this.activeInventoryItem - 1],
            new Vector3(camPos.x, camPos.y, 0),
            Quaternion.identity
        );
    }

    // IMPLEMENTATION METHODS


}
