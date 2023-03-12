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
    private IDictionary<string, string> gameEntityIdToSerializedPosition;

    // entity state history management
    private HistoryStack<List<GameEntityState>> entityStateHistoryStack;
    private List<GameEntityState> currentStateHistoryStep;

    // debug
    private bool useLogging = false;
    private bool useDebugIndicators = true;
    public GameObject occupiedIndicatorPrefab;
    private IDictionary<string, GameObject> positionToOccupiedIndicator;


    // UNITY HOOKS

    void Awake()
    {
        this.positionToGameEntity = new Dictionary<string, GameObject>();
        this.gameEntityIdToSerializedPosition = new Dictionary<string, string>();
        this.positionToOccupiedIndicator = new Dictionary<string, GameObject>();
        this.entityStateHistoryStack = new HistoryStack<List<GameEntityState>>(capacity: GameSettings.ENTITY_STATE_MAX_HISTORY);
        this.currentStateHistoryStep = new List<GameEntityState>();
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

    public bool AddGameEntity(GameObject gameEntity, bool trackHistory = false, bool forceLoggingOff = false)
    {
        Vector3 position = gameEntity.transform.position;
        Quaternion rotation = gameEntity.transform.rotation;
        // check for game entity already being in the place space
        if (this.gameEntityIdToSerializedPosition.ContainsKey(gameEntity.GetInstanceID().ToString()))
        {
            Debug.LogWarning(
                "Game entity: " + gameEntity.name +
                " already exists in the gameplay space at position: " +
                this.gameEntityIdToSerializedPosition[gameEntity.GetInstanceID().ToString()].ToString()
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
            this.gameEntityIdToSerializedPosition[gameEntity.GetInstanceID().ToString()] = sPos;
            if (this.useDebugIndicators)
            {
                GameObject occupiedIndicatior = Instantiate(this.occupiedIndicatorPrefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                this.positionToOccupiedIndicator[sPos] = occupiedIndicatior;
            }
            if (trackHistory)
            {
                this.currentStateHistoryStep.Add(new GameEntityState(
                    GameSettings.GAME_ENTITY_STATE_TYPE_CREATE,
                    gameEntity.GetInstanceID(),
                    position,
                    rotation
                ));
            }
            return true;
        }
        if (!forceLoggingOff && this.useLogging)
        {
            Debug.Log("Could NOT add game entity " + gameEntity.name + " at occupied position " + position.ToString());
        }
        return false;
    }

    // TODO: may not need this method
    // public bool UpdateGameEntity(GameObject gameEntity, bool trackHistory = false)
    // {
    //     Vector3 position = gameEntity.transform.position;
    //     Quaternion rotation = gameEntity.transform.rotation;
    //     // do remove
    //     bool removeStatus = this.RemoveGameEntity(gameEntity, trackHistory: false, forceLoggingOff: true);
    //     if (removeStatus)
    //     {
    //         // do add
    //         bool addStatus = this.AddGameEntity(gameEntity, trackHistory: false, forceLoggingOff: true);
    //         if (addStatus)
    //         {
    //             if (this.useLogging)
    //             {
    //                 Debug.Log("Updating game entity " + gameEntity.name + " at position " + position.ToString());
    //             }
    //             if (trackHistory)
    //             {
    //                 this.currentStateHistoryStep.Add(new GameEntityState(
    //                     GameSettings.GAME_ENTITY_STATE_TYPE_UPDATE,
    //                     gameEntity.GetInstanceID(),
    //                     position,
    //                     rotation
    //                 ));
    //             }
    //             return true;
    //         }
    //     }
    //     if (this.useLogging)
    //     {
    //         Debug.Log("Could NOT update game entity " + gameEntity.name + " at position " + position.ToString());
    //     }
    //     return false;
    // }

    public bool RemoveGameEntity(GameObject gameEntity, bool trackHistory = false, bool forceLoggingOff = false)
    {
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
                this.gameEntityIdToSerializedPosition.Remove(gameEntity.GetInstanceID().ToString());
                if (this.useDebugIndicators)
                {
                    GameObject occupiedIndicator = this.positionToOccupiedIndicator[sPos];
                    this.positionToOccupiedIndicator.Remove(sPos);
                    Destroy(occupiedIndicator);
                }
                if (trackHistory)
                {
                    this.currentStateHistoryStep.Add(new GameEntityState(
                        GameSettings.GAME_ENTITY_STATE_TYPE_DELETE,
                        gameEntity.GetInstanceID(),
                        gameEntity.transform.position,
                        gameEntity.transform.rotation
                    ));
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

    public void StartNewEntityStateHistoryStep()
    {
        this.entityStateHistoryStack.Push(this.currentStateHistoryStep);
        this.currentStateHistoryStep = new List<GameEntityState>();
    }

    public bool GoStateHistoryStep(string direction)
    {
        Debug.Log("doing undo/redo by applying state in direction: " + direction);
        if (direction == "back")
        {
            this.StartNewEntityStateHistoryStep();
            List<GameEntityState> prevStates = this.entityStateHistoryStack.Previous();
            Debug.Log(prevStates.ToString());
            foreach (var s in prevStates)
            {
                Debug.Log(s.ToString());
                if (s.stateType == GameSettings.GAME_ENTITY_STATE_TYPE_UPDATE)
                {
                    GameObject gameEntity = this.GetEntityByInstanceId(s.instanceId);
                    if (gameEntity != null)
                    {
                        gameEntity.transform.position = s.position;
                        gameEntity.transform.rotation = s.rotation;
                    }
                }
            }
            return true;
        }
        else if (direction == "forward")
        {
            // TODO: IMPLEMENT FORWARD
            return true;
        }
        return false;
    }

    // IMPL METHODS

    private void RegisterInitialGameEntities()
    {
        var gameEntities = GameObject.FindGameObjectsWithTag("GameEntity");
        foreach (GameObject e in gameEntities)
        {
            this.AddGameEntity(e);
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

    private GameObject GetEntityByInstanceId(int instanceId)
    {
        foreach (var ge in this.positionToGameEntity.Values)
        {
            if (ge.GetInstanceID() == instanceId)
            {
                return ge;
            }
        }
        return null;
    }


}