using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{


    private bool useLogging = false;
    private Coroutine rotationCoroutine = null;
    private float rotationForce = 0.0f;
    private Quaternion nextRotation = Quaternion.identity;


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

    public void CommitRotations()
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
        this.nextRotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + this.rotationForce);
        this.rotationCoroutine = StartCoroutine(this.RotateOverTime(
            this.gameObject,
            this.nextRotation,
            GameSettings.DEFAULT_TICK_DURATION / 2)
        );
        this.rotationForce = 0.0f;
    }

    public void FastForwardAnimations()
    {
        if (this.rotationCoroutine != null)
        {
            StopCoroutine(this.rotationCoroutine);
            this.rotationCoroutine = null;
            this.transform.rotation = this.nextRotation;
        }
    }

    // IMPL METHODS

    IEnumerator RotateOverTime(GameObject go, Quaternion endRotation, float duration)
    {
        // from chatGPT
        Quaternion startRotation = go.transform.rotation;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            go.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        go.transform.rotation = endRotation;
    }


}
