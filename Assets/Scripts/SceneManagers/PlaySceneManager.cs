using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{

    // The main gameplay singleton manager. Holds references to sub-managers.


    // MonoBehaviour manager components
    public GameSaveLoadManager gameSaveLoadManager;
    public ProceduralEnvironmentManager proceduralEnvironmentManager;
    public GameEntityRepoManager gameEntityRepoManager;
    public GameEntityManager gameEntityManager;
    public PlayerInputManager playerInputManager;
    public TickManager tickManager;
    public UIControlsInstructionsManager uiControlsInstructionsManager;
    public UITelemetryManager uiTelemetryManager;

    // events
    public InventoryOpenEvent inventoryOpenEvent;
    public InventoryClosedEvent inventoryClosedEvent;
    public InventoryItemClickedEvent inventoryItemClickedEvent;
    public InventoryItemHotbarAssignmentEvent inventoryItemHotbarAssignmentEvent;
    public HotbarItemSelectedEvent hotbarItemSelectedEvent;

    // the static reference to the singleton instance
    public static PlaySceneManager instance;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Awake()
    {
        if (this.useLogging)
        {
            Debug.Log("Instantiated PlaySceneManager");
        }
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        // instantiate events
        this.inventoryOpenEvent = new InventoryOpenEvent();
        this.inventoryClosedEvent = new InventoryClosedEvent();
        this.inventoryItemClickedEvent = new InventoryItemClickedEvent();
        this.inventoryItemHotbarAssignmentEvent = new InventoryItemHotbarAssignmentEvent();
        this.hotbarItemSelectedEvent = new HotbarItemSelectedEvent();
    }

    void Start()
    {
        this.proceduralEnvironmentManager.GenerateGrid();
        this.proceduralEnvironmentManager.SetGridColor(this.proceduralEnvironmentManager.defaultGridColor);
        this.gameEntityManager.Initialize();
        bool loadGameStatus = this.CheckLoadGame();
        if (!loadGameStatus)
        {
            // register initial game entities
            this.gameEntityManager.RegisterInitialGameEntities();
            // populate inventory hotbar with default items
            foreach (var gameEntityRepoItem in this.gameEntityRepoManager.items)
            {
                this.inventoryItemHotbarAssignmentEvent.Invoke(
                    gameEntityRepoItem,
                    gameEntityRepoItem.defaultHotbarAssignment
                );
            }
            this.SaveGame();
        }
    }

    void Update() { }

    // INTF METHODS

    public void OnClickSaveGame()
    {
        if (this.useLogging)
        {
            Debug.Log("OnClickSaveGame");
        }
        this.gameSaveLoadManager.SaveGame(
            SaveGameSignal.fileName,
            cameraPosition: this.playerInputManager.GetCameraPosition(),
            cameraSize: this.playerInputManager.GetCameraZoom(),
            gameEntityStates: this.gameEntityManager.GetAllGameEntityStates()
        );
    }

    public void OnClickLoadGame()
    {
        SceneManager.LoadScene("LoadGame");
    }

    public void OnClickExitPlayScene()
    {
        SceneManager.LoadScene("GameStart");
    }

    public List<HotbarItemData> GetHotbarItemDatas()
    {
        List<HotbarItemData> hotbarItemDatas = new List<HotbarItemData>();
        foreach (GameObject inventoryItem in GameObject.FindGameObjectsWithTag("HotbarItem"))
        {
            var hotbarItemData = inventoryItem.GetComponent<HotbarItemUI>().GetHotbarItemData();
            if (hotbarItemData != null)
            {
                hotbarItemDatas.Add(hotbarItemData);
            }
        }
        return hotbarItemDatas;
    }

    // IMPL METHODS

    private bool CheckLoadGame()
    {
        if (SaveGameSignal.shouldLoadFromFile)
        {
            SaveGameSignal.shouldLoadFromFile = false;
            var gameData = this.gameSaveLoadManager.LoadGame(SaveGameSignal.fileName);
            if (gameData != null)
            {
                this.playerInputManager.SetCameraPosition(gameData.cameraPosition.ToVector3());
                this.playerInputManager.SetCameraZoom(gameData.cameraSize);
                // clear all game entities from scene
                foreach (var ge in this.gameEntityManager.FindAllGameEntitiesInScene())
                {
                    Destroy(ge);
                }
                // create game entities from game data
                foreach (var gameEntityState in gameData.gameEntityStates)
                {
                    GameObject prefab = this.gameEntityRepoManager.GetGameEntityPrefabByName(gameEntityState.prefabName);
                    GameObject newEntity = Instantiate(
                        prefab,
                        Functions.QuantizeVector(gameEntityState.position.ToVector3()),
                        Functions.QuantizeQuaternion(gameEntityState.rotation.ToQuaternion())
                    );
                    this.gameEntityManager.AddGameEntity(newEntity, newEntity.transform.position);
                }
                // populate inventory hotbar with saved items
                foreach (HotbarItemData hotbarItemData in gameData.hotbarItemDatas)
                {
                    this.inventoryItemHotbarAssignmentEvent.Invoke(
                        this.gameEntityRepoManager.GetGameEntityRepoItemByName(hotbarItemData.prefabName),
                        hotbarItemData.keyCodeString
                    );
                }
                return true;
            }
        }
        return false;
    }

    private void SaveGame()
    {
        this.gameSaveLoadManager.SaveGame(
            SaveGameSignal.fileName,
            cameraPosition: this.playerInputManager.GetCameraPosition(),
            cameraSize: this.playerInputManager.GetCameraZoom(),
            gameEntityStates: this.gameEntityManager.GetAllGameEntityStates()
        );
    }


}
