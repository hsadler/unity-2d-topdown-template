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
    private PlayerInputManager pim;


    // UNITY HOOKS

    void Start()
    {
        this.pim = PlaySceneManager.instance.playerInputManager;
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
        if (this.keyCode != KeyCode.None && Input.GetKeyDown(this.keyCode))
        {
            this.HandleInventoryHotkey();
        }
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
        Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        pos.z = 0;
        GameObject spawned = this.CreateInventoryEntity(pos);
        if (spawned != null)
        {
            this.pim.SelectSingleEntity(spawned);
        }
    }

    private void HandleInventoryHotkey()
    {
        if (this.pim.inputMode == GameSettings.INPUT_MODE_INVENTORY_HOTKEY && this.isHotkeyActive)
        {
            this.pim.ClearHotKeyPlacementEntity();
            this.isHotkeyActive = false;
            return;
        }
        this.pim.InitEntitySelect();
        Vector3 pos = Functions.RoundVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        pos.z = 0;
        GameObject spawned = this.CreateInventoryEntity(pos);
        if (spawned != null)
        {
            this.pim.SelectSingleEntity(spawned);
            this.pim.inputMode = GameSettings.INPUT_MODE_INVENTORY_HOTKEY;
            this.isHotkeyActive = true;
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
