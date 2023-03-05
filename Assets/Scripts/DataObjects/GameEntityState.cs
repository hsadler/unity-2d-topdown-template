using UnityEngine;

public class GameEntityState
{


    // Responsible for holding a game entities state for the sake of keeping 
    // state history.


    public int stateType;
    public int entityId;
    public Vector3 position;
    public Quaternion rotation;


    public GameEntityState(int stateType, int entityId, Vector3 position, Quaternion rotation)
    {
        this.stateType = stateType;
        this.entityId = entityId;
        this.position = position;
        this.rotation = rotation;
    }


}