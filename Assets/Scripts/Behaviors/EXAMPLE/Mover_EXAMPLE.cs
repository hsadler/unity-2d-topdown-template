using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Mover_EXAMPLE : MonoBehaviour
{

    private bool eventListenersRegistered;
    private System.Random random;

    private GameEntityManager gem;


    // UNITY HOOKS

    void Awake()
    {
        this.eventListenersRegistered = false;
        this.random = new System.Random();
    }

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
        this.AddEventListeners();
    }

    void Update() { }

    void OnEnable()
    {
        this.AddEventListeners();
    }

    void OnDisable()
    {
        this.RemoveEventListeners();
    }

    void OnDestroy()
    {
        this.RemoveEventListeners();
    }

    // IMPL METHODS

    private void AutoBehavior()
    {
        if (!this.gem.GetGameEntityAtPosition(this.transform.position))
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

        Vector3 movePos = Functions.RoundVector(this.transform.position + this.transform.up);
        if (!this.gem.PositionIsOccupied(movePos))
        {
            this.gem.RemoveGameEntity(this.gameObject);
            this.transform.position = movePos;
            this.gem.AddGameEntity(this.gameObject);
        }
    }

    private void AddEventListeners()
    {
        if (!this.eventListenersRegistered && PlaySceneManager.instance)
        {
            PlaySceneManager.instance.tickManager.timeTickEvent.AddListener(this.TimeTickHandler);
            this.eventListenersRegistered = true;
        }
    }

    private void RemoveEventListeners()
    {
        PlaySceneManager.instance.tickManager.timeTickEvent.RemoveListener(this.TimeTickHandler);
        this.eventListenersRegistered = false;
    }

    private void TimeTickHandler(int val)
    {
        this.AutoBehavior();
    }


}
