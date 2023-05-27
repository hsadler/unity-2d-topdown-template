using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{


    public bool tickIsRunning;
    public TimeTickEvent timeTickEvent;

    private bool useLogging = true;


    // UNITY HOOKS

    void Awake()
    {
        this.tickIsRunning = false;
        this.timeTickEvent = new TimeTickEvent();
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
                InvokeRepeating("SendTick", 0f, 1f);
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
