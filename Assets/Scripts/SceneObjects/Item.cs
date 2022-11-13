using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{


    // UNITY HOOKS

    void Start()
    {
        InvokeRepeating("SelectSelf", 1, 1);
    }

    void Update()
    {

    }

    // INTERFACE METHODS

    public void SelectSelf()
    {
        this.GetComponent<Selectable>()?.ToggleSelected();
    }

    // IMPLEMENTATION METHODS


}
