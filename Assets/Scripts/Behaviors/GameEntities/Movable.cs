using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movable : MonoBehaviour
{


    public GameObject renderBody;

    private GameEntityManager gem;
    private List<Vector3> movementForces = new();
    private Coroutine movementCoroutine = null;

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

    public void ClearMovementForces()
    {
        this.movementForces = new List<Vector3>();
    }

    public bool CommitMovement(float animationDuration)
    {
        // short-circuit if no movement forces
        if (this.movementForces.Count == 0)
        {
            return true;
        }
        // otherwise, add forces and commit movement to position if unoccupied
        Vector3 endPos = this.transform.position;
        foreach (Vector3 movementForce in this.movementForces)
        {
            endPos += movementForce;
        }
        endPos = Functions.QuantizeVector(endPos);
        if (this.gem != null && this.gem.PositionIsFree(endPos))
        {
            this.AnimateMovement(this.transform.position, endPos, animationDuration);
            return true;
        }
        return false;
    }

    public void AnimateMovement(Vector3 startPosition, Vector3 endPosition, float animationDuration)
    {
        if (this.useLogging)
        {
            Debug.Log("Animating to position from: " + startPosition.ToString() + " to: " + endPosition.ToString());
        }
        this.gem.RemoveGameEntity(this.gameObject);
        this.gem.AddGameEntity(this.gameObject, endPosition);
        if (this.movementCoroutine != null)
        {
            StopCoroutine(this.movementCoroutine);
        }
        // snap position to discrete grid and register on game-entity manager
        this.transform.position = endPosition;
        // animate the movement of the render body (it will lag behind SOT of the transform)
        this.renderBody.transform.position = startPosition;
        this.movementCoroutine = StartCoroutine(
                Functions.MoveOverTime(
                    go: this.renderBody,
                    startPos: renderBody.transform.position,
                    endPos: endPosition,
                    duration: animationDuration
                )
        );
    }

    // IMPL METHODS


}
