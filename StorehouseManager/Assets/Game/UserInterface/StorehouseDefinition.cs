using System.Collections.Generic;
using System.Linq;
using Assets.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorehouseDefinition : MonoBehaviour
{
    public Button StorehouseStoreButton;
    public InventorySlot[] StorehouseStoreSlots;
    public UnityEvent<int> OnStore;

    public void Awake()
    {
        StorehouseStoreButton.onClick.AddListener(OnStoreItemsPressed);
    }

    private void OnStoreItemsPressed()
    {
        var occupiedSlots = StorehouseStoreSlots.Where(s => !s.IsFree).ToList();
        
        if (occupiedSlots.Any())
        {
            StoreItems(occupiedSlots);
        }
    }

    private void StoreItems(List<InventorySlot> occupiedSlots)
    {
        var items = occupiedSlots.Select(itm => itm.CurrentItem.CurrentItemData).ToList();
        
        int finalScore = CalculateScore(items);

        DestroyStoredItems(occupiedSlots);
        OnStore.Invoke(finalScore);
    }

    private Dictionary<string, float> tagBonuses = new Dictionary<string, float>();
    private float tagBase = 1.0f;
    private float tagBonus = 0.2f;
    private float uniqueTypeBonus = 0.1f;
    private void UpdateTagBonuses(List<ItemDefinition> itemDefinitions)
    {
        tagBonuses.Clear();
        foreach (ItemDefinition itemDefinition in itemDefinitions)
        {
            foreach (string setTag in itemDefinition.SetTags)
            {
                float currentTagBonus = tagBase;
                if (tagBonuses.TryGetValue(setTag, out var current))
                {
                    currentTagBonus = current + tagBonus;
                }

                tagBonuses[setTag] = currentTagBonus;
            }
        }
    }
    private float ItemLevelScore(ItemDefinition itm) => (float)itm.ItemLevel.CurrentLevel / (float)itm.ItemLevel.MaxLevel;
    private float ItemStackScore(ItemDefinition itm) => (float)itm.StackSize.CurrentCount / (float)itm.StackSize.MaxCount;
    private float TypeRarityScore(ItemDefinition itm) => itm.ItemData.TypeReputationPercentage * itm.ItemData.RarityReputation;
    private float ItemScore(ItemDefinition itm)
    {
        float stackScore = 1.0f;
        if (itm.ItemData.Flags.Contains(ItemFlags.Stackable))
        {
            stackScore += ItemStackScore(itm);
        }

        float levelScore = 1.0f;
        if (itm.ItemData.Flags.Contains(ItemFlags.Levelable))
        {
            levelScore += ItemLevelScore(itm);
        }

        return (stackScore + levelScore) * TypeRarityScore(itm);
    }

    private float UniqueItems(List<ItemDefinition> itms) => new HashSet<long>(itms.Select(itm => itm.ItemId)).Count;
    private float UniqueTypes(List<ItemDefinition> itms) => new HashSet<ItemType>(itms.Select(itm => itm.Type)).Count;
    private float UniqueBonus(List<ItemDefinition> itms) => 1.0f + UniqueTypes(itms) * uniqueTypeBonus + UniqueItems(itms) / StorehouseStoreSlots.Length;
    private int CalculateScore(List<ItemDefinition> itemDefinitions)
    {
        UpdateTagBonuses(itemDefinitions);

        float finalScore = 0;
        foreach (var itm in itemDefinitions)
        {
            float tBonus = itm.SetTags.Length > 0 ? itm.SetTags.Max(itag => tagBonuses[itag]) : 1.0f;
            float currentScore = ItemScore(itm) 
                                 * tBonus;

            finalScore += currentScore;
        }

        finalScore *= UniqueBonus(itemDefinitions);

        return (int)finalScore;
    }

    private void DestroyStoredItems(List<InventorySlot> occupiedSlots)
    {
        foreach (InventorySlot inventorySlot in occupiedSlots)
        {
            inventorySlot.KillChild();
        }
    }
}
