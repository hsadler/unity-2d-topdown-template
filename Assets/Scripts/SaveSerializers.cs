using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableQuaternion(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

[System.Serializable]
public class SerializableGameEntityState
{
    public string uuid;
    public string prefabName;
    public SerializableVector3 position;
    public SerializableQuaternion rotation;

    public SerializableGameEntityState(GameEntityState state)
    {
        uuid = state.uuid;
        prefabName = state.prefabName;
        position = new SerializableVector3(state.position);
        rotation = new SerializableQuaternion(state.rotation);
    }

    public GameEntityState ToGameEntityState()
    {
        return new GameEntityState(uuid, prefabName, position.ToVector3(), rotation.ToQuaternion());
    }
}
