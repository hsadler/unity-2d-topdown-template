using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityGridLayer
{


    // position state management
    private IDictionary<string, GameObject> positionToGameEntity;
    private IDictionary<string, string> gameEntityUUIDToSerializedPosition;

    // debug
    private readonly bool useLogging = false;


    public GameEntityGridLayer()
    {
        this.positionToGameEntity = new Dictionary<string, GameObject>();
        this.gameEntityUUIDToSerializedPosition = new Dictionary<string, string>();
    }

    // INTF METHODS

    public List<GameObject> GetAllGameEntities()
    {
        return new List<GameObject>(this.positionToGameEntity.Values);
    }

    public bool PositionIsOccupied(Vector3 position)
    {
        return this.positionToGameEntity.ContainsKey(position.ToString());
    }

    public bool PositionIsValidAndFree(Vector3 position)
    {
        return PlaySceneManager.instance.proceduralEnvironmentManager.IsPositionValid(position)
            && !this.positionToGameEntity.ContainsKey(position.ToString());
    }

    public GameObject GetGameEntityAtPosition(Vector3 position)
    {
        string sPos = position.ToString();
        if (this.positionToGameEntity.ContainsKey(sPos))
        {
            return this.positionToGameEntity[sPos];
        }
        return null;
    }

    public bool AddGameEntity(GameObject gameEntity, Vector3 position)
    {
        position = Functions.QuantizeVector(position);
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;
        // check for game entity already being in the place space
        if (this.gameEntityUUIDToSerializedPosition.ContainsKey(eUUID))
        {
            Debug.LogWarning("Game entity: " + gameEntity.name + " already exists in the gameplay space at position: " + this.gameEntityUUIDToSerializedPosition[eUUID].ToString());
            return false;
        }
        // check if requested position is occupied
        if (this.PositionIsValidAndFree(position))
        {
            if (this.useLogging) Debug.Log("Adding game entity " + gameEntity.name + " at position " + position.ToString());
            string sPos = position.ToString();
            this.positionToGameEntity[sPos] = gameEntity;
            this.gameEntityUUIDToSerializedPosition[eUUID] = sPos;
            return true;
        }
        if (this.useLogging) Debug.Log("Could NOT add game entity " + gameEntity.name + " at occupied position " + position.ToString());
        return false;
    }

    public bool RemoveGameEntity(GameObject gameEntity)
    {
        string positionKeyToRemove = null;
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;
        Vector3 position = gameEntity.transform.position;
        // check that game entity transform position and tracked position match
        string currPos = gameEntity.GetComponent<GameEntity>().GetSerializedPosition();
        string sPos = position.ToString();
        // try to remove at game object's current position
        if (sPos == currPos && this.PositionIsOccupied(position))
        {
            GameObject gEntityAtPosition = this.positionToGameEntity[sPos];
            if (gEntityAtPosition == gameEntity)
            {
                positionKeyToRemove = sPos;
            }
        }
        // as a fallback, try to remove by gameobject equality
        if (positionKeyToRemove == null)
        {
            foreach (KeyValuePair<string, GameObject> item in this.positionToGameEntity)
            {
                if (item.Value == gameEntity)
                {
                    positionKeyToRemove = item.Key;
                }
            }
        }
        if (positionKeyToRemove != null)
        {
            if (this.useLogging) Debug.Log("Removing game entity " + gameEntity.name + " at position " + positionKeyToRemove);
            this.positionToGameEntity.Remove(positionKeyToRemove);
            this.gameEntityUUIDToSerializedPosition.Remove(eUUID);
            return true;
        }
        if (this.useLogging) Debug.Log("Could NOT remove game entity " + gameEntity.name + " at position " + positionKeyToRemove + " since it was not found in the GameEntityManager");
        return false;
    }

    public GameObject GetEntityByUUID(string uuid)
    {
        foreach (var e in this.positionToGameEntity.Values)
        {
            if (e.GetComponent<GameEntity>().uuid == uuid)
            {
                return e;
            }
        }
        return null;
    }

    // IMPL METHODS


}