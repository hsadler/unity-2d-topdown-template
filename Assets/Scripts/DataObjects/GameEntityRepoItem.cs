using UnityEngine;

[System.Serializable]
public struct GameEntityRepoItem
{


    public GameObject prefab;
    public Sprite icon;
    public bool isInventoryAvailable;


    public GameEntityRepoItem(GameObject prefab, Sprite icon, bool isInventoryAvailable)
    {
        this.prefab = prefab;
        this.icon = icon;
        this.isInventoryAvailable = isInventoryAvailable;
    }


}
