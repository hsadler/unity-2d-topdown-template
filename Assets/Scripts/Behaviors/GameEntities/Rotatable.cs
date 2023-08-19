using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{


    public GameObject renderBody;
    private float rotationForce = 0.0f;
    private Coroutine rotationCoroutine = null;

    private readonly bool useLogging = false;


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
        this.rotationForce += zRotation;
    }

    public void CommitRotations(float animationDuration)
    {
        // short-circuit if no rotations
        if (this.rotationForce == 0.0f)
        {
            return;
        }
        // otherwise, commit next rotation
        if (this.useLogging)
        {
            Debug.Log(
                "Committing rotations to game-entity: " + this.gameObject.GetInstanceID().ToString() +
                " with rotation force: " + this.rotationForce.ToString()
            );
        }
        Quaternion startRotation = this.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + this.rotationForce);
        this.AnimateRotation(startRotation, endRotation, animationDuration);
        this.rotationForce = 0.0f;
    }

    public void AnimateRotation(Quaternion startRotation, Quaternion endRotation, float animationDuration)
    {
        if (this.useLogging)
        {
            Debug.Log("Animating to rotation from: " + startRotation.ToString() + " to: " + endRotation.ToString());
        }
        if (this.rotationCoroutine != null)
        {
            StopCoroutine(this.rotationCoroutine);
        }
        this.transform.rotation = endRotation;
        this.renderBody.transform.rotation = startRotation;
        this.rotationCoroutine = StartCoroutine(Functions.RotateOverTime(
            go: this.renderBody,
            startRotation: startRotation,
            endRotation: endRotation,
            duration: animationDuration
        ));
    }

    // IMPL METHODS


}
