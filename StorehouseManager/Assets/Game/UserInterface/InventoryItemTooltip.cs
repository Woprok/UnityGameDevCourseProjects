using System;
using System.Linq;
using System.Text;
using Assets.Game;
using TMPro;
using UnityEngine;

public class InventoryItemTooltip : MonoBehaviour
{
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public TextMeshProUGUI ItemStats;

    public void ShowTooltip(ItemDefinition item)
    {
        ItemName.text = item.Name;
        ItemDescription.text = item.Description;
        ItemStats.text = item.GetStatString();

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}

public static class InventoryDefinitionExtensions
{
    public static string GetStatString(this ItemDefinition item)
    {
        StringBuilder statsBuilder = new StringBuilder();
        
        statsBuilder.AppendLine($"{item.Rarity} {item.Type}");
        if (item.ItemData.Flags.Contains(ItemFlags.Stackable))
            statsBuilder.AppendLine($"Count: {item.StackSize}");
        if (item.ItemData.Flags.Contains(ItemFlags.Levelable))
            statsBuilder.AppendLine($"Level: {item.ItemLevel}");
        if (item.SetTags.Length > 0)
            statsBuilder.AppendLine($"Tags: {item.SetTags.Aggregate((f, l) => f + "," + l)}");
        if (item.ItemData.Flags.Contains(ItemFlags.Sellable))
            statsBuilder.AppendLine($"Sell: {(int)item.GetCurrentSellPrice()}");
        if (item.ItemData.Flags.Contains(ItemFlags.Buyable))
            statsBuilder.AppendLine($"Buy: {(int)item.GetCurrentBuyPrice()}");

        return statsBuilder.ToString();
    }
}