using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameSceneManager : MonoBehaviour
{


    void Start()
    {
        Debug.Log("LoadGameSceneManager Start");
    }

    void Update()
    {

    }

    // INTF METHODS

    public void OnClickTest()
    {
        Debug.Log("OnClickTest");
    }

    public void OnClickExitLoadGameScene()
    {
        Debug.Log("OnClickExitLoadGameScene");
        SceneManager.LoadScene("GameStart");
    }

    // IMPLEMENTATION METHODS


}
