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

    private bool schedCreateEntity;


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
        if (this.schedCreateEntity)
        {
            this.CreateEntityFromInventory();
            this.schedCreateEntity = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.schedCreateEntity = true;
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void CreateEntityFromInventory()
    {
        PlaySceneManager.instance.playerInputManager.InitEntitySelect();
        if (this.activeInventoryItem > 0)
        {
            Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            pos.z = 0;
            GameObject prefab = this.itemPrefabs[this.activeInventoryItem - 1];
            GameObject spawned = Instantiate(prefab, pos, Quaternion.identity);
            PlaySceneManager.instance.playerInputManager.SelectSingleEntity(spawned);
        }
    }


}
