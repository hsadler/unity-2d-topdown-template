using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStartSceneManager : MonoBehaviour
{

    // MonoBehaviour manager components
    public GameSaveLoadManager gameSaveLoadManager;

    public GameObject newGameModal;
    public TMP_InputField newGameNameInputField;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.newGameModal.SetActive(false);
        this.newGameNameInputField.onSubmit.AddListener((string newGameName) =>
        {
            if (this.useLogging)
            {
                Debug.Log("Starting new game name: " + newGameName);
            }
            SaveGameSignal.shouldLoadFromFile = false;
            SaveGameSignal.fileName = newGameName;
            SceneManager.LoadScene("PlayScene");
        });
    }

    void Update()
    {
        // player input
        this.CheckEscPress();
    }

    // INTF METHODS

    public void OnClickNewGame()
    {
        this.newGameModal.SetActive(true);
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

    private void CheckEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // exit new game modal if open
            this.newGameModal.SetActive(false);
        }
    }


}
