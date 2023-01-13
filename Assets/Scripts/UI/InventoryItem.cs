using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[ExecuteAlways]
public class InventoryItem : MonoBehaviour, IPointerDownHandler
{


    public int activeInventoryItem;
    public List<GameObject> inventoryItems;
    public List<GameObject> itemPrefabs;

    public GameObject hotkeyNumberDisplay;

    private IDictionary<int, KeyCode> intToKeyCode = new Dictionary<int, KeyCode>() {
        {1, KeyCode.Alpha1},
        {2, KeyCode.Alpha2},
        {3, KeyCode.Alpha3},
        {4, KeyCode.Alpha4},
        {5, KeyCode.Alpha5},
        {6, KeyCode.Alpha6},
        {7, KeyCode.Alpha7},
        {8, KeyCode.Alpha8},
        {9, KeyCode.Alpha9},
        {0, KeyCode.Alpha0},
    };
    private KeyCode itemKeyCode;


    // UNITY HOOKS

    void Start()
    {
        if (this.activeInventoryItem > 0)
        {
            this.inventoryItems[this.activeInventoryItem - 1].SetActive(true);
            this.hotkeyNumberDisplay.GetComponent<TextMeshProUGUI>().text = this.activeInventoryItem.ToString();
            this.itemKeyCode = this.intToKeyCode[this.activeInventoryItem];
        }
        else
        {
            this.hotkeyNumberDisplay.SetActive(false);
        }
    }

    void Update()
    {
        this.CheckInputForInventoryHotkey();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.CreateEntityFromInventory();
    }

    // INTF METHODS

    // IMPL METHODS

    private void CreateEntityFromInventory()
    {
        PlaySceneManager.instance.playerInputManager.InitEntitySelect();
        if (this.activeInventoryItem > 0)
        {
            Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            pos.z = 0;
            GameObject prefab = this.itemPrefabs[this.activeInventoryItem - 1];
            GameObject spawned = Instantiate(prefab, pos, Quaternion.identity);
            spawned.GetComponent<GameEntity>().isNewlyCreated = true;
            spawned.GetComponent<Selectable>().SetSelected(true);
            PlaySceneManager.instance.playerInputManager.SelectSingleEntity(spawned);
        }
    }

    private void CheckInputForInventoryHotkey()
    {
        if (Input.GetKeyDown(this.itemKeyCode))
        {
            Debug.Log("item inventory pressed: " + this.itemKeyCode.ToString());
            // TODO: set player input mode to inventory-placement
        }
    }


}
