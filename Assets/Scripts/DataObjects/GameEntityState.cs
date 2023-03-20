using UnityEngine;

public class GameEntityState
{


    // Responsible for holding a game entities state for the sake of keeping 
    // state history.


    public string uuid;
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;


    public GameEntityState(string uuid, string prefabName, Vector3 position, Quaternion rotation)
    {
        this.uuid = uuid;
        this.prefabName = prefabName;
        this.position = position;
        this.rotation = rotation;
    }


}