using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public List<GameObject> renders;
    public GameObject selectionIndicator;

    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void SetSortingLayer(string sortingLayer)
    {
        Debug.Log("Setting sorting layer to: " + sortingLayer);
        this.selectionIndicator.GetComponent<LineRenderer>().sortingLayerName = sortingLayer;
        foreach (GameObject rend in this.renders)
        {
            rend.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
        }
    }

    // IMPL METHODS


}
