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
    public GameEntityManager gameEntityManager;
    public PlayerInputManager playerInputManager;
    public PlayerInventoryManager playerInventoryManager;
    public TickManager tickManager;
    public UIControlsInstructionsManager uiControlsInstructionsManager;
    public UITelemetryManager uiTelemetryManager;

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
    }

    void Start()
    {
        this.proceduralEnvironmentManager.GenerateGrid();
        this.proceduralEnvironmentManager.SetGridColor(this.proceduralEnvironmentManager.defaultGridColor);
        this.gameEntityManager.Initialize();
        this.CheckLoadGame();
    }

    void Update() { }

    // INTF METHODS

    public void OnClickRestartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnClickSaveGame()
    {
        if (this.useLogging)
        {
            Debug.Log("OnClickSaveGame");
        }
        this.gameSaveLoadManager.SaveGame(
            cameraPosition: PlaySceneManager.instance.playerInputManager.GetCameraPosition(),
            cameraSize: PlaySceneManager.instance.playerInputManager.GetCameraZoom(),
            gameEntityStates: this.gameEntityManager.GetAllGameEntityStates()
        );
    }

    public void OnClickExitPlayScene()
    {
        SceneManager.LoadScene("GameStart");
    }

    // IMPL METHODS

    public void CheckLoadGame()
    {
        if (LoadGameSignal.shouldLoadFromFile)
        {
            LoadGameSignal.shouldLoadFromFile = false;
            var gameData = this.gameSaveLoadManager.LoadGame();
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
                    GameObject prefab = this.playerInventoryManager.GetInventoryPrefabByName(gameEntityState.prefabName);
                    GameObject newEntity = Instantiate(
                        prefab,
                        Functions.QuantizeVector(gameEntityState.position.ToVector3()),
                        Functions.QuantizeQuaternion(gameEntityState.rotation.ToQuaternion())
                    );
                    this.gameEntityManager.AddGameEntity(newEntity, newEntity.transform.position);
                }
            }
            else
            {
                // register initial game entities
                this.gameEntityManager.RegisterInitialGameEntities();
            }
        }
    }


}
