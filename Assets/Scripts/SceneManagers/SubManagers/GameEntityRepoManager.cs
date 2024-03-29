using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityRepoManager : MonoBehaviour
{


    // Manages player inventory items.


    public GameEntityRepoItem[] items;

    // debug
    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public GameObject GetGameEntityPrefabByName(string name)
    {
        if (this.useLogging)
        {
            Debug.Log("Getting inventory prefab by name: " + name);
        }
        foreach (var item in this.items)
        {
            if (name == item.prefab.name)
            {
                return item.prefab;
            }
        }
        Debug.LogWarning("Couldn't find inventory prefab by name: " + name);
        return null;
    }

    public GameEntityRepoItem GetGameEntityRepoItemByName(string name)
    {
        if (this.useLogging)
        {
            Debug.Log("Getting inventory repo item by name: " + name);
        }
        foreach (var item in this.items)
        {
            if (name == item.prefab.name)
            {
                return item;
            }
        }
        Debug.LogWarning("Couldn't find inventory repo item by name: " + name);
        return null;
    }

    // IMPL METHODS


}
