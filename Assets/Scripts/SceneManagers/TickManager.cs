using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{


    public bool tickIsRunning;
    public TimeTickEvent timeTickEvent;

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

    void Start() { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (this.tickIsRunning)
            {
                CancelInvoke();
                if (this.useLogging)
                {
                    Debug.Log("time tick turned OFF");
                }
            }
            else
            {
                InvokeRepeating("SendTick", GameSettings.DEFAULT_TICK_DURATION, GameSettings.DEFAULT_TICK_DURATION);
                if (this.useLogging)
                {
                    Debug.Log("time tick turned ON");
                }
            }
            this.tickIsRunning = !this.tickIsRunning;
        }
    }

    void OnDestroy()
    {
        CancelInvoke();
        this.timeTickEvent.RemoveAllListeners();
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
