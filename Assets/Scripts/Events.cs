using UnityEngine.Events;

[System.Serializable]
public class TimeTickEvent : UnityEvent<int> { }

public class InventoryItemClickedEvent : UnityEvent<GameEntityRepoItem> { }

public class InventoryClosedEvent : UnityEvent { }

public class InventoryItemHotbarAssignmentEvent : UnityEvent<GameEntityRepoItem, string> { }
