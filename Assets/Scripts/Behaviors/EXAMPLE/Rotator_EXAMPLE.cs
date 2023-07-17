using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    private GameEntityManager gem;

    private bool useLogging = false;


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
            Debug.Log("AutoBehavior() called for Rotator: " + this.gameObject.GetInstanceID().ToString());
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
        // rotates a game entity next to the destroyer position
        //
        Vector3 rotatePos = Functions.RoundVector(this.transform.position + this.transform.up);
        GameObject toRotate = this.gem.GetGameEntityAtPosition(rotatePos);
        if (toRotate != null)
        {
            if (this.useLogging)
            {
                Debug.Log("Rotating entity at position: " + rotatePos.ToString());
            }
            toRotate.transform.Rotate(0, 0, 90);
            var resourceData = toRotate.GetComponent<IResourceData>();
            if (resourceData != null)
            {
                if (this.useLogging)
                {
                    Debug.Log("Rotator found resource data. Toggling data.");
                }
                resourceData.ToggleData();
            }
        }
    }


}
