using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityManager : MonoBehaviour
{


    // Manages Game Entity positions within the play area. Assumes that multiple 
    // Game Entities cannot occupy the same discrete position within the same 
    // layer.
    // Also manages entity state history for undo/redo functionality.
    // Also envokes game-entity auto-behaviors per tick.

    // grid layer management
    private IDictionary<string, GameEntityGridLayer> gridLayerToGameEntityGridLayer;

    // entity state history management
    private HistoryStack<List<GameEntityState>> entityStateHistoryStack;

    // debug
    private readonly bool useLogging = false;
    private readonly bool useDebugIndicators = false;
    public GameObject occupiedIndicatorPrefab;
    private IDictionary<string, GameObject> positionToOccupiedIndicator;


    // UNITY HOOKS

    void Awake() { }

    void Start()
    {
        PlaySceneManager.instance.tickManager.timeTickEvent.AddListener(this.ExecuteAutoBehaviorActions);
    }

    void Update() { }

    void OnDestroy()
    {
        PlaySceneManager.instance.tickManager.timeTickEvent.RemoveListener(this.ExecuteAutoBehaviorActions);
    }

    // INTF METHODS

    public void Initialize()
    {
        this.gridLayerToGameEntityGridLayer = new Dictionary<string, GameEntityGridLayer>();
        this.gridLayerToGameEntityGridLayer[GameSettings.GAME_ENTITY_GRID_LAYER_GROUND] = new GameEntityGridLayer();
        this.gridLayerToGameEntityGridLayer[GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS] = new GameEntityGridLayer();
        this.positionToOccupiedIndicator = new Dictionary<string, GameObject>();
        this.entityStateHistoryStack = new HistoryStack<List<GameEntityState>>(capacity: GameSettings.ENTITY_STATE_MAX_HISTORY);
    }

    public GameObject[] FindAllGameEntitiesInScene()
    {
        return GameObject.FindGameObjectsWithTag("GameEntity");
    }

    public void RegisterInitialGameEntities()
    {
        foreach (GameObject e in this.FindAllGameEntitiesInScene())
        {
            GameEntity geScript = e.GetComponent<GameEntity>();
            this.AddGameEntity(geScript.gridLayer, e, e.transform.position);
        }
        this.TryPushEntityStateHistoryStep();
    }

    public GameEntityGridLayer GetGameEntityGridLayer(string gridLayer)
    {
        return this.gridLayerToGameEntityGridLayer[gridLayer];
    }

    public List<GameObject> GetAllGameEntities()
    {
        List<GameObject> gos = new();
        foreach (GameEntityGridLayer gridLayer in this.gridLayerToGameEntityGridLayer.Values)
        {
            foreach (GameObject entity in gridLayer.GetAllGameEntities())
            {
                gos.Add(entity);
            }
        }
        return gos;
    }

    public List<GameEntityState> GetAllGameEntityStates()
    {
        List<GameEntityState> entityStates = new();
        foreach (GameObject go in this.GetAllGameEntities())
        {
            var geScript = go.GetComponent<GameEntity>();
            var entityState = new GameEntityState(
                uuid: geScript.uuid,
                prefabName: geScript.prefabName,
                gridLayer: geScript.gridLayer,
                position: go.transform.position,
                rotation: go.transform.rotation
            );
            entityStates.Add(entityState);
        }
        return entityStates;
    }

    public GameObject GetGameEntityAtPosition(string gridLayer, Vector3 position)
    {
        return this.gridLayerToGameEntityGridLayer[gridLayer].GetGameEntityAtPosition(position);
    }

    public bool AddGameEntity(string gridLayer, GameObject gameEntity, Vector3 position)
    {
        bool status = this.gridLayerToGameEntityGridLayer[gridLayer].AddGameEntity(gameEntity, position);
        if (status)
        {
            if (GameSettings.DISPLAY_UI_DEBUG || this.useDebugIndicators)
            {
                GameObject occupiedIndicatior = Instantiate(this.occupiedIndicatorPrefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                this.positionToOccupiedIndicator[Functions.QuantizeVector(position).ToString()] = occupiedIndicatior;
            }
            return true;
        }
        return false;
    }

    public bool RemoveGameEntity(string gridLayer, GameObject gameEntity)
    {
        GameEntityGridLayer gameEntityGridLayer = this.gridLayerToGameEntityGridLayer[gridLayer];
        bool status = gameEntityGridLayer.RemoveGameEntity(gameEntity);
        if (status)
        {
            if (GameSettings.DISPLAY_UI_DEBUG || this.useDebugIndicators)
            {
                string positionKeyToRemove = gameEntity.GetComponent<GameEntity>().GetSerializedPosition();
                GameObject occupiedIndicator = this.positionToOccupiedIndicator[positionKeyToRemove];
                this.positionToOccupiedIndicator.Remove(positionKeyToRemove);
                Destroy(occupiedIndicator);
            }
            return true;
        }
        return false;
    }

    public void TryPushEntityStateHistoryStep()
    {
        var currentHistoryStep = this.entityStateHistoryStack.GetCurrent();
        var newHistoryStep = new List<GameEntityState>();
        foreach (GameObject entity in this.GetAllGameEntities())
        {
            var geScript = entity.GetComponent<GameEntity>();
            var entityState = new GameEntityState(
                uuid: geScript.uuid,
                prefabName: geScript.prefabName,
                gridLayer: geScript.gridLayer,
                position: Functions.QuantizeVector(entity.transform.position),
                rotation: Functions.QuantizeQuaternion(entity.transform.rotation)
            );
            newHistoryStep.Add(entityState);
        }
        if (this.IsDiffHistorySteps(newHistoryStep, currentHistoryStep))
        {
            this.entityStateHistoryStack.Push(newHistoryStep);
            if (this.useLogging) Debug.Log("pushed entity state history step with length: " + newHistoryStep.Count.ToString());
        }
        else
        {
            if (this.useLogging) Debug.Log("NOT pushing entity state history step since no diff detected or there is no history");
        }
    }

    public bool GoStateHistoryStep(string direction)
    {
        if (this.useLogging) Debug.Log("doing undo/redo by applying state in direction: " + direction);
        List<GameEntityState> entityStates = direction == "back" ? this.entityStateHistoryStack.Previous() : this.entityStateHistoryStack.Next();
        if (entityStates != null)
        {
            var entities = this.GetAllGameEntities();

            // 1. initialize all to non-active for clean slate
            if (this.useLogging) Debug.Log("Setting all game entities to non-active");
            foreach (GameObject e in entities)
            {
                e.SetActive(false);
            }

            // 2. gather game entity game objects to be processed and remove from board tracking
            if (this.useLogging) Debug.Log("Gathering game entities to be processed and removing from board tracking");
            var uuidToGameEntity = new Dictionary<string, GameObject>();
            foreach (GameEntityState s in entityStates)
            {
                GameEntityGridLayer gameEntityGridLayer = this.gridLayerToGameEntityGridLayer[s.gridLayer];
                GameObject gameEntity = gameEntityGridLayer.GetEntityByUUID(s.uuid);
                if (gameEntity != null)
                {
                    uuidToGameEntity.Add(s.uuid, gameEntity);
                    bool removeStatus = this.RemoveGameEntity(s.gridLayer, gameEntity);
                    if (removeStatus)
                    {
                        if (this.useLogging) Debug.Log("Removed game entity: " + gameEntity.name + " from board tracking with UUID: " + s.uuid + " and position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    }
                    else
                    {
                        if (this.useLogging) Debug.Log("Could not remove game entity: " + gameEntity.name + " from board tracking with UUID: " + s.uuid + " and position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    }
                }
                else
                {
                    if (this.useLogging) Debug.Log("Could not find game entity by instance UUID: " + s.uuid);
                }
            }

            // 3. set entity states to history states and add to board tracking
            if (this.useLogging) Debug.Log("Setting entity states to history states and adding to board tracking");
            foreach (GameEntityState s in entityStates)
            {
                // for existing game entities, restore state
                if (uuidToGameEntity.ContainsKey(s.uuid))
                {
                    GameObject gameEntity = uuidToGameEntity[s.uuid];
                    GameEntity geScript = gameEntity.GetComponent<GameEntity>();
                    if (this.useLogging) Debug.Log("Restoring game entity: " + gameEntity.name + " from state with position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    gameEntity.SetActive(true);
                    var movable = geScript.GetMovable();
                    if (movable != null)
                    {
                        movable.AnimateMovement(movable.transform.position, s.position, GameSettings.FAST_ANIMATION_DURATION);
                    }
                    else
                    {
                        gameEntity.transform.position = s.position;
                        this.AddGameEntity(s.gridLayer, gameEntity, gameEntity.transform.position);
                    }
                    var rotatable = geScript.GetRotatable();
                    if (rotatable != null)
                    {
                        rotatable.AnimateRotation(rotatable.transform.rotation, s.rotation, GameSettings.FAST_ANIMATION_DURATION);
                    }
                    else
                    {
                        gameEntity.transform.rotation = s.rotation;
                    }
                }
                // for game entities that don't exist, instantiate and restore state
                else
                {
                    GameObject prefab = PlaySceneManager.instance.gameEntityRepoManager.GetGameEntityPrefabByName(s.prefabName);
                    GameObject spawned = Instantiate(prefab, s.position, s.rotation);
                    if (this.useLogging) Debug.Log("Instantiated game entity: " + spawned.name + " from state with position: " + s.position.ToString() + " and rotation: " + s.rotation.ToString());
                    spawned.GetComponent<GameEntity>().SetUUID(s.uuid);
                    this.AddGameEntity(s.gridLayer, spawned, spawned.transform.position);
                }
            }
            // 4. any game entities with a remaining non-active state are de-registered and destroyed
            foreach (GameObject e in entities)
            {
                if (e.activeSelf == false)
                {
                    GameEntityGridLayer gameEntityGridLayer = this.gridLayerToGameEntityGridLayer[e.GetComponent<GameEntity>().gridLayer];
                    gameEntityGridLayer.RemoveGameEntity(e);
                    GameObject.Destroy(e);
                }
            }
        }
        return true;
    }

    // IMPL METHODS

    private bool IsDiffHistorySteps(List<GameEntityState> historyStep1, List<GameEntityState> historyStep2)
    {
        return Diff(historyStep1, historyStep2) || Diff(historyStep2, historyStep1);
        bool Diff(List<GameEntityState> historyStep1, List<GameEntityState> historyStep2)
        {
            if (historyStep1 == null || historyStep2 == null)
            {
                if (this.useLogging) Debug.Log("One of the diffed history steps is null");
                return true;
            }
            Dictionary<string, GameEntityState> uuidToGameEntityState = new();
            foreach (var s in historyStep1)
            {
                uuidToGameEntityState.Add(s.uuid, s);
            }
            foreach (var s2 in historyStep2)
            {
                if (uuidToGameEntityState.ContainsKey(s2.uuid))
                {
                    GameEntityState s1 = uuidToGameEntityState[s2.uuid];
                    // if the states are equal, pass
                    if (
                        s1.uuid == s2.uuid &&
                        s1.gridLayer == s2.gridLayer &&
                        s1.prefabName == s2.prefabName &&
                        s1.position == s2.position &&
                        s1.rotation == s2.rotation
                    )
                    {
                        // pass
                    }
                    else
                    {
                        if (this.useLogging) Debug.Log("A history step diff was found");
                        return true;
                    }
                }
                else
                {
                    if (this.useLogging) Debug.Log("A history step diff was found");
                    return true;
                }
            }
            if (this.useLogging) Debug.Log("A history step diff was NOT found");
            return false;
        }
    }

    private void ExecuteAutoBehaviorActions(int value)
    {
        // do auto-behaviors
        List<GameObject> gos = this.GetAllGameEntities();
        foreach (GameObject entity in gos)
        {
            if (entity != null && entity.TryGetComponent(out IGameEntityAutoBehavior autoBehavior))
            {
                autoBehavior.AutoBehavior();
            }
        }
        // commit rotations
        float animationDuration = GameSettings.DEFAULT_TICK_DURATION / 2f;
        foreach (GameObject entity in gos)
        {
            if (entity != null)
            {
                var rotatable = entity.GetComponent<Rotatable>();
                if (rotatable != null)
                {
                    rotatable.CommitRotations(animationDuration);
                }
            }
        }
        // commit movements
        List<Movable> movables = new();
        foreach (GameObject entity in gos)
        {
            if (entity != null)
            {
                var movable = entity.GetComponent<GameEntity>().GetMovable();
                if (movable)
                {
                    movables.Add(movable);
                }
            }
        }
        if (movables.Count > 0)
        {
            // do multiple resolution passes until no more can be done
            int lastMovementCount = -1;
            int resolutionPassCount = 0;
            while (lastMovementCount < 0 || (movables.Count > 0 && movables.Count != lastMovementCount))
            {
                resolutionPassCount += 1;
                List<Movable> recheckMovables = new();
                foreach (Movable movable in movables)
                {
                    bool movementStatus = movable.CommitMovement(animationDuration);
                    if (!movementStatus)
                    {
                        recheckMovables.Add(movable);
                    }
                }
                lastMovementCount = movables.Count;
                movables = recheckMovables;
            }
            if (this.useLogging) Debug.Log("Auto-behavior resolution passes: " + resolutionPassCount.ToString());
        }
        // reset all movables movement forces
        foreach (GameObject entity in gos)
        {
            if (entity != null)
            {
                var movable = entity.GetComponent<GameEntity>().GetMovable();
                if (movable != null)
                {
                    movable.ClearMovementForces();
                }
            }
        }
        this.TryPushEntityStateHistoryStep();
    }


}