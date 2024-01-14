using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    public LineRenderer beamLineRenderer;
    private readonly int beamLength = 10;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior executed for Conveyor: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.TryPush();
        }
    }

    // IMPL METHODS

    public void TryPush()
    {
        //
        // pushes game-entities within the conveyor's beam
        //
        if (this.useLogging)
        {
            Debug.Log("Trying push for entity: " + this.gameObject.GetInstanceID().ToString());
        }
        for (int i = this.beamLength; i > 0; i--)
        {
            Vector3 beamPos = this.transform.position + (this.transform.up * i);
            if (this.useLogging)
            {
                Debug.Log("Checking position in beam: " + beamPos.ToString());
            }
            GameObject go = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, beamPos);
            if (go != null)
            {
                var movable = go.GetComponent<GameEntity>().GetMovable();
                if (movable != null)
                {

                    if (this.useLogging)
                    {
                        Debug.Log("Found movable game-entity at beam position: " + beamPos.ToString() + ". Trying to move it.");
                    }
                    movable.AddMovementForce(this.transform.up, 1);
                }
            }
        }
    }


}
