using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    public GameObject spawnPrefab;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior executed for Spawner: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.TrySpawnGameEntity();
        }
    }

    // IMPL METHODS

    public void TrySpawnGameEntity()
    {
        //
        // spawn a new game-entity within the target area
        //
        Vector3 spawnPos = Functions.QuantizeVector(this.transform.position + this.transform.up);
        if (PlaySceneManager.instance.gameEntityManager.GetGameEntityGridLayer(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS).PositionIsValidAndFree(spawnPos))
        {
            if (this.useLogging)
            {
                Debug.Log("Spawning new entity at position: " + spawnPos.ToString());
            }
            GameObject newEntity = Instantiate(this.spawnPrefab, spawnPos, Quaternion.identity);
            PlaySceneManager.instance.gameEntityManager.AddGameEntity(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, newEntity, newEntity.transform.position);
        }
    }


}
