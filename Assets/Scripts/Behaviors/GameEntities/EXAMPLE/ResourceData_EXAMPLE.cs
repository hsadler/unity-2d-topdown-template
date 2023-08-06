using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior, IResourceData
{


    public SpriteRenderer bodySpriteRenderer;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update()
    {
        this.SetDataOnIndicator();
    }

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

    public int ReadData()
    {
        //
        // returns the 1 or 0 data value
        //
        Vector2 facingDirection = transform.up;
        if (facingDirection == Vector2.up || facingDirection == Vector2.down)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    // IMPL METHODS

    public void SetDataOnIndicator()
    {
        //
        // sets the data on indicator
        //
        this.bodySpriteRenderer.color = this.ReadData() == 1 ? Color.white : Color.black;
    }


}
