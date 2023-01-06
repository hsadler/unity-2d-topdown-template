using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public List<GameObject> renders;

    // UNITY HOOKS

    void Start()
    {
        PlaySceneManager.instance.gameEntityManager.AddGameEntityAtPosition(this.transform.position, this.gameObject);
    }

    void Update() { }

    void OnDestroy()
    {
        Debug.Log("game entity destroyed: " + this.gameObject.name);
        PlaySceneManager.instance.gameEntityManager.RemoveGameEntityAtPosition(this.transform.position, this.gameObject);
    }

    // INTF METHODS

    public void SetRenderersSortingLayer(string sortingLayer)
    {
        // Debug.Log("Setting sorting layer to: " + sortingLayer);
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
        // Debug.Log("setting opacity to: " + alpha.ToString());
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
