using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{


    public bool tickIsRunning;

    public TimeTickEvent timeTickEvent;


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
            }
            else
            {
                InvokeRepeating("SendTick", 0f, 1f);
            }
            this.tickIsRunning = !this.tickIsRunning;
        }
    }

    // IMPL METHODS

    private void SendTick()
    {
        this.timeTickEvent.Invoke(1);
    }


}
