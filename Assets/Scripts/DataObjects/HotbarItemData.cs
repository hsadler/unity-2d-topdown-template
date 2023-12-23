[System.Serializable]
public class HotbarItemData
{


    public string keyCodeString;
    public string prefabName;
    public bool isSelected;


    public HotbarItemData(string keyCodeString, string prefabName, bool isSelected)
    {
        this.keyCodeString = keyCodeString;
        this.prefabName = prefabName;
        this.isSelected = isSelected;
    }


}
