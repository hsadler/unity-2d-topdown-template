using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{


    private GameEntityManager gem;
    private bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
    }

    void Update() { }

    // INTF METHODS

    public bool TryMove(Vector3 direction, int distance)
    {
        if (this.useLogging)
        {
            Debug.Log(
                "Trying to move game-entity: " + this.gameObject.GetInstanceID().ToString() +
                " at current position: " + this.transform.position.ToString() +
                " with direction: " + direction.ToString() +
                " and distance: " + distance.ToString()
            );
        }
        Vector3 newPosition = this.transform.position + (direction * distance);
        if (this.gem.GetGameEntityAtPosition(newPosition) == null)
        {
            this.gem.RemoveGameEntity(this.gameObject);
            this.transform.position = newPosition;
            this.gem.AddGameEntity(this.gameObject);
            if (this.useLogging)
            {
                Debug.Log(
                    "Moved game-entity: " + this.gameObject.GetInstanceID().ToString() +
                    " to position: " + newPosition.ToString()
                );
            }
            return true;
        }
        else
        {
            if (this.useLogging)
            {
                Debug.Log(
                    "Could not move game-entity: " + this.gameObject.GetInstanceID().ToString() +
                    " to position: " + newPosition.ToString() + " because it is occupied."
                );
            }
            return false;
        }
    }

    // IMPL METHODS


}
