using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLootTable : MonoBehaviour
{
    public UnityEvent OnLootAdded;
    public UnityEvent OnLootAddFail;
    public InventoryDefinition LootInventory;
    /// <summary>
    /// Defines item that is prefab for new items.
    /// </summary>
    public GameObject ItemType;

    public void OnAdventureFinish()
    {
        var slot = LootInventory.HasEmptySlot;
        if (slot != null)
        {
            AssignItem(slot, CreateItem());
            OnLootAdded.Invoke();
        }
        else
        {
            OnLootAddFail.Invoke();
        }
    }

    private ItemDefinition CreateItem()
    {
        return new ItemDefinition();
    }

    private void AssignItem(InventorySlot slot, ItemDefinition newItemData)
    {
        var item = Instantiate(ItemType, slot.transform).GetComponent<InventoryItem>();
        item.Item = newItemData;
        item.name = $"Item: {newItemData.Name}";
    }
}