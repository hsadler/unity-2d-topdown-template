using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Functions
{


    public static Vector3 RoundVector(Vector3 v)
    {
        return new Vector3(
            (int)Math.Round(v.x, 0),
            (int)Math.Round(v.y, 0),
            (int)Math.Round(v.z, 0)
        );
    }

    // borrowed from: https://answers.unity.com/questions/164257/find-the-average-of-10-vectors.html
    public static Vector3 VectorMidpoint(List<Vector3> vectors)
    {
        var midpoint = vectors.Aggregate(Vector3.zero, (acc, v) => acc + v) / vectors.Count;
        return midpoint;
    }

    public static Vector3 VectorMidpointFromGameObjects(List<GameObject> gameObjects, bool quantized = false)
    {
        List<Vector3> positions = gameObjects.Select(go => go.transform.position).ToList();
        Vector3 midpoint = VectorMidpoint(positions);
        return quantized ? RoundVector(midpoint) : midpoint;
    }

    public static GameObject MostCenterGameObject(List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0)
        {
            return null;
        }
        Vector3 midpoint = VectorMidpointFromGameObjects(gameObjects);
        return gameObjects.OrderBy(go => Vector3.Distance(go.transform.position, midpoint)).First<GameObject>();
    }


}