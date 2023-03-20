using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityManager : MonoBehaviour
{


    // Manages Game Entity positions within the play area. Assumes that multiple 
    // Game Entities cannot occupy the same discrete position. Also manages 
    // entity state history for undo/redo functionality.


    // position state management
    private IDictionary<string, GameObject> positionToGameEntity;
    private IDictionary<string, string> gameEntityUUIDToSerializedPosition;

    // entity state history management
    private HistoryStack<List<GameEntityState>> entityStateHistoryStack;

    // debug
    private bool useLogging = false;
    private bool useDebugIndicators = true;
    public GameObject occupiedIndicatorPrefab;
    private IDictionary<string, GameObject> positionToOccupiedIndicator;


    // UNITY HOOKS

    void Awake()
    {
        this.positionToGameEntity = new Dictionary<string, GameObject>();
        this.gameEntityUUIDToSerializedPosition = new Dictionary<string, string>();
        this.positionToOccupiedIndicator = new Dictionary<string, GameObject>();
        this.entityStateHistoryStack = new HistoryStack<List<GameEntityState>>(capacity: GameSettings.ENTITY_STATE_MAX_HISTORY);
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

    public bool AddGameEntity(GameObject gameEntity, bool forceLoggingOff = false)
    {
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;
        Vector3 position = gameEntity.transform.position;
        Quaternion rotation = gameEntity.transform.rotation;
        // check for game entity already being in the place space
        if (this.gameEntityUUIDToSerializedPosition.ContainsKey(eUUID))
        {
            Debug.LogWarning(
                "Game entity: " + gameEntity.name +
                " already exists in the gameplay space at position: " +
                this.gameEntityUUIDToSerializedPosition[eUUID].ToString()
            );
            return false;
        }
        // check if requested position is occupied
        if (!this.PositionIsOccupied(position))
        {
            if (!forceLoggingOff && this.useLogging)
            {
                Debug.Log("Adding game entity " + gameEntity.name + " at position " + position.ToString());
            }
            string sPos = position.ToString();
            this.positionToGameEntity[sPos] = gameEntity;
            this.gameEntityUUIDToSerializedPosition[eUUID] = sPos;
            if (this.useDebugIndicators)
            {
                GameObject occupiedIndicatior = Instantiate(this.occupiedIndicatorPrefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                this.positionToOccupiedIndicator[sPos] = occupiedIndicatior;
            }
            return true;
        }
        if (!forceLoggingOff && this.useLogging)
        {
            Debug.Log("Could NOT add game entity " + gameEntity.name + " at occupied position " + position.ToString());
        }
        return false;
    }

    public bool RemoveGameEntity(GameObject gameEntity, bool forceLoggingOff = false)
    {
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;
        Vector3 position = gameEntity.transform.position;
        Quaternion rotation = gameEntity.transform.rotation;
        // check that game entity transform position and tracked position match
        string currPos = this.GetSerializedGameEntityPosition(gameEntity);
        string sPos = position.ToString();
        if (sPos == currPos && this.PositionIsOccupied(position))
        {
            GameObject gEntityAtPosition = this.positionToGameEntity[sPos];
            if (gEntityAtPosition == gameEntity)
            {
                if (!forceLoggingOff && this.useLogging)
                {
                    Debug.Log("Removing game entity " + gameEntity.name + " at position " + position.ToString());
                }
                this.positionToGameEntity.Remove(sPos);
                this.gameEntityUUIDToSerializedPosition.Remove(eUUID);
                if (this.useDebugIndicators)
                {
                    GameObject occupiedIndicator = this.positionToOccupiedIndicator[sPos];
                    this.positionToOccupiedIndicator.Remove(sPos);
                    Destroy(occupiedIndicator);
                }
                return true;
            }
        }
        if (!forceLoggingOff && this.useLogging)
        {
            Debug.Log("Could NOT remove game entity " + gameEntity.name + " at position " + position.ToString());
        }
        return false;
    }

    public void PushEntityStateHistoryStep()
    {
        var historyStep = new List<GameEntityState>();
        foreach (GameObject entity in this.positionToGameEntity.Values)
        {
            var geScript = entity.GetComponent<GameEntity>();
            var entityState = new GameEntityState(
                uuid: geScript.uuid,
                prefabName: geScript.prefabName,
                position: entity.transform.position,
                rotation: entity.transform.rotation
            );
            historyStep.Add(entityState);
        }
        this.entityStateHistoryStack.Push(historyStep);
        Debug.Log("pushed entity state history step with length: " + historyStep.Count.ToString());
    }

    public bool GoStateHistoryStep(string direction)
    {
        // Debug.Log("doing undo/redo by applying state in direction: " + direction);
        List<GameEntityState> states = direction == "back" ? this.entityStateHistoryStack.Previous() : this.entityStateHistoryStack.Next();
        if (states != null)
        {
            var entities = new List<GameObject>(this.positionToGameEntity.Values);
            // 1. initialize all to non-active for clean slate
            foreach (GameObject e in entities)
            {
                e.SetActive(false);
            }
            // 2. set entities to previous states and reregister
            foreach (GameEntityState s in states)
            {
                GameObject gameEntity = this.GetEntityByInstanceUUID(s.uuid);
                if (gameEntity != null)
                {
                    // Debug.Log("Restoring game entity state at position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    gameEntity.SetActive(true);
                    this.RemoveGameEntity(gameEntity);
                    gameEntity.transform.position = s.position;
                    gameEntity.transform.rotation = s.rotation;
                    this.AddGameEntity(gameEntity);
                }
                else
                {
                    GameObject prefab = PlaySceneManager.instance.playerInventoryManager.GetInventoryPrefabByName(s.prefabName);
                    GameObject spawned = Instantiate(prefab, s.position, s.rotation);
                    spawned.GetComponent<GameEntity>().SetUUID(s.uuid);
                    this.AddGameEntity(spawned);
                }
            }
            // 3. any game entities with a remaining non-active state are 
            // de-registered and destroyed
            foreach (GameObject e in entities)
            {
                if (e.activeSelf == false)
                {
                    this.RemoveGameEntity(e);
                    GameObject.Destroy(e);
                }
            }
        }
        return true;
    }

    // IMPL METHODS

    private void RegisterInitialGameEntities()
    {
        var gameEntities = GameObject.FindGameObjectsWithTag("GameEntity");
        foreach (GameObject e in gameEntities)
        {
            this.AddGameEntity(e);
        }
        this.PushEntityStateHistoryStep();
    }

    private string GetSerializedGameEntityPosition(GameObject gameEntity)
    {
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;

        if (this.gameEntityUUIDToSerializedPosition.ContainsKey(eUUID))
        {
            return this.gameEntityUUIDToSerializedPosition[eUUID];
        }
        return null;
    }

    private GameObject GetEntityByInstanceUUID(string uuid)
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


}