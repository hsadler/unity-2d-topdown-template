using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


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
        GameObject toDuplicate = this.gem.GetGameEntityAtPosition(inputPos);
        GameObject entityAtOutputPos = this.gem.GetGameEntityAtPosition(outputPos);
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
            this.gem.AddGameEntity(newEntity, newEntity.transform.position);
        }
    }


}
