using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Mover_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    private GameEntityManager gem;
    private GameEntity ge;

    private bool useLogging = false;

    private System.Random random;


    // UNITY HOOKS

    void Awake()
    {
        this.random = new System.Random();
    }

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
        this.ge = this.GetComponent<GameEntity>();
    }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior executed for Mover: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.MoveOrTurn();
        }
    }

    // IMPL METHODS

    private void MoveOrTurn()
    {
        //
        // randomly moves forward or turns, favoring movement 2/3 of the time
        //
        int randomNumber = this.random.Next(3);
        if (randomNumber == 0)
        {
            this.Turn();
        }
        else
        {
            this.Move();
        }
    }

    private void Turn()
    {
        //
        // rotates randomly left or right
        //
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
            if (this.TryGetComponent<Rotatable>(out Rotatable rotatable))
            {
                rotatable.AddRotation(rot);
            }
        }
    }

    private void Move()
    {
        //
        // add movement force in current direction
        //
        if (this.TryGetComponent<Movable>(out Movable movable))
        {
            movable.AddMovement(this.transform.up, 1);
        }
    }


}
