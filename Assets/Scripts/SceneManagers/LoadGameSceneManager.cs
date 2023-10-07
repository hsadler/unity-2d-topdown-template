using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameSceneManager : MonoBehaviour
{


    // MonoBehaviour manager components
    public GameSaveLoadManager gameSaveLoadManager;

    public GameObject buttonGrid;
    public GameObject buttonPrefab;

    private readonly bool useLogging = false;


    // UNITY HOOKS

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
            var button = Instantiate(this.buttonPrefab, this.buttonGrid.transform);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = saveName;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                SaveGameSignal.shouldLoadFromFile = true;
                SaveGameSignal.fileName = saveName;
                SceneManager.LoadScene("PlayScene");
            });
        }
    }


}
