using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior, IResourceData
{


    public SpriteRenderer bodySpriteRenderer;

    private bool dataIsOn = false;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior executed for ResourceData: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            // noop
        }
    }

    public void ToggleData()
    {
        //
        // toggles data state on/off
        //
        if (this.useLogging)
        {
            Debug.Log("Data toggled for ResourceData: " + this.gameObject.GetInstanceID().ToString());
        }
        this.dataIsOn = !this.dataIsOn;
        if (this.dataIsOn)
        {
            this.bodySpriteRenderer.color = Color.white;
        }
        else
        {
            this.bodySpriteRenderer.color = Color.black;
        }
    }

    // IMPL METHODS


}
