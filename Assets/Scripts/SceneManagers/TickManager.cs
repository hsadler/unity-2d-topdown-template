using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{


    public MyEvent1 event1;
    public MyEvent2 event2;


    // UNITY HOOKS

    void Awake()
    {
        this.event1 = new MyEvent1();
        this.event2 = new MyEvent2();
    }

    void Start()
    {
        InvokeRepeating("SendEvents", 0f, 1f);
    }

    void Update()
    {

    }

    // IMPL METHODS

    private void SendEvents()
    {
        Debug.Log("Sending events...");
        event1.Invoke(123, "Event1 message");
        event2.Invoke(234, "Event2 message");
    }


}
