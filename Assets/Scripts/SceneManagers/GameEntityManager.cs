using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityManager : MonoBehaviour
{


    public IDictionary<string, GameObject> positionToGameEntity = new Dictionary<string, GameObject>();
    public IDictionary<string, string> gameEntityIdToSerializedPosition = new Dictionary<string, string>();


    // UNITY HOOKS

    void Awake() { }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public bool AddGameEntityAtPosition(Vector3 position, GameObject gameEntity)
    {
        // STUB
        return false;
    }

    public bool UpdateGameEntityPosition(Vector3 position, GameObject gameEntity)
    {
        // STUB
        return false;
    }

    public bool RemoveGameEntityAtPosition(Vector3 position, GameObject gameEntity)
    {
        // STUB
        return false;
    }

    // IMPL METHODS

    private string SerializePositionVector(Vector3 position)
    {
        // STUB
        return null;
    }


}