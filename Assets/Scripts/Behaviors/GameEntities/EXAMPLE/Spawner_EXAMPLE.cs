using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    public GameObject spawnPrefab;

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
        if (this.gem.PositionIsFree(spawnPos))
        {
            if (this.useLogging)
            {
                Debug.Log("Spawning new entity at position: " + spawnPos.ToString());
            }
            GameObject newEntity = Instantiate(this.spawnPrefab, spawnPos, Quaternion.identity);
            this.gem.AddGameEntity(newEntity, newEntity.transform.position);
        }
    }


}
