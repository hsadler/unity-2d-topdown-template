using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityManager : MonoBehaviour
{


    // Manages Game Entity positions within the play area. Assumes that multiple 
    // Game Entities cannot occupy the same discrete position. Also manages 
    // entity state history for undo/redo functionality. Also manages 
    // game-entity auto-behavior


    // position state management
    private IDictionary<string, GameObject> positionToGameEntity;
    private IDictionary<string, string> gameEntityUUIDToSerializedPosition;

    // entity state history management
    private HistoryStack<List<GameEntityState>> entityStateHistoryStack;

    // debug
    private bool useLogging = false;
    private bool useDebugIndicators = false;
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
        PlaySceneManager.instance.tickManager.timeTickEvent.AddListener(this.ExecuteAutoBehaviorActions);
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

    public bool AddGameEntity(GameObject gameEntity, Vector3 position)
    {
        position = Functions.RoundVector(position);
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;
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
            if (this.useLogging)
            {
                Debug.Log("Adding game entity " + gameEntity.name + " at position " + position.ToString());
            }
            string sPos = position.ToString();
            this.positionToGameEntity[sPos] = gameEntity;
            this.gameEntityUUIDToSerializedPosition[eUUID] = sPos;
            if (GameSettings.DISPLAY_UI_DEBUG || this.useDebugIndicators)
            {
                GameObject occupiedIndicatior = Instantiate(this.occupiedIndicatorPrefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                this.positionToOccupiedIndicator[sPos] = occupiedIndicatior;
            }
            return true;
        }
        if (this.useLogging)
        {
            Debug.Log("Could NOT add game entity " + gameEntity.name + " at occupied position " + position.ToString());
        }
        return false;
    }

    public bool RemoveGameEntity(GameObject gameEntity)
    {
        string keyToRemove = null;
        string eUUID = gameEntity.GetComponent<GameEntity>().uuid;
        Vector3 position = gameEntity.transform.position;
        // check that game entity transform position and tracked position match
        string currPos = this.GetSerializedGameEntityPosition(gameEntity);
        string sPos = position.ToString();
        // try to remove at game object's current position
        if (sPos == currPos && this.PositionIsOccupied(position))
        {
            GameObject gEntityAtPosition = this.positionToGameEntity[sPos];
            if (gEntityAtPosition == gameEntity)
            {
                keyToRemove = sPos;
            }
        }
        // as a fallback, try to remove by gameobject equality
        if (keyToRemove == null)
        {
            foreach (KeyValuePair<string, GameObject> item in this.positionToGameEntity)
            {
                if (item.Value == gameEntity)
                {
                    keyToRemove = item.Key;
                }
            }
        }
        if (keyToRemove != null)
        {
            if (this.useLogging)
            {
                Debug.Log("Removing game entity " + gameEntity.name + " at position " + keyToRemove);
            }
            this.positionToGameEntity.Remove(keyToRemove);
            this.gameEntityUUIDToSerializedPosition.Remove(eUUID);
            if (GameSettings.DISPLAY_UI_DEBUG || this.useDebugIndicators)
            {
                GameObject occupiedIndicator = this.positionToOccupiedIndicator[keyToRemove];
                this.positionToOccupiedIndicator.Remove(keyToRemove);
                Destroy(occupiedIndicator);
            }
            return true;
        }
        Debug.LogError("Could NOT remove game entity " + gameEntity.name + " from GameEntityManager");
        return false;
    }

    public void TryPushEntityStateHistoryStep()
    {
        var currentHistoryStep = this.entityStateHistoryStack.GetCurrent();
        var newHistoryStep = new List<GameEntityState>();
        foreach (GameObject entity in this.positionToGameEntity.Values)
        {
            var geScript = entity.GetComponent<GameEntity>();
            var entityState = new GameEntityState(
                uuid: geScript.uuid,
                prefabName: geScript.prefabName,
                position: entity.transform.position,
                rotation: entity.transform.rotation
            );
            newHistoryStep.Add(entityState);
        }
        if (this.IsDiffHistorySteps(newHistoryStep, currentHistoryStep))
        {
            this.entityStateHistoryStack.Push(newHistoryStep);
            if (this.useLogging)
            {
                Debug.Log("pushed entity state history step with length: " + newHistoryStep.Count.ToString());
            }
        }
        else
        {
            if (this.useLogging)
            {
                Debug.Log("NOT pushing entity state history step since no diff detected or there is no history");
            }
        }
    }

    public bool GoStateHistoryStep(string direction)
    {
        if (this.useLogging)
        {
            Debug.Log("doing undo/redo by applying state in direction: " + direction);
        }
        List<GameEntityState> states = direction == "back" ? this.entityStateHistoryStack.Previous() : this.entityStateHistoryStack.Next();
        if (states != null)
        {
            var entities = new List<GameObject>(this.positionToGameEntity.Values);
            // 1. initialize all to non-active for clean slate
            foreach (GameObject e in entities)
            {
                e.SetActive(false);
            }

            // 2. gather game entity game objects to be processed and remove from board tracking
            var uuidToGameEntity = new Dictionary<string, GameObject>();
            foreach (GameEntityState s in states)
            {
                GameObject gameEntity = this.GetEntityByInstanceUUID(s.uuid);
                if (gameEntity != null)
                {
                    uuidToGameEntity.Add(s.uuid, gameEntity);
                    this.RemoveGameEntity(gameEntity);
                }
            }

            // 3. set entities to previous states and add to board tracking
            foreach (GameEntityState s in states)
            {
                if (this.useLogging)
                {
                    Debug.Log("Procesing game-entity-state with UUID: " + s.uuid);
                }
                if (uuidToGameEntity.ContainsKey(s.uuid))
                {
                    GameObject gameEntity = uuidToGameEntity[s.uuid];
                    if (this.useLogging)
                    {
                        Debug.Log("Restoring game entity: " + gameEntity.name + " from state with position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    }
                    gameEntity.SetActive(true);
                    gameEntity.transform.position = s.position;
                    gameEntity.transform.rotation = s.rotation;
                    this.AddGameEntity(gameEntity, gameEntity.transform.position);
                }
                else
                {
                    GameObject prefab = PlaySceneManager.instance.playerInventoryManager.GetInventoryPrefabByName(s.prefabName);
                    GameObject spawned = Instantiate(prefab, s.position, s.rotation);
                    if (this.useLogging)
                    {
                        Debug.Log("Instantiated game entity: " + spawned.name + " from state with position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    }
                    spawned.GetComponent<GameEntity>().SetUUID(s.uuid);
                    this.AddGameEntity(spawned, spawned.transform.position);
                }
            }
            // 4. any game entities with a remaining non-active state are de-registered and destroyed
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
            this.AddGameEntity(e, e.transform.position);
        }
        this.TryPushEntityStateHistoryStep();
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

    private bool IsDiffHistorySteps(List<GameEntityState> historyStep1, List<GameEntityState> historyStep2)
    {
        return Diff(historyStep1, historyStep2) || Diff(historyStep2, historyStep1);
        bool Diff(List<GameEntityState> historyStep1, List<GameEntityState> historyStep2)
        {
            if (historyStep1 == null || historyStep2 == null)
            {
                if (this.useLogging)
                {
                    Debug.Log("One of the diffed history steps is null");
                }
                return true;
            }
            Dictionary<string, GameEntityState> uuidToGameEntityState = new Dictionary<string, GameEntityState>();
            foreach (var s in historyStep1)
            {
                uuidToGameEntityState.Add(s.uuid, s);
            }
            foreach (var s2 in historyStep2)
            {
                if (uuidToGameEntityState.ContainsKey(s2.uuid))
                {
                    GameEntityState s1 = uuidToGameEntityState[s2.uuid];
                    if (
                        s1.prefabName == s2.prefabName &&
                        s1.position == s2.position &&
                        s1.rotation == s2.rotation
                    )
                    {
                        // pass
                    }
                    else
                    {
                        if (this.useLogging)
                        {
                            Debug.Log("A history step diff was found");
                        }
                        return true;
                    }
                }
                else
                {
                    if (this.useLogging)
                    {
                        Debug.Log("A history step diff was found");
                    }
                    return true;
                }
            }
            if (this.useLogging)
            {
                Debug.Log("A history step diff was NOT found");
            }
            return false;
        }
    }

    private void ExecuteAutoBehaviorActions(int value)
    {
        // do auto-behaviors
        foreach (GameObject entity in new List<GameObject>(this.positionToGameEntity.Values))
        {
            if (entity != null && entity.TryGetComponent(out IGameEntityAutoBehavior autoBehavior))
            {
                autoBehavior.AutoBehavior();
            }
        }
        // commit movements and rotations
        foreach (GameObject entity in new List<GameObject>(this.positionToGameEntity.Values))
        {
            if (entity != null && entity.TryGetComponent(out Rotatable rotatable))
            {
                rotatable.CommitRotations();
            }
            if (entity != null && entity.TryGetComponent(out Movable movable))
            {
                movable.CommitMovement();
            }
        }
        this.TryPushEntityStateHistoryStep();
    }


}