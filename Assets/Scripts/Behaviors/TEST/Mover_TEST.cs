using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_TEST : MonoBehaviour
{


    // UNITY HOOKS

    void Start()
    {
        InvokeRepeating("Move", 0f, 1f);
    }

    void Update() { }


    // IMPL METHODS

    private void Move()
    {
        GameEntityManager gem = PlaySceneManager.instance.gameEntityManager;
        if (gem.GetGameEntityAtPosition(this.transform.position))
        {
            Vector3 movePos = Functions.RoundVector(this.transform.position + this.transform.up);
            if (!gem.PositionIsOccupied(movePos))
            {
                gem.RemoveGameEntity(this.gameObject);
                this.transform.position = movePos;
                gem.AddGameEntity(this.gameObject);
            }
        }
    }

}
