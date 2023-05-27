using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Mover_TEST : MonoBehaviour
{

    private bool moverIsActive;
    private System.Random random = new System.Random();
    private GameEntityManager gem;


    // UNITY HOOKS

    void Start()
    {
        this.moverIsActive = false;
        this.gem = PlaySceneManager.instance.gameEntityManager;
        InvokeRepeating("AutoBehavior", 0f, 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.moverIsActive = !this.moverIsActive;
        }
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

}
