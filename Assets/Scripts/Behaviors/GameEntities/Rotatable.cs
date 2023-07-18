using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{


    private bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void Rotate(float zRotation)
    {
        if (this.useLogging)
        {
            Debug.Log(
                "Rotating game-entity: " + this.gameObject.GetInstanceID().ToString() +
                " with zRotation: " + zRotation.ToString()
            );
        }
        this.transform.Rotate(0, 0, zRotation);
    }

    // IMPL METHODS


}
