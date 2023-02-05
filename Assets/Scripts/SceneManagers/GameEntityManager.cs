using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityManager : MonoBehaviour
{


    // Manages Game Entity positions within the play area. Assumes that multiple 
    // Game Entities cannot occupy the same discrete position.


    private IDictionary<string, GameObject> positionToGameEntity;
    private IDictionary<string, string> gameEntityIdToSerializedPosition = new Dictionary<string, string>();

    private bool useLogging = true;


    // UNITY HOOKS

    void Awake()
    {
        this.positionToGameEntity = new Dictionary<string, GameObject>();
        this.gameEntityIdToSerializedPosition = new Dictionary<string, string>();
    }

    void Start()
    {
        this.RegisterInitialGameEntities();
    }

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
        if (!this.PositionIsOccupied(position))
        {
            if (this.useLogging)
            {
                Debug.Log("Adding game entity " + gameEntity.name + " at position " + position.ToString());
            }
            string sPos = position.ToString();
            this.positionToGameEntity[sPos] = gameEntity;
            this.gameEntityIdToSerializedPosition[gameEntity.GetInstanceID().ToString()] = sPos;
            return true;
        }
        if (this.useLogging)
        {
            Debug.Log("Could NOT add game entity " + gameEntity.name + " at position " + position.ToString());
        }
        return false;
    }

    public bool RemoveGameEntityAtPosition(Vector3 position, GameObject gameEntity)
    {
        string currPos = this.GetSerializedGameEntityPosition(gameEntity);
        string sPos = position.ToString();
        if (sPos == currPos && this.PositionIsOccupied(position))
        {
            GameObject gEntityAtPosition = this.positionToGameEntity[sPos];
            if (gEntityAtPosition == gameEntity)
            {
                if (this.useLogging)
                {
                    Debug.Log("Removing game entity " + gameEntity.name + " at position " + position.ToString());
                }
                this.positionToGameEntity.Remove(sPos);
                this.gameEntityIdToSerializedPosition.Remove(gameEntity.GetInstanceID().ToString());
                return true;
            }
        }
        if (this.useLogging)
        {
            Debug.Log("Could NOT remove game entity " + gameEntity.name + " at position " + position.ToString());
        }
        return false;
    }

    // IMPL METHODS

    private void RegisterInitialGameEntities()
    {
        var gameEntities = GameObject.FindGameObjectsWithTag("GameEntity");
        foreach (GameObject e in gameEntities)
        {
            this.AddGameEntityAtPosition(e.transform.position, e);
        }
    }

    private string GetSerializedGameEntityPosition(GameObject gameEntity)
    {
        if (this.gameEntityIdToSerializedPosition.ContainsKey(gameEntity.GetInstanceID().ToString()))
        {
            return this.gameEntityIdToSerializedPosition[gameEntity.GetInstanceID().ToString()];
        }
        return null;
    }


}