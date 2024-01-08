using UnityEngine;

public class GameEntityState
{


    // Internal representation of a game entity state.


    public string uuid;
    public string prefabName;
    public string gridLayer;
    public Vector3 position;
    public Quaternion rotation;


    public GameEntityState(string uuid, string prefabName, string gridLayer, Vector3 position, Quaternion rotation)
    {
        this.uuid = uuid;
        this.prefabName = prefabName;
        this.gridLayer = gridLayer;
        this.position = position;
        this.rotation = rotation;
    }

    // INTERFACE METHODS

    public SerializableGameEntityState ToSerializable()
    {
        return new SerializableGameEntityState(this);
    }

    // IMPLEMENTATION METHODS


}
