using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDragContainer : MonoBehaviour
{


    public GameObject debugPositionIndicator;


    void Start()
    {
        debugPositionIndicator.SetActive(GameSettings.DISPLAY_UI_DEBUG);
    }

    void Update() { }


}
