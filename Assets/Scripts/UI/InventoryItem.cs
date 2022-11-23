using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class InventoryItem : MonoBehaviour, IPointerDownHandler
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

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        pos.z = 0;
        GameObject spawned = this.CreateEntityFromInventory(pos);
        PlaySceneManager.instance.playerInputManager.SelectSingleEntity(spawned);
    }

    // INTERFACE METHODS

    public GameObject CreateEntityFromInventory(Vector3 position)
    {
        GameObject prefab = this.itemPrefabs[this.activeInventoryItem - 1];
        return Instantiate(prefab, position, Quaternion.identity);
    }

    // IMPLEMENTATION METHODS


}
