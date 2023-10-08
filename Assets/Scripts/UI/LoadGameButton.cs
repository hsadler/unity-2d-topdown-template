using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadGameButton : MonoBehaviour
{


    private string gameName;
    [SerializeField]
    private GameObject gameNameButton;
    [SerializeField]
    private GameObject deleteGameButton;


    // UNITY HOOKS

    void Start() { }

    void Update() { }

    // INTERFACE METHODS

    public void SetGameName(string gameName)
    {
        this.gameName = gameName;
        this.gameNameButton.GetComponentInChildren<TextMeshProUGUI>().text = gameName;
        this.gameNameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            SaveGameSignal.shouldLoadFromFile = true;
            SaveGameSignal.fileName = gameName;
            SceneManager.LoadScene("PlayScene");
        });
        this.deleteGameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            LoadGameSceneManager.instance.gameSaveLoadManager.DeleteSave(this.gameName);
            Destroy(this.gameObject);
        });
    }

    // IMPLEMENTATION METHODS


}
