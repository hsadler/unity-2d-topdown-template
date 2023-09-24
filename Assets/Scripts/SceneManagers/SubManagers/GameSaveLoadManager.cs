using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSaveLoadManager : MonoBehaviour
{


    private const string SAVE_DIR = "/saves/";
    private const string SAVE_FILENAME = "test_save.dat";

    private readonly bool useLogging = false;


    // UNITY HOOKS 

    void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + SAVE_DIR))
        {
            Directory.CreateDirectory(Application.persistentDataPath + SAVE_DIR);
        }
    }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void CheckLoadGame()
    {
        if (LoadGameSignal.shouldLoadFromFile)
        {
            LoadGameSignal.shouldLoadFromFile = false;
            // Load from file
            if (this.useLogging)
            {

                Debug.Log("Loading from file");
            }
            this.LoadGame();
        }
        else
        {
            // Load from new game
            if (this.useLogging)
            {
                Debug.Log("Loading new game");
            }
        }
    }

    public void SaveGame()
    {
        if (this.useLogging)
        {
            Debug.Log("Saving game to file: " + this.GetSavePath());
        }
        var gameData = new GameData(score: 100);
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
                Debug.Log("Loaded score from saved game data: " + gameData.score.ToString());
            }
            return gameData;
        }
    }

    // IMPLEMENTATION METHODS

    private void Init()
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
