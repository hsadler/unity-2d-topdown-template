using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Functions
{


    public static Vector3 QuantizeVector(Vector3 v)
    {
        return new Vector3(
            (int)Math.Round(v.x, 0),
            (int)Math.Round(v.y, 0),
            (int)Math.Round(v.z, 0)
        );
    }

    // TODO: delete if not needed
    // chatgpt impl
    public static Quaternion QuantizeQuaternion(Quaternion q)
    {
        Vector3 eulerAngles = q.eulerAngles;
        eulerAngles.x = Mathf.Round(eulerAngles.x / 90f) * 90f;
        eulerAngles.y = Mathf.Round(eulerAngles.y / 90f) * 90f;
        eulerAngles.z = Mathf.Round(eulerAngles.z / 90f) * 90f;
        Quaternion quantizedRotation = Quaternion.Euler(eulerAngles);
        return quantizedRotation;
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
        return quantized ? QuantizeVector(midpoint) : midpoint;
    }

    public static GameObject MostCenterGameObject(List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0)
        {
            return null;
        }
        Vector3 midpoint = VectorMidpointFromGameObjects(gameObjects);
        return gameObjects.OrderBy(go => Vector3.Distance(go.transform.position, midpoint)).First();
    }

    public static IEnumerator MoveOverTime(GameObject go, Vector3 startPos, Vector3 endPos, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            go.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        go.transform.position = endPos;
    }

    public static IEnumerator RotateOverTime(GameObject go, Quaternion startRotation, Quaternion endRotation, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            go.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        go.transform.rotation = endRotation;
    }


}