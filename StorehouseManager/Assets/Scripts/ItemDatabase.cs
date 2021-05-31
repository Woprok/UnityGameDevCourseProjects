using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Database")]
public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemDefinition[] Items;
    public Dictionary<ItemDefinition, int> GetId;
    public Dictionary<int, ItemDefinition> GetItem;

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        GetItem = new Dictionary<int, ItemDefinition>();
        GetId = new Dictionary<ItemDefinition, int>();
        int i = 0;
        foreach (ItemDefinition item in Items)
        {
            GetId.Add(item, i);
            GetItem.Add(i++, item);
        }
    }
}
