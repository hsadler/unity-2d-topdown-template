using UnityEngine;

public class GameEntityState
{


    // Responsible for holding a game entities state for the sake of keeping 
    // state history.


    public string stateType;
    public int instanceId;
    public Vector3 position;
    public Quaternion rotation;


    public GameEntityState(string stateType, int instanceId, Vector3 position, Quaternion rotation)
    {
        this.stateType = stateType;
        this.instanceId = instanceId;
        this.position = position;
        this.rotation = rotation;
    }


}