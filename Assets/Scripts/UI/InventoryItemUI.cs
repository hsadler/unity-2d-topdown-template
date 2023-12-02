
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IPointerDownHandler
{


    public GameObject entityPrefab;
    public Image entityIcon;
    private GameEntityRepoItem gameEntityRepoItem;

    public GameObject selectionIndicator;
    private bool isSelected;

    // debug
    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.isSelected = false;
        this.selectionIndicator.SetActive(this.isSelected);
        PlaySceneManager.instance.inventoryItemClickedEvent.AddListener(this.SetSelected);
    }

    void Update()
    {
        this.HandleHotbarAssignmentKeyDown();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleInventoryItemClick();
    }

    // INTF METHODS

    public void SetGameEntityRepoItem(GameEntityRepoItem item)
    {
        this.gameEntityRepoItem = item;
        this.entityPrefab = item.prefab;
        this.entityIcon.sprite = item.icon;
    }

    public void HandleInventoryItemClick()
    {
        if (this.useLogging)
        {
            Debug.Log("Inventory item clicked: " + this.entityPrefab.name);
        }
        PlaySceneManager.instance.inventoryItemClickedEvent.Invoke(this.gameEntityRepoItem);
    }

    // IMPL METHODS

    private void SetSelected(GameEntityRepoItem item)
    {
        if (item.prefab == this.entityPrefab && item.icon == this.entityIcon.sprite)
        {
            this.isSelected = !this.isSelected;
        }
        else
        {
            this.isSelected = false;
        }
        this.selectionIndicator.SetActive(this.isSelected);
    }

    private void HandleHotbarAssignmentKeyDown()
    {
        // Check if any numeric key is pressed
        if (this.isSelected && Input.anyKeyDown && this.IsNumericKey())
        {
            if (this.useLogging)
            {
                Debug.Log("Numeric key pressed: " + Input.inputString);
            }
            PlaySceneManager.instance.inventoryItemHotbarAssignmentEvent.Invoke(
                this.gameEntityRepoItem,
                int.Parse(Input.inputString)
            );
        }
    }

    private bool IsNumericKey()
    {
        // Check if the pressed key is a numeric key (0-9)
        for (KeyCode keyCode = KeyCode.Alpha0; keyCode <= KeyCode.Alpha9; keyCode++)
        {
            if (Input.GetKeyDown(keyCode))
            {
                return true;
            }
        }
        return false;
    }


}
