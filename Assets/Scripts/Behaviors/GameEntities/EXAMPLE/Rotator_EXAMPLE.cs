using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    private GameEntityManager gem;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
    }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior executed for Rotator: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.TryRotate();
        }
    }

    // IMPL METHODS

    public void TryRotate()
    {
        //
        // rotates a game-entity within the target area
        //
        Vector3 rotatePos = Functions.QuantizeVector(this.transform.position + this.transform.up);
        GameObject toRotate = this.gem.GetGameEntityAtPosition(rotatePos);
        if (toRotate != null)
        {
            var rotatable = toRotate.GetComponent<GameEntity>().GetRotatable();
            if (rotatable != null)
            {
                if (this.useLogging)
                {
                    Debug.Log("Rotating entity at position: " + rotatePos.ToString());
                }
                rotatable.AddRotation(90);
            }
        }
    }


}
