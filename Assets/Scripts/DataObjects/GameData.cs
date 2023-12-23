using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{


    public SerializableVector3 cameraPosition;
    public float cameraSize;
    public List<SerializableGameEntityState> gameEntityStates;
    public List<HotbarItemData> hotbarItemDatas;

    public GameData(
        SerializableVector3 cameraPosition,
        float cameraSize,
        List<SerializableGameEntityState> gameEntityStates,
        List<HotbarItemData> hotbarItemDatas
    )
    {
        this.cameraPosition = cameraPosition;
        this.cameraSize = cameraSize;
        this.gameEntityStates = gameEntityStates;
        this.hotbarItemDatas = hotbarItemDatas;
    }

    // INTF METHODS

    public string GetStringFormattedData()
    {
        return "Camera Position: " + this.cameraPosition.ToVector3().ToString() + "\n" +
            "Camera Size: " + this.cameraSize.ToString() + "\n" +
            "Game Entity States Count: " + this.gameEntityStates.Count.ToString() + "\n" +
            "Hotbar Item Datas Count: " + this.hotbarItemDatas.Count.ToString();
    }

    // IMPLEMENTATION METHODS


}
