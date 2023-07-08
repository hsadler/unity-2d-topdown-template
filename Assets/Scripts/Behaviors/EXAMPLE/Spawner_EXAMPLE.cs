using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{

    private bool useLogging = true;
    private GameObject spawnPrefab;


    // UNITY HOOKS

    void Start()
    {

    }

    void Update()
    {

    }

    // INTF METHODS

    public void AutoBehavior()
    {
        // TODO: implement stub
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior() called for Spawner: " + this.gameObject.GetInstanceID().ToString());
        }
    }


}
