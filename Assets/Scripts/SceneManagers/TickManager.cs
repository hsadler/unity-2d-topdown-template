using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{


    public bool tickIsRunning;
    public TimeTickEvent timeTickEvent;
    public GameObject tickOnIndicator;

    private bool useLogging = false;


    // UNITY HOOKS

    void Awake()
    {
        this.tickIsRunning = false;
        this.timeTickEvent = new TimeTickEvent();
        if (this.useLogging)
        {
            Debug.Log("Instantiated TickManager");
        }
    }

    void Start()
    {
        this.tickOnIndicator.SetActive(false);
    }

    void Update()
    {
        // allow tick to play only if no entity is being dragged
        if (
            Input.GetKeyDown(GameSettings.PLAY_PAUSE_KEY) &&
            !PlaySceneManager.instance.playerInputManager.isEntityDragging &&
            PlaySceneManager.instance.playerInputManager.inputMode == GameSettings.INPUT_MODE_DEFAULT
        )
        {
            this.SetTickPlayState(!this.tickIsRunning);
        }
    }

    void OnDestroy()
    {
        CancelInvoke();
        this.timeTickEvent.RemoveAllListeners();
    }

    // INTF METHODS

    public void SetTickPlayState(bool on)
    {
        if (on)
        {
            InvokeRepeating("SendTick", GameSettings.DEFAULT_TICK_DURATION, GameSettings.DEFAULT_TICK_DURATION);
            this.tickOnIndicator.SetActive(true);
            if (this.useLogging)
            {
                Debug.Log("time tick turned ON");
            }
        }
        else
        {
            CancelInvoke();
            this.tickOnIndicator.SetActive(false);
            if (this.useLogging)
            {
                Debug.Log("time tick turned OFF");
            }
        }
        this.tickIsRunning = on;
    }

    // IMPL METHODS

    private void SendTick()
    {
        this.timeTickEvent.Invoke(1);
        if (this.useLogging)
        {
            Debug.Log("time tick event sent");
        }
    }


}
