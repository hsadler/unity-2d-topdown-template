using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSaveLoadManager : MonoBehaviour
{


    private const string SAVE_DIR = "/saves/";

    private readonly bool useLogging = false;


    // UNITY HOOKS 

    void Awake()
    {
        this.Initialize();
    }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void SaveGame(string saveName, Vector3 cameraPosition, float cameraSize, List<GameEntityState> gameEntityStates)
    {
        if (this.useLogging)
        {
            Debug.Log("Saving game to file: " + this.GetSavePath(saveName));
        }
        List<SerializableGameEntityState> serializableGameEntityStates = new();
        foreach (var gameEntity in gameEntityStates)
        {
            serializableGameEntityStates.Add(gameEntity.ToSerializable());
        }
        var gameData = new GameData(
            cameraPosition: new SerializableVector3(cameraPosition),
            cameraSize: cameraSize,
            gameEntityStates: serializableGameEntityStates
        );
        using (FileStream stream = new(this.GetSavePath(saveName), FileMode.OpenOrCreate))
        {
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, gameData);
        }
    }

    public GameData LoadGame(string saveName)
    {
        if (!this.SaveFileExists(saveName + ".dat"))
        {
            return null;
        }
        if (this.useLogging)
        {
            Debug.Log("Loading game from file: " + this.GetSavePath(saveName));
        }
        using (FileStream stream = new(this.GetSavePath(saveName), FileMode.Open))
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

    public void DeleteAllSaves()
    {
        if (this.useLogging)
        {
            Debug.Log("Deleting save files from directory: " + this.GetSaveDir());
        }
        if (Directory.Exists(this.GetSaveDir()))
        {
            Directory.Delete(this.GetSaveDir(), true);
        }
    }

    public List<string> GetAllSaveNames()
    {
        List<string> saveNames = new();
        if (Directory.Exists(this.GetSaveDir()))
        {
            foreach (var saveFilename in Directory.GetFiles(this.GetSaveDir()))
            {
                saveNames.Add(Path.GetFileNameWithoutExtension(saveFilename));
            }
        }
        return saveNames;
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

    private string GetSavePath(string saveName)
    {
        return Application.persistentDataPath + SAVE_DIR + saveName + ".dat";
    }

    private bool SaveFileExists(string saveFilename)
    {
        string saveDir = this.GetSaveDir();
        if (!Directory.Exists(Path.GetDirectoryName(saveDir)))
        {
            Debug.LogWarning("Save directory does not exist.");
            return false;
        }
        if (!File.Exists(saveDir + saveFilename))
        {
            Debug.LogWarning("Save file does not exist.");
            return false;
        }
        return true;
    }


}
