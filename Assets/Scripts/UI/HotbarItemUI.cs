using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HotbarItemUI : MonoBehaviour, IPointerDownHandler
{


    public string keyCodeString;
    private GameEntityRepoItem gameEntityRepoItem;
    private bool isSelected;

    // child refs
    public Image itemIcon;
    public GameObject emptyIcon;
    public GameObject inventoryKeyDisplay;
    public GameObject selectionIndicator;

    // debug
    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.isSelected = false;
        this.selectionIndicator.SetActive(this.isSelected);
        this.emptyIcon.SetActive(true);
        this.inventoryKeyDisplay.GetComponent<TextMeshProUGUI>().text = this.keyCodeString;
        PlaySceneManager.instance.inventoryItemHotbarAssignmentEvent.AddListener(this.HandleHotbarAssignmentEvent);
        PlaySceneManager.instance.inventoryOpenEvent.AddListener(this.Deselect);
        PlaySceneManager.instance.hotbarItemSelectedEvent.AddListener(this.HandleHotbarItemSelectedEvent);
    }

    void Update()
    {
        this.HandleHotbarItemKeyPress();
    }

    void OnDestroy()
    {
        PlaySceneManager.instance.inventoryItemHotbarAssignmentEvent.RemoveListener(this.HandleHotbarAssignmentEvent);
        PlaySceneManager.instance.inventoryOpenEvent.RemoveListener(this.Deselect);
        PlaySceneManager.instance.hotbarItemSelectedEvent.RemoveListener(this.HandleHotbarItemSelectedEvent);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleHotbarItemClick();
    }

    // INTF METHODS

    public void DeactivateHotbarItem()
    {
        this.Deselect();
    }

    // IMPL METHODS

    private void HandleHotbarAssignmentEvent(GameEntityRepoItem gameEntityRepoItem, string keyCode)
    {
        if (this.keyCodeString == keyCode)
        {
            if (this.useLogging)
            {
                Debug.Log("Assigning game-entity: " + gameEntityRepoItem.prefab.name + " to hotbar key: " + this.keyCodeString);
            }
            this.gameEntityRepoItem = gameEntityRepoItem;
            this.itemIcon.sprite = gameEntityRepoItem.icon;
            this.emptyIcon.SetActive(false);
        }
        else if (this.gameEntityRepoItem == gameEntityRepoItem)
        {
            if (this.useLogging)
            {
                Debug.Log("Removing game-entity: " + gameEntityRepoItem.prefab.name + " from hotbar key: " + this.keyCodeString);
            }
            this.gameEntityRepoItem = null;
            this.itemIcon.sprite = null;
            this.emptyIcon.SetActive(true);
        }
    }

    private void HandleHotbarItemSelectedEvent(GameEntityRepoItem gameEntityRepoItem)
    {
        if (this.gameEntityRepoItem == gameEntityRepoItem)
        {
            if (PlaySceneManager.instance.playerInputManager.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT)
            {
                PlaySceneManager.instance.playerInputManager.ExitMultiPlacement();
            }
            this.SetSelected(!this.isSelected);
        }
        else
        {
            this.SetSelected(false);
        }
    }

    private void HandleHotbarItemClick()
    {
        if (this.gameEntityRepoItem != null)
        {
            if (this.useLogging)
            {
                Debug.Log("Hotbar item clicked for game-entity: " + this.gameEntityRepoItem.prefab.name);
            }
            PlaySceneManager.instance.hotbarItemSelectedEvent.Invoke(this.gameEntityRepoItem);
        }
    }

    private void HandleHotbarItemKeyPress()
    {
        if (
            this.gameEntityRepoItem != null &&
            (
                PlaySceneManager.instance.playerInputManager.inputMode == GameSettings.INPUT_MODE_DEFAULT ||
                PlaySceneManager.instance.playerInputManager.inputMode == GameSettings.INPUT_MODE_MULTIPLACEMENT
            ) &&
            Input.anyKeyDown
        )
        {
            if (Functions.IsNumericKey(Input.inputString) && Input.inputString == this.keyCodeString)
            {
                if (this.useLogging)
                {
                    Debug.Log("Hotbar item key pressed. KeyCode: " + this.keyCodeString + " for game-entity: " + this.gameEntityRepoItem.prefab.name);
                }
                PlaySceneManager.instance.hotbarItemSelectedEvent.Invoke(this.gameEntityRepoItem);
            }
        }
    }

    private void SetSelected(bool isSelected)
    {
        if (this.useLogging && isSelected)
        {
            Debug.Log("Selecting game-entity: " + this.gameEntityRepoItem.prefab.name + " from hotbar");
        }
        this.isSelected = isSelected;
        this.selectionIndicator.SetActive(this.isSelected);
        if (this.isSelected)
        {
            PlaySceneManager.instance.playerInputManager.StartMultiPlacement(new List<GameObject>() { this.gameEntityRepoItem.prefab });
        }
    }

    private void Deselect()
    {
        this.SetSelected(false);
    }


}
