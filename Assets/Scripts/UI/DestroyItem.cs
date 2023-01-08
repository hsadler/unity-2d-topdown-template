using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public bool shouldDisplayImpendingEntityDeleteOnClick = false;

    private bool canDestroy = false;


    // UNITY HOOKS

    void Start()
    {

    }

    void Update()
    {
        if (this.canDestroy)
        {
            if (Input.GetMouseButtonUp(0))
            {
                PlaySceneManager.instance.playerInputManager.DeleteSelectedEntities();
            }
            else if (this.shouldDisplayImpendingEntityDeleteOnClick && Input.GetMouseButtonDown(0))
            {
                Debug.Log("Destroy Item.cs clicked...");
                var pim = PlaySceneManager.instance.playerInputManager;
                pim.DisplayImpendingDeleteForSelectedEntities(status: true);
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        this.canDestroy = true;
        if (Input.GetMouseButton(0))
        {
            PlaySceneManager.instance.playerInputManager.DisplayImpendingDeleteForSelectedEntities(status: true);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        this.canDestroy = false;
        PlaySceneManager.instance.playerInputManager.DisplayImpendingDeleteForSelectedEntities(status: false);
    }

    // INTF METHODS

    // IMPL METHODS


}
