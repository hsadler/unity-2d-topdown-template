using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{


    public GameObject renderBody;

    private GameEntityManager gem;
    private List<Vector3> movementForces = new();

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
    }

    void Update() { }

    // INTF METHODS

    public void AddMovementForce(Vector3 direction, int distance)
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

    public void CommitMovement(float animationDuration)
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
        newPosition = Functions.QuantizeVector(newPosition);
        if (this.gem != null && this.gem.GetGameEntityAtPosition(newPosition) == null)
        {
            Vector3 oldPosition = this.transform.position;
            // snap position to discrete grid and register on game-entity manager
            this.transform.position = newPosition;
            this.gem.RemoveGameEntity(this.gameObject);
            this.gem.AddGameEntity(this.gameObject, this.transform.position);
            // animate the movement of the render body (it will lag behind SOT of the transform)
            this.renderBody.transform.position = oldPosition;
            StartCoroutine(
                Functions.MoveOverTime(this.renderBody, newPosition, animationDuration)
            );
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
