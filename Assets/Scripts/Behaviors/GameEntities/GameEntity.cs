using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{


    public string prefabName;
    public string uuid;
    public bool isNewlyCreated = false;
    public List<GameObject> renders;

    private PlaySceneManager psm;


    // UNITY HOOKS

    void Awake()
    {
        this.uuid = Guid.NewGuid().ToString();
    }

    void Start()
    {
        this.psm = PlaySceneManager.instance;
        this.psm.uiTelemetryManager.gameEntityCount += 1;
    }

    void Update() { }

    void OnDestroy()
    {
        this.psm.uiTelemetryManager.gameEntityCount -= 1;
    }

    // INTF METHODS

    public bool EntityIsPlaying()
    {
        Vector3 quantizedPos = Functions.QuantizeVector(this.transform.position);
        GameObject trackedEntity = this.psm.gameEntityManager.GetGameEntityAtPosition(quantizedPos);
        bool isPlaying = trackedEntity && trackedEntity == this.gameObject;
        return isPlaying;
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


}
