using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // TODO: The checks on `playerInputManager.selectionBoxGO` are janky. Refactor.


    private bool canDestroy = false;

    // manager refs
    private PlayerInputManager playerInputManager;


    // UNITY HOOKS

    void Start()
    {
        this.playerInputManager = PlaySceneManager.instance.playerInputManager;
    }

    void Update()
    {
        if (this.canDestroy)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (this.playerInputManager.selectionBoxGO.activeSelf)
                {
                    this.playerInputManager.DoEntitySelectionWithSelectionBox();
                }
                else
                {
                    this.playerInputManager.DeleteSelectedEntities();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (!this.playerInputManager.AreCurrentSelectedEntitiesNewlyCreated())
                {
                    this.playerInputManager.InitEntitySelect();
                }
                this.playerInputManager.DisplayImpendingDeleteForSelectedEntities(status: true);
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        this.canDestroy = true;
        if (Input.GetMouseButton(0) && !this.playerInputManager.selectionBoxGO.activeSelf)
        {
            this.playerInputManager.DisplayImpendingDeleteForSelectedEntities(status: true);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        this.canDestroy = false;
        this.playerInputManager.DisplayImpendingDeleteForSelectedEntities(status: false);
    }

    // INTF METHODS

    // IMPL METHODS


}
