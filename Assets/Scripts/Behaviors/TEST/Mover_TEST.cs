using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Mover_TEST : MonoBehaviour
{

    private bool moverIsActive;
    private bool eventListenersRegistered;
    private System.Random random;

    private GameEntityManager gem;


    // UNITY HOOKS

    void Awake()
    {
        this.moverIsActive = false;
        this.eventListenersRegistered = false;
        this.random = new System.Random();
    }

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
        this.AddEventListeners();
        InvokeRepeating("AutoBehavior", 0f, 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.moverIsActive = !this.moverIsActive;
        }
    }

    void OnEnable()
    {
        this.AddEventListeners();
    }

    void OnDisable()
    {
        this.RemoveEventListeners();
    }

    // IMPL METHODS

    private void AutoBehavior()
    {
        if (!this.moverIsActive)
        {
            return;
        }
        int randomNumber = this.random.Next(2);
        if (randomNumber == 0)
        {
            this.Move();
        }
        else
        {
            this.Turn();
        }
        this.gem.TryPushEntityStateHistoryStep();
    }

    private void Turn()
    {
        // rotates randomly left or right

        int randomNumber = this.random.Next(2);
        int rot = 0;
        if (randomNumber == 0)
        {
            rot += 90;
        }
        else
        {
            rot -= 90;
        }
        if (rot != 0)
        {
            this.transform.Rotate(new Vector3(0, 0, rot));
        }
    }

    private void Move()
    {
        // moves forward one space

        if (this.gem.GetGameEntityAtPosition(this.transform.position))
        {
            Vector3 movePos = Functions.RoundVector(this.transform.position + this.transform.up);
            if (!this.gem.PositionIsOccupied(movePos))
            {
                this.gem.RemoveGameEntity(this.gameObject);
                this.transform.position = movePos;
                this.gem.AddGameEntity(this.gameObject);
            }
        }
    }

    private void AddEventListeners()
    {
        if (!this.eventListenersRegistered && PlaySceneManager.instance)
        {
            PlaySceneManager.instance.tickManager.event1.AddListener(this.Event1Handler);
            PlaySceneManager.instance.tickManager.event2.AddListener(this.Event2Handler);
            this.eventListenersRegistered = true;
            Debug.Log("event listeners added");
        }
        else
        {
            Debug.Log("couldn't add event listeners...");
        }
    }

    private void RemoveEventListeners()
    {
        PlaySceneManager.instance.tickManager.event1.RemoveListener(this.Event1Handler);
        PlaySceneManager.instance.tickManager.event2.RemoveListener(this.Event2Handler);
        this.eventListenersRegistered = false;
        Debug.Log("event listeners removed");
    }

    private void Event1Handler(int val, string message)
    {
        Debug.Log("Event1 received with arg1: " + val.ToString() + " and arg2: " + message);
    }

    private void Event2Handler(int val, string message)
    {
        Debug.Log("Event2 received with arg1: " + val.ToString() + " and arg2: " + message);
    }


}
