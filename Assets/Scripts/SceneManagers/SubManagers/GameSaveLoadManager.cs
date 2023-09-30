using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSaveLoadManager : MonoBehaviour
{


    private const string SAVE_DIR = "/saves/";
    private const string SAVE_FILENAME = "test_save.dat";

    private readonly bool useLogging = true;


    // UNITY HOOKS 

    void Awake()
    {
        this.Initialize();
    }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void SaveGame()
    {
        if (this.useLogging)
        {
            Debug.Log("Saving game to file: " + this.GetSavePath());
        }
        // TODO: Replace this with actual game data
        var cameraPosition = new SerializableVector3(PlaySceneManager.instance.playerInputManager.GetCameraPosition());
        float cameraSize = PlaySceneManager.instance.playerInputManager.GetCameraZoom();
        var gameData = new GameData(
            score: 100,
            cameraPosition: cameraPosition,
            cameraSize: cameraSize
        );
        using (FileStream stream = new(this.GetSavePath(), FileMode.OpenOrCreate))
        {
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, gameData);
        }
    }

    public GameData LoadGame()
    {
        if (this.useLogging)
        {
            Debug.Log("Loading game from file: " + this.GetSavePath());
        }
        using (FileStream stream = new(this.GetSavePath(), FileMode.Open))
        {
            BinaryFormatter formatter = new();
            var gameData = formatter.Deserialize(stream) as GameData;
            if (this.useLogging)
            {
                Debug.Log("Loaded from saved game data: " + gameData.GetStringFormattedData());
            }
            return gameData;
        }
    }

    // IMPLEMENTATION METHODS

    private void Initialize()
    {
        if (!Directory.Exists(this.GetSaveDir()))
        {
            Directory.CreateDirectory(this.GetSaveDir());
        }
    }

    private string GetSaveDir()
    {
        return Application.persistentDataPath + SAVE_DIR;
    }

    private string GetSavePath()
    {
        return Application.persistentDataPath + SAVE_DIR + SAVE_FILENAME;
    }


}
