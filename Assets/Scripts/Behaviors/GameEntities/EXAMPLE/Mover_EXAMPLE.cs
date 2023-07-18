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
            Debug.Log("AutoBehavior() called for Mover: " + this.gameObject.GetInstanceID().ToString());
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
            this.transform.Rotate(new Vector3(0, 0, rot));
        }
    }

    private void Move()
    {
        //
        // moves forward one space
        //
        Vector3 movePos = Functions.RoundVector(this.transform.position + this.transform.up);
        if (!this.gem.PositionIsOccupied(movePos))
        {
            this.gem.RemoveGameEntity(this.gameObject);
            this.transform.position = movePos;
            this.gem.AddGameEntity(this.gameObject);
        }
    }


}
