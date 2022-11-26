using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    private bool canDestroy = false;


    // UNITY HOOKS

    void Start()
    {

    }

    void Update()
    {
        if (this.canDestroy && Input.GetMouseButtonUp(0))
        {
            PlaySceneManager.instance.playerInputManager.DeleteSelectedEntities();
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        this.canDestroy = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        this.canDestroy = false;
    }

    // INTF METHODS

    // IMPL METHODS


}
