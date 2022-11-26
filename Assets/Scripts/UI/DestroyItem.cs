using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyItem : MonoBehaviour, IDropHandler
{


    void Start()
    {

    }

    void Update()
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        // STUB
        Debug.Log("pointer drop!");
    }


}
