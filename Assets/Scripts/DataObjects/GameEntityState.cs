using UnityEngine;

public class GameEntityState
{


    // Internal representation of a game entity state.


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

    // INTERFACE METHODS

    public SerializableGameEntityState ToSerializable()
    {
        return new SerializableGameEntityState(this);
    }

    public static GameEntityState FromSerializable(SerializableGameEntityState state)
    {
        return new GameEntityState(
            state.uuid,
            state.prefabName,
            state.position.ToVector3(),
            state.rotation.ToQuaternion()
        );
    }


    // IMPLEMENTATION METHODS


}
