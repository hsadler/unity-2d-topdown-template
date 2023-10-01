using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartSceneManager : MonoBehaviour
{


    public GameSaveLoadManager gameSaveLoadManager;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void OnClickNewGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnClickLoadGame()
    {
        SceneManager.LoadScene("LoadGame");
    }

    public void OnClickDeleteSaves()
    {
        this.gameSaveLoadManager.DeleteAllSaves();
    }

    public void OnClickQuitGame()
    {
        Application.Quit();
    }

    // IMPLEMENTATION METHODS


}
