using UnityEngine;

public class GameEntityState
{


    // Responsible for holding a game entities state for the sake of keeping 
    // state history.


    public string uuid;
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;


    public GameEntityState(string uuid, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        this.uuid = uuid;
        this.prefab = prefab;
        this.position = position;
        this.rotation = rotation;
    }


}