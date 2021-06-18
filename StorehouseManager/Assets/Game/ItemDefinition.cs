using System;
using System.Collections.Generic;
using Assets.Game;
using UnityEngine;

public enum ItemRarity
{
    Common = 0,
    Rare = 10,
    Epic = 25,
    Legendary = 50,
    Artifact = 750
}

public enum ItemType
{
    Junk,
    Part,
    Ingredient,
    Armor,
    Weapon,
    MagicalWeapon,
    Jewel,
    Beverage,
    Shield,
    Container
}

[Serializable]
public class ItemData
{
    public ItemData(float typeReputationPercentage, float rarityReputation, HashSet<ItemFlags> flags, 
        float upgradeExp, float upgradeExpPercentage, float priceBase, float finalSellPricePercentage,
        float finalBuyPricePercentage)
    {
        TypeReputationPercentage = typeReputationPercentage;
        RarityReputation = rarityReputation;
        Flags = flags;
        UpgradeExperienceBase = upgradeExp;
        UpgradeExperiencePercentage = upgradeExpPercentage;
        PriceBase = priceBase;
        FinalSellPricePercentage = finalSellPricePercentage;
        FinalBuyPricePercentage = finalBuyPricePercentage;
    }
    public ItemData(ItemData other)
    {
        TypeReputationPercentage = other.TypeReputationPercentage;
        RarityReputation = other.RarityReputation;
        Flags = other.Flags;
        UpgradeExperienceBase = other.UpgradeExperienceBase;
        UpgradeExperiencePercentage = other.UpgradeExperiencePercentage;
        PriceBase = other.PriceBase;
        FinalSellPricePercentage = other.FinalSellPricePercentage;
        FinalBuyPricePercentage = other.FinalBuyPricePercentage;
    }

    public readonly float RarityReputation;
    public readonly float TypeReputationPercentage;
    public readonly float UpgradeExperienceBase;
    public readonly float UpgradeExperiencePercentage;
    public readonly float PriceBase;
    public readonly float FinalSellPricePercentage;
    public readonly float FinalBuyPricePercentage;
    public HashSet<ItemFlags> Flags;
}

[Serializable]
public class ItemLevel
{
    public static uint ExpPerLevel = 100;
    public ItemLevel(uint minLevel, uint maxLevel)
    {
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        CurrentExp = MinLevel * ExpPerLevel;
    }
    public ItemLevel(ItemLevel other)
    {
        MinLevel = other.MinLevel;
        MaxLevel = other.MaxLevel;
        CurrentExp = other.CurrentExp;
    }
    public readonly uint MinLevel;
    public readonly uint MaxLevel;
    public uint CurrentExp;
    public uint CurrentLevel => Math.Min(CurrentExp / ExpPerLevel, MaxLevel);
    public override string ToString() => $"{CurrentLevel} / {MaxLevel}";
}

[Serializable]
public class ItemStack
{
    public ItemStack(uint minCount, uint maxCount)
    {
        MinCount = minCount;
        MaxCount = maxCount;
        CurrentCount = MinCount;
    }
    public ItemStack(ItemStack other)
    {
        MinCount = other.MinCount;
        MaxCount = other.MaxCount;
        CurrentCount = other.CurrentCount;
    }
    public readonly uint MinCount;
    public readonly uint MaxCount;
    public uint CurrentCount;

    public override string ToString() => $"{CurrentCount} of {MaxCount}";
}


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/New Item")]
public class ItemDefinition : ScriptableObject
{
    // Unique Id
    private static long NextItemId = 0;
    private static long NextUniqueId = 0;
    public long ItemId { get; private set; } = NextItemId++;
    public long UniqueId { get; } = NextUniqueId++;
    // Data Fields
    public Sprite Icon;
    public string Name = "Trash";
    public string Description = "";
    public ItemType Type = ItemType.Junk;
    public ItemRarity Rarity = ItemRarity.Common;
    public string[] SetTags = { };

    public bool CanBeLeveled()
    {
        if (ItemData.Flags.Contains(ItemFlags.Levelable) && 
            ItemLevel.CurrentLevel != ItemLevel.MaxLevel)
        {
            return true;
        }

        return false;
    }

    public ItemStack StackSize;
    public ItemLevel ItemLevel;
    public ItemData ItemData;
    public bool ValidItem => StackSize.CurrentCount >= 1;

    /// <summary>
    /// Items are equal only if they have same UniqueId.
    /// </summary>
    public override bool Equals(object other)
    {
        if (other is ItemDefinition otherItem)
            return UniqueId == otherItem.UniqueId;
        return false;
    }
    /// <summary>
    /// Hash is purely based on UniqueId.
    /// </summary>
    public override int GetHashCode() => (int)(UniqueId * base.GetHashCode());
    
    /// <summary>
    /// IsStackableWith is checked from not Equals and ItemId.
    /// </summary>
    public bool IsStackableWith(ItemDefinition other)
    {
        return !Equals(other) && ItemId == other.ItemId;
    }

    /// <summary>
    /// Updates stack and returns new value of other.
    /// </summary>
    public uint StackWith(ItemDefinition other)
    {
        var totalCount = StackSize.CurrentCount + other.StackSize.CurrentCount;
        var thisNewCount = Math.Min(StackSize.MaxCount, totalCount);
        var otherNewCount = totalCount - thisNewCount;
        StackSize.CurrentCount = thisNewCount;
        other.StackSize.CurrentCount = otherNewCount;
        return otherNewCount;
    }
    
    public ItemDefinition CreateCopy()
    {
        var copy = ScriptableObject.CreateInstance<ItemDefinition>();
        copy.ItemId = ItemId;
        copy.Icon = Icon;
        copy.Name = Name;
        copy.Description = Description;
        copy.Rarity = Rarity;
        copy.Type = Type;
        copy.SetTags = SetTags;
        copy.StackSize = new ItemStack(StackSize);
        copy.ItemLevel = new ItemLevel(ItemLevel);
        copy.ItemData = new ItemData(ItemData);
        return copy;
    }

    public float GetCurrentSellPrice()
    {
        var levelMulti = 0.5f;
        var stackMulti = 0.5f;

        if (ItemData.Flags.Contains(ItemFlags.Levelable))
            levelMulti = ItemLevel.CurrentLevel;
        if (ItemData.Flags.Contains(ItemFlags.Stackable))
            stackMulti = StackSize.CurrentCount;

        return ItemData.PriceBase
               * (levelMulti + stackMulti)
               * ItemData.FinalSellPricePercentage;
    }

    public float GetCurrentBuyPrice()
    {
        var levelMulti = 0.5f;
        var stackMulti = 0.5f;

        if (ItemData.Flags.Contains(ItemFlags.Levelable))
            levelMulti = ItemLevel.CurrentLevel;
        if (ItemData.Flags.Contains(ItemFlags.Stackable))
            stackMulti = StackSize.CurrentCount;

        return ItemData.PriceBase
               * (levelMulti + stackMulti)
               * ItemData.FinalBuyPricePercentage;
    }
}
