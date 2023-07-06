using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Mover_EXAMPLE : MonoBehaviour
{


    private GameEntityManager gem;
    private GameEntity ge;

    private bool useLogging = true;

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
        this.ge.AddAutoBehaviorAction(this.AutoBehavior());
    }

    void Update() { }

    // INTF METHODS

    public IEnumerator AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior() called for Mover: " + this.gameObject.GetInstanceID().ToString());
        }
        if (!this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            yield break;
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

    // IMPL METHODS

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


}
