using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerDownHandler
{

    public KeyCode keyCode;
    public string entityName;

    private GameObject prefab;
    private bool isHotkeyActive = false;

    private IDictionary<string, string> keycodeToHumanReadable = new Dictionary<string, string>()
    {
        {KeyCode.Alpha1.ToString(), "1"},
        {KeyCode.Alpha2.ToString(), "2"},
        {KeyCode.Alpha3.ToString(), "3"},
    };

    // child references
    public GameObject hotkeyDisplay;

    // manager refs
    private PlayerInputManager playerInputManager;


    // UNITY HOOKS

    void Start()
    {
        this.playerInputManager = PlaySceneManager.instance.playerInputManager;
        this.playerInputManager.inventoryItemScripts.Add(this);
        if (this.keyCode != KeyCode.None && this.entityName.Length > 0)
        {
            this.prefab = PlaySceneManager.instance.playerInventoryManager.GetInventoryPrefabByName(this.entityName);
            this.hotkeyDisplay.GetComponent<TextMeshProUGUI>().text = this.keycodeToHumanReadable[this.keyCode.ToString()];
        }
        else
        {
            this.hotkeyDisplay.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    void Update()
    {
        this.HandleInventoryHotkey();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleInventoryItemClick();
    }

    // INTF METHODS

    // IMPL METHODS

    private void HandleInventoryItemClick()
    {
        this.playerInputManager.InitEntitySelect();
        Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        pos.z = 0;
        GameObject spawned = this.CreateInventoryEntity(pos);
        if (spawned != null)
        {
            this.playerInputManager.SelectSingleEntity(spawned);
        }
    }

    private void HandleInventoryHotkey()
    {
        // key match
        if (this.keyCode != KeyCode.None && Input.GetKeyDown(this.keyCode))
        {
            // set all other inventory scripts hotkey-active as off
            foreach (var inventoryItemScript in this.playerInputManager.inventoryItemScripts)
            {
                if (inventoryItemScript != this)
                {
                    inventoryItemScript.isHotkeyActive = false;
                }
            }
            // same key press toggle off
            if (this.playerInputManager.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY && this.isHotkeyActive)
            {
                this.playerInputManager.ClearInventoryHotkeyEntity();
                this.playerInputManager.inputMode = GameSettings.INPUT_MODE_DEFAULT;
                this.isHotkeyActive = false;
            }
            else
            {
                // switch from one inventory hotkey to another
                if (this.playerInputManager.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
                {
                    this.playerInputManager.ClearInventoryHotkeyEntity();
                }
                // toggle on
                this.playerInputManager.SetInventoryHotkeyPrefab(this.prefab);
                this.playerInputManager.CreateInventoryHotkeyEntity(this.playerInputManager.inventoryHotkeyMemRotation);
                this.playerInputManager.inputMode = GameSettings.INPUT_MODE_INVENTORY_HOTKEY;
                this.isHotkeyActive = true;
            }
        }
    }

    private GameObject CreateInventoryEntity(Vector3 pos, bool isNewlyCreated = true, bool isSelected = true)
    {
        if (this.prefab)
        {
            GameObject spawned = Instantiate(this.prefab, pos, Quaternion.identity);
            spawned.GetComponent<GameEntity>().isNewlyCreated = isNewlyCreated;
            spawned.GetComponent<Selectable>().SetSelected(isSelected);
            return spawned;
        }
        return null;
    }


}
