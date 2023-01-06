using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityManager : MonoBehaviour
{


    // Manages Game Entity positions within the play area. Assumes that multiple 
    // Game Entities cannot occupy the same discrete position.


    public IDictionary<string, GameObject> positionToGameEntity;
    public IDictionary<string, string> gameEntityIdToSerializedPosition = new Dictionary<string, string>();


    // UNITY HOOKS

    void Awake()
    {
        this.positionToGameEntity = new Dictionary<string, GameObject>();
        this.gameEntityIdToSerializedPosition = new Dictionary<string, string>();
    }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public bool PositionIsOccupied(Vector3 position)
    {
        return this.positionToGameEntity.ContainsKey(position.ToString());
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

    public bool AddGameEntityAtPosition(Vector3 position, GameObject gameEntity)
    {
        // Debug.Log("Adding game entity " + gameEntity.name + " at position " + position.ToString());
        if (!this.PositionIsOccupied(position))
        {
            string sPos = position.ToString();
            this.positionToGameEntity[sPos] = gameEntity;
            this.gameEntityIdToSerializedPosition[gameEntity.GetInstanceID().ToString()] = sPos;
            return true;
        }
        return false;
    }

    // public bool UpdateGameEntityPosition(Vector3 newPosition, GameObject gameEntity)
    // {
    //     Debug.Log("Updating game entity " + gameEntity.name + " to position " + newPosition.ToString());
    //     string currPos = this.GetSerializedGameEntityPosition(gameEntity);
    //     if (currPos == null)
    //     {
    //         return false;
    //     }
    //     bool addStatus = this.AddGameEntityAtPosition(newPosition, gameEntity);
    //     if (addStatus)
    //     {
    //         this.positionToGameEntity.Remove(currPos);
    //         return true;
    //     }
    //     return false;
    // }

    public bool RemoveGameEntityAtPosition(Vector3 position, GameObject gameEntity)
    {
        // Debug.Log("Removing game entity " + gameEntity.name + " at position " + position.ToString());
        string currPos = this.GetSerializedGameEntityPosition(gameEntity);
        if (position.ToString() == currPos && this.PositionIsOccupied(position))
        {
            string sPos = position.ToString();
            GameObject gEntityAtPosition = this.positionToGameEntity[sPos];
            if (gEntityAtPosition == gameEntity)
            {
                this.positionToGameEntity.Remove(sPos);
                this.gameEntityIdToSerializedPosition.Remove(gameEntity.GetInstanceID().ToString());
                return true;
            }
        }
        return false;
    }

    // IMPL METHODS

    private string GetSerializedGameEntityPosition(GameObject gameEntity)
    {
        if (this.gameEntityIdToSerializedPosition.ContainsKey(gameEntity.GetInstanceID().ToString()))
        {
            return this.gameEntityIdToSerializedPosition[gameEntity.GetInstanceID().ToString()];
        }
        return null;
    }


}