using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameSceneManager : MonoBehaviour
{


    void Start() { }

    void Update() { }

    // INTF METHODS

    public void OnClickTest()
    {
        Debug.Log("OnClickTest");
    }

    public void OnClickLoadPlayScene()
    {
        LoadGameSignal.shouldLoadFromFile = true;
        SceneManager.LoadScene("PlayScene");
    }

    public void OnClickExitLoadGameScene()
    {
        SceneManager.LoadScene("GameStart");
    }

    // IMPLEMENTATION METHODS


}
