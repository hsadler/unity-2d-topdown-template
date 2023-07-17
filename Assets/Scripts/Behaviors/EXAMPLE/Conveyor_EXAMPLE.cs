using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor_EXAMPLE : MonoBehaviour, IGameEntityAutoBehavior
{


    public LineRenderer beamLineRenderer;

    private GameEntityManager gem;
    private bool useLogging = true;
    private int beamLength = 10;


    // UNITY HOOKS

    void Start()
    {
        this.gem = PlaySceneManager.instance.gameEntityManager;
        this.SetBeamLinePositions();
    }

    void Update() { }

    // INTF METHODS

    public void AutoBehavior()
    {
        if (this.useLogging)
        {
            Debug.Log("AutoBehavior() called for Conveyor: " + this.gameObject.GetInstanceID().ToString());
        }
        if (this.GetComponent<GameEntity>().EntityIsPlaying())
        {
            this.TryPush();
        }
    }

    // IMPL METHODS

    public void SetBeamLinePositions()
    {
        float offset = 0.5f;
        this.beamLineRenderer.SetPosition(0, Vector3.zero + (Vector3.up * offset));
        this.beamLineRenderer.SetPosition(1, Vector3.up * this.beamLength + (Vector3.up * offset));
    }

    public void TryPush()
    {
        //
        // pushes game-entities within the conveyor's beam
        //
        if (this.useLogging)
        {
            Debug.Log("Conveyor.TryPush() called for entity: " + this.gameObject.GetInstanceID().ToString());
        }
        for (int i = this.beamLength; i > 0; i--)
        {
            Vector3 beamPos = this.transform.position + (this.transform.up * i);
            if (this.useLogging)
            {
                Debug.Log("Checking beam position: " + beamPos.ToString());
            }
            GameObject go = this.gem.GetGameEntityAtPosition(beamPos);
            if (go != null)
            {
                if (this.useLogging)
                {
                    Debug.Log("Found game-entity at beam position: " + go.GetInstanceID().ToString());
                }
                Vector3 posToMove = go.transform.position + (this.transform.up * 1);
                if (this.gem.GetGameEntityAtPosition(posToMove) == null)
                {
                    if (this.useLogging)
                    {
                        Debug.Log("Conveyor is pushing game-entity to position: " + posToMove.ToString());
                    }
                    this.gem.RemoveGameEntity(go);
                    go.transform.position = posToMove;
                    this.gem.AddGameEntity(go);
                }
                else
                {
                    if (this.useLogging)
                    {
                        Debug.Log("Game-entity at conveyor beam position is blocked: " + go.GetInstanceID().ToString());
                    }
                }
            }
        }
    }


}
