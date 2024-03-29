using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{


    public string prefabName;
    public string gridLayer;
    public string uuid;
    public bool isNewlyCreated = false;
    public List<GameObject> renders;

    // behavior script references
    private Selectable selectableScript;
    private Draggable draggableScript;
    private Rotatable rotatableScript;
    private Movable movableScript;


    // UNITY HOOKS

    void Awake()
    {
        this.uuid = Guid.NewGuid().ToString();
        this.selectableScript = this.GetComponent<Selectable>();
        this.draggableScript = this.GetComponent<Draggable>();
        this.rotatableScript = this.GetComponent<Rotatable>();
        this.movableScript = this.GetComponent<Movable>();
#pragma warning disable 0162
        if (GameSettings.IS_ADMIN)
        {
            this.selectableScript.enabled = true;
            this.draggableScript.enabled = true;
            this.rotatableScript.enabled = true;
            this.movableScript.enabled = true;
        }
#pragma warning restore 0162
    }

    void Start()
    {
        PlaySceneManager.instance.uiTelemetryManager.gameEntityCount += 1;
        this.SetRenderersSortingLayer(this.gridLayer);
    }

    void Update() { }

    void OnDestroy()
    {
        PlaySceneManager.instance.uiTelemetryManager.gameEntityCount -= 1;
    }

    // INTF METHODS

    public Selectable GetSelectable()
    {
        if (this.selectableScript != null && this.selectableScript.isActiveAndEnabled)
        {
            return this.selectableScript;
        }
        else
        {
            return null;
        }
    }

    public Draggable GetDraggable()
    {
        if (this.draggableScript != null && this.draggableScript.isActiveAndEnabled)
        {
            return this.draggableScript;
        }
        else
        {
            return null;
        }
    }

    public Rotatable GetRotatable()
    {
        if (this.rotatableScript != null && this.rotatableScript.isActiveAndEnabled)
        {
            return this.rotatableScript;
        }
        else
        {
            return null;
        }
    }

    public Movable GetMovable()
    {
        if (this.movableScript != null && this.movableScript.isActiveAndEnabled)
        {
            return this.movableScript;
        }
        else
        {
            return null;
        }
    }

    public bool EntityIsPlaying()
    {
        Vector3 quantizedPos = Functions.QuantizeVector(this.transform.position);
        GameObject trackedEntity = PlaySceneManager.instance.gameEntityManager.GetGameEntityAtPosition(this.gridLayer, quantizedPos);
        bool isPlaying = trackedEntity && trackedEntity == this.gameObject;
        return isPlaying;
    }

    public string GetSerializedPosition()
    {
        return Functions.QuantizeVector(this.transform.position).ToString();
    }

    public void SetUUID(string uuid)
    {
        this.uuid = uuid;
    }

    public void SetRenderersSortingLayer(string sortingLayer)
    {
        foreach (GameObject rend in this.renders)
        {
            if (rend.TryGetComponent(out SpriteRenderer sr))
            {
                sr.sortingLayerName = sortingLayer;
            }
            if (rend.TryGetComponent(out LineRenderer lr))
            {
                lr.sortingLayerName = sortingLayer;
            }
        }
    }

    public void SetSpriteRenderersOpacity(float alpha)
    {
        foreach (GameObject rend in this.renders)
        {
            if (rend.TryGetComponent(out SpriteRenderer sr))
            {
                sr.material.color = new Color(
                    sr.material.color.r,
                    sr.material.color.g,
                    sr.material.color.b,
                    alpha
                );
            }
        }
    }

    // IMPL METHODS


}
