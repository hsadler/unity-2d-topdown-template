using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{


    private bool useLogging = false;
    private float nextRotation = 0.0f;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void AddRotation(float zRotation)
    {
        if (this.useLogging)
        {
            Debug.Log(
                "Adding rotation to game-entity: " + this.gameObject.GetInstanceID().ToString() +
                " with zRotation: " + zRotation.ToString()
            );
        }
        this.nextRotation += zRotation;
    }

    public void CommitRotations()
    {
        if (this.useLogging)
        {
            Debug.Log(
                "Committing rotations to game-entity: " + this.gameObject.GetInstanceID().ToString() +
                " with nextRotation: " + this.nextRotation.ToString()
            );
        }
        this.transform.Rotate(0, 0, this.nextRotation);
        this.nextRotation = 0.0f;
    }

    // IMPL METHODS


}
