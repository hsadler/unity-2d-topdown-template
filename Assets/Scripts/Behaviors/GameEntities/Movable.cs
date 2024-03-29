using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movable : MonoBehaviour
{


    public GameObject renderBody;

    private List<Vector3> movementForces = new();
    private Coroutine movementCoroutine = null;

    private readonly bool useLogging = false;


    // UNITY HOOKS

    void Start() { }

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
        if (PlaySceneManager.instance.gameEntityManager != null)
        {
            GameEntityGridLayer gridLayer = PlaySceneManager.instance.gameEntityManager.GetGameEntityGridLayer(GameSettings.GAME_ENTITY_GRID_LAYER_OBJECTS);
            if (gridLayer.PositionIsValidAndFree(endPos))
            {
                this.AnimateMovement(this.transform.position, endPos, animationDuration);
                return true;
            }
        }
        return false;
    }

    public void AnimateMovement(Vector3 startPosition, Vector3 endPosition, float animationDuration)
    {
        if (this.useLogging)
        {
            Debug.Log("Animating to position from: " + startPosition.ToString() + " to: " + endPosition.ToString());
        }
        if (this.movementCoroutine != null)
        {
            StopCoroutine(this.movementCoroutine);
        }
        // snap position to discrete grid and register on game-entity manager
        string gridLayerName = this.GetComponent<GameEntity>().gridLayer;
        PlaySceneManager.instance.gameEntityManager.RemoveGameEntity(gridLayerName, this.gameObject);
        PlaySceneManager.instance.gameEntityManager.AddGameEntity(gridLayerName, this.gameObject, endPosition);
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
