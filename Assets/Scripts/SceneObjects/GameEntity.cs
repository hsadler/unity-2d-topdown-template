using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public bool isNewlyCreated = false;

    public List<GameObject> renders;

    // UNITY HOOKS

    void Start()
    {
        PlaySceneManager.instance.telemetryManager.gameEntityCount += 1;
    }

    void Update() { }

    void OnDestroy()
    {
        PlaySceneManager.instance.telemetryManager.gameEntityCount -= 1;
        // Debug.Log("game entity destroyed: " + this.gameObject.name);
    }

    // INTF METHODS

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
