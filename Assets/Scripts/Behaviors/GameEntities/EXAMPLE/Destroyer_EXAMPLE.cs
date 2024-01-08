using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
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
            Debug.Log("AutoBehavior executed for Destroyer: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.TryDestroy();
        }
    }

    // IMPL METHODS

    public void TryDestroy()
    {
        //
        // destroy a game-entity within the target area
        //
        Vector3 destroyPos = Functions.QuantizeVector(this.transform.position + this.transform.up);
        GameObject toDestroy = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, destroyPos);
        if (toDestroy != null)
        {
            if (this.useLogging)
            {
                Debug.Log("Destroying entity at position: " + destroyPos.ToString());
            }
            PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, toDestroy);
            Destroy(toDestroy);
        }
    }


}
