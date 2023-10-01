using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{


    public SerializableVector3 cameraPosition;
    public float cameraSize;
    public List<SerializableGameEntityState> gameEntityStates;

    public GameData(SerializableVector3 cameraPosition, float cameraSize, List<SerializableGameEntityState> gameEntityStates)
    {
        this.cameraPosition = cameraPosition;
        this.cameraSize = cameraSize;
        this.gameEntityStates = gameEntityStates;
    }

    // INTF METHODS

    public string GetStringFormattedData()
    {
        return "Camera Position: " + this.cameraPosition.ToVector3().ToString() + "\n" +
            "Camera Size: " + this.cameraSize.ToString() + "\n" +
            "Game Entity States Count: " + this.gameEntityStates.Count.ToString();
    }

    // IMPLEMENTATION METHODS


}
