using System;
using UnityEngine;

public static class ItemTypeComparer
{
    public static bool CanAccept(this ItemTypeAcceptance acceptor, ItemType target)
    {
        switch (acceptor)
        {
            case ItemTypeAcceptance.None:
                return false;
            case ItemTypeAcceptance.Any:
                return true;
            case ItemTypeAcceptance.Armor:
                return target == ItemType.Armor;
            case ItemTypeAcceptance.Weapon:
                return target == ItemType.Weapon;
            case ItemTypeAcceptance.Jewel:
                return target == ItemType.Jewel;
            case ItemTypeAcceptance.Potion:
                return target == ItemType.Potion;
            default:
                throw new ArgumentOutOfRangeException(nameof(acceptor), acceptor, null);
        }
    }
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Artifact
}

public enum ItemTypeAcceptance
{
    None,
    Any,
    Armor,
    Weapon,
    Jewel,
    Potion,
}

public enum ItemType
{
    Junk,
    Food,
    Ingredient,
    Armor,
    Weapon,
    Jewel,
    Potion,
}

public enum ItemBehaviour
{
    Stackable,
    Upgradeable
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/New Item")]
public class ItemDefinition : ScriptableObject
{
    /// <summary>
    /// Each item has a name.
    /// </summary>
    public string Name = "Thrash";
    /// <summary>
    /// Each item has a description.
    /// </summary>
    public string Description = "Clearly this shouldn't be a loot. What did they think ?";
    /// <summary>
    /// Each item belongs to a specific item category.
    /// </summary>
    public ItemType Type = ItemType.Junk;
    /// <summary>
    /// Each item spawns or is defined with certain rarity.
    /// Rarity does not change.
    /// </summary>
    public readonly ItemRarity Rarity = ItemRarity.Common;
    /// <summary>
    /// Items can either be stacked on each other or upgradable.
    /// </summary>
    public ItemBehaviour Behaviour = ItemBehaviour.Stackable;
    public uint BehaviourMinValue = 1;
    public uint BehaviourCurrentValue = 1;
    public uint BehaviourMaxValue = 1;
}
