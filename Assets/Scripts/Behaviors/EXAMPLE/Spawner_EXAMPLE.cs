using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    public GameObject spawnPrefab;

    private GameEntityManager gem;
    private bool useLogging = true;


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
            Debug.Log("AutoBehavior() called for Spawner: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.SpawnGameEntity();
        }
    }

    // IMPL METHODS

    public void SpawnGameEntity()
    {
        //
        // spawn a new game entity next to the spawner position
        //
        Vector3 spawnPos = Functions.RoundVector(this.transform.position + this.transform.up);
        if (!this.gem.PositionIsOccupied(spawnPos))
        {
            if (this.useLogging)
            {
                Debug.Log("Spawning new entity at: " + spawnPos.ToString());
            }
            GameObject newEntity = Instantiate(this.spawnPrefab, spawnPos, Quaternion.identity);
            this.gem.AddGameEntity(newEntity);
        }
    }


}
