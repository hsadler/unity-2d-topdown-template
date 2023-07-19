using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{


    private GameEntityManager gem;
    private bool useLogging = false;
    private List<Vector3> movementForces = new List<Vector3>();


    // UNITY HOOKS

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
    }

    void Update() { }

    // INTF METHODS

    public void AddMovement(Vector3 direction, int distance)
    {
        if (this.useLogging)
        {
            Debug.Log(
                "Adding movement to game-entity: " + this.gameObject.GetInstanceID().ToString() +
                " at current position: " + this.transform.position.ToString() +
                " with direction: " + direction.ToString() +
                " and distance: " + distance.ToString()
            );
        }
        this.movementForces.Add(direction * distance);
    }

    public void CommitMovement()
    {
        // short-circuit if no movement forces
        if (this.movementForces.Count == 0)
        {
            return;
        }
        // otherwise, add forces and commit movement to position if unoccupied
        Vector3 newPosition = this.transform.position;
        foreach (Vector3 movementForce in this.movementForces)
        {
            newPosition += movementForce;
        }
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
        }
        this.movementForces = new List<Vector3>();
    }

    // IMPL METHODS


}
