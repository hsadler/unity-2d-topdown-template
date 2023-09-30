using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{


    public int score;
    public SerializableVector3 cameraPosition;
    public float cameraSize;
    public List<SerializableGameEntityState> gameEntityStates;

    public GameData(int score, SerializableVector3 cameraPosition, float cameraSize, List<SerializableGameEntityState> gameEntityStates)
    {
        this.score = score;
        this.cameraPosition = cameraPosition;
        this.cameraSize = cameraSize;
        this.gameEntityStates = gameEntityStates;
    }

    // INTF METHODS

    public string GetStringFormattedData()
    {
        return "Score: " + this.score.ToString() + "\n" +
            "Camera Position: " + this.cameraPosition.ToVector3().ToString() + "\n" +
            "Camera Size: " + this.cameraSize.ToString() + "\n" +
            "Game Entity States Count: " + this.gameEntityStates.Count.ToString();
    }

    // IMPLEMENTATION METHODS


}
