using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameSceneManager : MonoBehaviour
{


    // MonoBehaviour manager components
    public GameSaveLoadManager gameSaveLoadManager;

    public GameObject buttonGrid;
    public GameObject loadGameButtonPrefab;

    // the static reference to the singleton instance
    public static LoadGameSceneManager instance;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Awake()
    {
        if (this.useLogging)
        {
            Debug.Log("Instantiated LoadGameSceneManager");
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
        this.DisplaySavedGames();
    }

    void Update() { }

    // INTF METHODS

    public void OnClickTest()
    {
        Debug.Log("OnClickTest");
    }

    public void OnClickLoadPlayScene()
    {
        SaveGameSignal.shouldLoadFromFile = true;
        SceneManager.LoadScene("PlayScene");
    }

    public void OnClickExitLoadGameScene()
    {
        SceneManager.LoadScene("GameStart");
    }

    // IMPLEMENTATION METHODS

    private void DisplaySavedGames()
    {
        List<string> saveNames = this.gameSaveLoadManager.GetAllSaveNames();
        if (this.useLogging)
        {
            Debug.Log("Loaded save names: " + string.Join(", ", saveNames));
        }
        foreach (var saveName in saveNames)
        {
            var loadGameButton = Instantiate(this.loadGameButtonPrefab, this.buttonGrid.transform);
            loadGameButton.GetComponent<LoadGameButton>().SetGameName(saveName);
        }
    }


}
