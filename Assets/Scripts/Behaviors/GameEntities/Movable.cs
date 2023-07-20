using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{


    private GameEntityManager gem;
    private bool useLogging = false;
    private Coroutine movementCoroutine = null;
    private List<Vector3> movementForces = new List<Vector3>();
    private Vector3 nextPosition = Vector3.zero;


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
        this.nextPosition = Functions.QuantizeVector(newPosition);
        if (this.gem != null && this.gem.GetGameEntityAtPosition(this.nextPosition) == null)
        {
            this.gem.RemoveGameEntity(this.gameObject);
            this.gem.AddGameEntity(this.gameObject, this.nextPosition);
            this.movementCoroutine = StartCoroutine(
                this.MoveOverTime(this.gameObject, this.nextPosition, GameSettings.DEFAULT_TICK_DURATION / 2)
            );
            if (this.useLogging)
            {
                Debug.Log(
                    "Moved game-entity: " + this.gameObject.GetInstanceID().ToString() +
                    " to position: " + this.nextPosition.ToString()
                );
            }
        }
        else
        {
            if (this.useLogging)
            {
                Debug.Log(
                    "Could not move game-entity: " + this.gameObject.GetInstanceID().ToString() +
                    " to position: " + this.nextPosition.ToString() + " because it is occupied."
                );
            }
        }
        this.movementForces = new List<Vector3>();
    }

    public void FastForwardAnimations()
    {
        if (this.movementCoroutine != null)
        {
            StopCoroutine(this.movementCoroutine);
            this.movementCoroutine = null;
            this.transform.position = this.nextPosition;
        }
    }

    // IMPL METHODS

    private IEnumerator MoveOverTime(GameObject go, Vector3 end, float duration)
    {
        // from chatGPT
        Vector3 start = go.transform.position;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            go.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        go.transform.position = end;
    }


}
