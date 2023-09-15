using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartSceneManager : MonoBehaviour
{


    void Start()
    {
        // Debug.Log("GameStartSceneManager Start");
    }

    void Update()
    {

    }

    // INTF METHODS

    public void OnClickNewGame()
    {
        // Debug.Log("OnClickStartGame");
        SceneManager.LoadScene("PlayScene");
    }

    public void OnClickLoadGame()
    {
        // Debug.Log("OnClickLoadGame");
        SceneManager.LoadScene("LoadGame");
    }

    public void OnClickQuitGame()
    {
        // Debug.Log("OnClickQuitGame");
        Application.Quit();
    }

    // IMPLEMENTATION METHODS


}
