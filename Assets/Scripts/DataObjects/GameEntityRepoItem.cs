using UnityEngine;

[System.Serializable]
public struct GameEntityRepoItem
{


    public GameObject prefab;
    public Sprite icon;
    public string defaultHotbarAssignment;
    public bool isInventoryAvailable;


    public GameEntityRepoItem(GameObject prefab, Sprite icon, string defaultHotbarAssignment, bool isInventoryAvailable)
    {
        this.prefab = prefab;
        this.icon = icon;
        this.defaultHotbarAssignment = defaultHotbarAssignment;
        this.isInventoryAvailable = isInventoryAvailable;
    }


}
