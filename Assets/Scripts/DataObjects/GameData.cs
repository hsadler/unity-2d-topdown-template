
[System.Serializable]
public class GameData
{


    public int score;
    public SerializableVector3 cameraPosition;
    public float cameraSize;

    public GameData(int score, SerializableVector3 cameraPosition, float cameraSize)
    {
        this.score = score;
        this.cameraPosition = cameraPosition;
        this.cameraSize = cameraSize;
    }

    // INTF METHODS

    public string GetStringFormattedData()
    {
        return "Score: " + this.score.ToString() + "\n" +
            "Camera Position: " + this.cameraPosition.ToVector3().ToString() + "\n" +
            "Camera Size: " + this.cameraSize.ToString();
    }

    // IMPLEMENTATION METHODS


}
