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

    public void DeactivateHotkey()
    {
        this.isHotkeyActive = false;
    }

    // IMPL METHODS

    private void HandleInventoryItemClick()
    {
        this.playerInputManager.InitEntitySelect();
        Vector3 quantizedPosition = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        quantizedPosition.z = 0;
        if (this.prefab)
        {
            GameObject spawned = Instantiate(this.prefab, quantizedPosition, Quaternion.identity);
            if (spawned != null)
            {
                var geScript = spawned.GetComponent<GameEntity>();
                geScript.isNewlyCreated = true;
                spawned.GetComponent<Selectable>().SetSelected(true);
                this.playerInputManager.SelectSingleEntity(spawned);
            }
            else
            {
                Debug.LogWarning("Could not create inventory entity at position: " + quantizedPosition.ToString());
            }
        }
    }

    private void HandleInventoryHotkey()
    {
        // key match
        if (this.keyCode != KeyCode.None && Input.GetKeyDown(this.keyCode))
        {
            bool isToggleCurrentSelectedOff = this.playerInputManager.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY && this.isHotkeyActive;
            // already a selected hotkey condition
            if (this.playerInputManager.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY)
            {
                this.playerInputManager.DeleteSelectedEntities();
                this.playerInputManager.ClearInventoryHotkeyEntity();
            }
            // fresh selected hotkey condition
            else
            {
                this.playerInputManager.InitEntitySelect();
            }
            // init state
            this.playerInputManager.inputMode = GameSettings.INPUT_MODE_DEFAULT;
            foreach (var inventoryItemScript in this.playerInputManager.inventoryItemScripts)
            {
                inventoryItemScript.isHotkeyActive = false;
            }
            // action is toggle-off, so noop
            if (isToggleCurrentSelectedOff)
            {
                return;
            }
            // toggle on
            else
            {
                this.playerInputManager.SetInventoryHotkeyPrefab(this.prefab);
                GameObject spawned = this.playerInputManager.CreateInventoryHotkeyEntity(this.playerInputManager.inventoryHotkeyMemRotation);
                this.playerInputManager.SelectSingleEntity(spawned);
                this.playerInputManager.inputMode = GameSettings.INPUT_MODE_INVENTORY_HOTKEY;
                this.isHotkeyActive = true;
            }
        }
    }


}
