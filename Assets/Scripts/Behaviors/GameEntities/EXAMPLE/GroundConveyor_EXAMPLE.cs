using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundConveyor_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior executed for Ground Conveyor: " + this.gameObject.GetInstanceID().ToString());
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
        // adds movement force to game-entity on top of the conveyor
        //
        Vector3 startPostion = Functions.QuantizeVector(this.transform.position);
        GameObject toPush = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, startPostion);
        if (toPush != null)
        {
            var movable = toPush.GetComponent<GameEntity>().GetMovable();
            if (movable != null)
            {
                if (this.useLogging)
                {
                    Debug.Log("Found movable component on game-entity to push: " + toPush.gameObject.GetInstanceID().ToString());
                }
                movable.AddMovementForce(this.transform.up, 1);
            }
        }
    }


}
