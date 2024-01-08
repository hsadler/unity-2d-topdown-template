using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
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
            Debug.Log("AutoBehavior executed for Duplicator: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.TryDuplicate();
        }
    }

    // IMPL METHODS

    public void TryDuplicate()
    {
        //
        // take a game-entity from the input area and duplicated it to the 
        // output area
        //
        Vector3 inputPos = Functions.QuantizeVector(this.transform.position + transform.up);
        Vector3 outputPos = Functions.QuantizeVector(this.transform.position - transform.up);
        GameObject toDuplicate = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, inputPos);
        GameObject entityAtOutputPos = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, outputPos);
        if (entityAtOutputPos != null)
        {
            if (this.useLogging)
            {
                Debug.Log("Cannot duplicate entity at " + inputPos.ToString() + " to " + outputPos.ToString() + " because output position is occupied");
            }
        }
        if (toDuplicate != null && entityAtOutputPos == null)
        {
            if (this.useLogging)
            {
                Debug.Log("Duplicating entity at " + inputPos.ToString() + " to " + outputPos.ToString());
            }
            GameObject spawnPrefab = PlaySceneManager.instance.gameEntityRepoManager.GetGameEntityPrefabByName(toDuplicate.GetComponent<GameEntity>().prefabName);
            GameObject newEntity = Instantiate(spawnPrefab, outputPos, toDuplicate.transform.rotation);
            PlaySceneManager.instance.gameEntityManager.AddGameEntity(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS, newEntity, newEntity.transform.position);
        }
    }


}
