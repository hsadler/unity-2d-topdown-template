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
    private bool isInventoryKeyActive = false;

    private IDictionary<string, string> keycodeToHumanReadable = new Dictionary<string, string>()
    {
        {KeyCode.Alpha1.ToString(), "1"},
        {KeyCode.Alpha2.ToString(), "2"},
        {KeyCode.Alpha3.ToString(), "3"},
    };

    // child references
    public GameObject inventoryKeyDisplay;

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
            this.inventoryKeyDisplay.GetComponent<TextMeshProUGUI>().text = this.keycodeToHumanReadable[this.keyCode.ToString()];
        }
        else
        {
            this.inventoryKeyDisplay.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    void Update()
    {
        this.HandleInventoryKeyPress();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleInventoryItemClick();
    }

    // INTF METHODS

    public void DeactivateInventoryKey()
    {
        this.isInventoryKeyActive = false;
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

    private void HandleInventoryKeyPress()
    {
        // key match
        if (this.keyCode != KeyCode.None && Input.GetKeyDown(this.keyCode))
        {
            bool isToggleCurrentSelectedOff = this.playerInputManager.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT && this.isInventoryKeyActive;
            // already a selected an inventory key
            if (this.playerInputManager.inputMode == GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT)
            {
                this.playerInputManager.DeleteSelectedEntities();
                this.playerInputManager.ClearInventoryMultiPlacementEntity();
            }
            // fresh selected inventory key
            else
            {
                this.playerInputManager.InitEntitySelect();
            }
            // init state
            this.playerInputManager.inputMode = GameSettings.INPUT_MODE_DEFAULT;
            foreach (var inventoryItemScript in this.playerInputManager.inventoryItemScripts)
            {
                inventoryItemScript.isInventoryKeyActive = false;
            }
            // action is toggle-off, so noop
            if (isToggleCurrentSelectedOff)
            {
                return;
            }
            // toggle on
            else
            {
                this.playerInputManager.SetInventoryMultiPlacementPrefab(this.prefab);
                GameObject spawned = this.playerInputManager.CreateInventoryMultiPlacementEntity(this.playerInputManager.inventoryMultiPlacementMemRotation);
                this.playerInputManager.SelectSingleEntity(spawned);
                this.playerInputManager.inputMode = GameSettings.INPUT_MODE_INVENTORY_MULTIPLACEMENT;
                this.isInventoryKeyActive = true;
            }
        }
    }


}
