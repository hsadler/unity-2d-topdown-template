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
    private readonly bool useLogging = true;


    // UNITY HOOKS

    void Start()
    {
        this.isSelected = false;
        this.selectionIndicator.SetActive(this.isSelected);
        this.emptyIcon.SetActive(true);
        this.inventoryKeyDisplay.GetComponent<TextMeshProUGUI>().text = this.keyCodeString;
        PlaySceneManager.instance.inventoryItemHotbarAssignmentEvent.AddListener(this.HandleHotbarAssignment);
    }

    void Update()
    {
        this.HandleHotbarItemKeyPress();
    }

    void OnDestroy()
    {
        PlaySceneManager.instance.inventoryItemHotbarAssignmentEvent.RemoveListener(this.HandleHotbarAssignment);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.HandleHotbarItemClick();
    }

    // INTF METHODS

    public void DeactivateHotbarItem()
    {
        if (this.useLogging)
        {
            Debug.Log("Deactivating inventory key");
        }
    }

    // IMPL METHODS

    private void HandleHotbarAssignment(GameEntityRepoItem gameEntityRepoItem, string keyCode)
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

    // TODO: convert these to use unity events instead to
    private void HandleHotbarItemClick()
    {
        if (this.useLogging)
        {
            Debug.Log("Hotbar item clicked. KeyCode: " + this.keyCodeString);
        }
        this.SetSelected(true);
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
            if (Functions.IsNumericKey(Input.inputString))
            {
                if (this.useLogging)
                {
                    Debug.Log("Hotbar item key pressed. KeyCode: " + this.keyCodeString);
                }
                if (Input.GetKeyDown(this.keyCodeString))
                {
                    this.SetSelected(!this.isSelected);
                }
                else
                {
                    this.SetSelected(false);
                }
            }
        }
    }

    private void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;
        this.selectionIndicator.SetActive(this.isSelected);
        if (this.isSelected)
        {
            PlaySceneManager.instance.playerInputManager.StartMultiPlacement(new List<GameObject>() { this.gameEntityRepoItem.prefab });
        }
        else
        {
            PlaySceneManager.instance.playerInputManager.ExitMultiPlacement();
        }
    }


}
