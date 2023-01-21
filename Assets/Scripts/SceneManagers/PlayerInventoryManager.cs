using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{


    // Manages player inventory items.


    public List<GameObject> prefabs;
    // public IDictionary<string, GameObject> nameToPrefab = new Dictionary<string, GameObject>();


    // UNITY HOOKS

    void Start()
    {

    }

    void Update()
    {

    }

    // INTF METHODS

    public GameObject GetInventoryPrefabByName(string name)
    {
        foreach (var p in this.prefabs)
        {
            if (name == p.name)
            {
                return p;
            }
        }
        Debug.LogWarning("Couldn't find inventory prefab by name: " + name);
        return null;
    }

    // IMPL METHODS


}
