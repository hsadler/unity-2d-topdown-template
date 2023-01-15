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

    // manager refs
    private PlayerInputManager pim;


    // UNITY HOOKS

    void Start()
    {
        this.pim = PlaySceneManager.instance.playerInputManager;
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
        this.HandleInventoryItemClick();
    }

    // INTF METHODS

    // IMPL METHODS

    private void HandleInventoryItemClick()
    {
        this.pim.InitEntitySelect();
        if (this.activeInventoryItem > 0)
        {
            Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            pos.z = 0;
            GameObject spawned = this.CreateInventoryEntity(pos);
            this.pim.SelectSingleEntity(spawned);
        }
    }

    private void CheckInputForInventoryHotkey()
    {
        if (Input.GetKeyDown(this.itemKeyCode))
        {
            if (this.pim.inputMode == GameSettings.INPUT_MODE_HOTKEY_PLACEMENT)
            {
                this.pim.ClearHotKeyPlacementEntity();
            }
            else
            {
                Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                pos.z = 0;
                GameObject spawned = this.CreateInventoryEntity(pos);
                this.pim.SetHotKeyPlacementEntity(spawned);
            }
        }
    }

    private GameObject CreateInventoryEntity(Vector3 pos, bool isNewlyCreated = true, bool isSelected = true)
    {
        GameObject prefab = this.itemPrefabs[this.activeInventoryItem - 1];
        GameObject spawned = Instantiate(prefab, pos, Quaternion.identity);
        spawned.GetComponent<GameEntity>().isNewlyCreated = isNewlyCreated;
        spawned.GetComponent<Selectable>().SetSelected(isSelected);
        return spawned;
    }


}
