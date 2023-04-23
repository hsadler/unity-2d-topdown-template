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

    public static Vector3 QuantizedMidpointFromGameObjects(List<GameObject> gameObjects)
    {
        // STUB: TODO
        var vectors = new List<Vector3>();
        return Vector3.zero;
    }


}