using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartSceneManager : MonoBehaviour
{


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

    public void OnClickQuitGame()
    {
        Application.Quit();
    }

    // IMPLEMENTATION METHODS


}
