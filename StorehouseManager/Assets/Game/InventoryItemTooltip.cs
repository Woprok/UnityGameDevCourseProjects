using System;
using System.Text;
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
        statsBuilder.AppendLine($"{item.Behaviour.GetBehaviourString()}: {item.GetBehaviourValueString()}");

        return statsBuilder.ToString();
    }

    public static string GetBehaviourString(this ItemBehaviour behaviour)
    {
        switch (behaviour)
        {
            case ItemBehaviour.Stackable:
                return "Count";
            case ItemBehaviour.Upgradeable: 
                return "Level";
            default:
                throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
        }
    }
    public static string GetBehaviourValueString(this ItemDefinition item)
    {
        switch (item.Behaviour)
        {
            case ItemBehaviour.Stackable:
                return $"{item.BehaviourCurrentValue}";
            case ItemBehaviour.Upgradeable:
                return $"{item.BehaviourCurrentValue} / {item.BehaviourMaxValue}";
            default:
                throw new ArgumentOutOfRangeException(nameof(item.Behaviour), item.Behaviour, null);
        }
    }
}