using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = System.Random;

public class ShopDefinition : MonoBehaviour
{
    public InventorySlot[] ShopSlots;
    public InventorySlot[] ArtisanIngredientsSlots;
    public InventorySlot[] ArtisanResultSlots;
    public Button ArtisanCraftButton;
    public ExpansionDefinition[] Expansions;
    public QuestDefinition[] Quests;
    // set by owner
    internal GameItemData GameItemData;
    internal InventoryDefinition StorehouseInventory;
    public UnityEvent<int> OnItemSold;
    public UnityEvent<ItemDefinition> OnSpecificItemSold;
    public Func<float, bool> OnTryItemBuy;
    public UnityEvent OnItemBought;
    public UnityEvent OnItemUpgraded;

    void Awake()
    {
        ArtisanCraftButton.onClick.AddListener(TryUpdate);
        foreach (InventorySlot shopSlot in ShopSlots)
        {
            shopSlot.SetDropCallback(HandleItemSell);
            shopSlot.SetLostCallback(HandleItemBuy);
            shopSlot.CanChildMove = false;
        }
    }

    public bool HandleItemBuy(InventorySlot callerSlot, InventoryItem boughtItem)
    {
        var freeSlot = StorehouseInventory.GetEmptySlot();
        if (boughtItem.CurrentItemData.ItemData.Flags.Contains(ItemFlags.Buyable) && freeSlot != null)
        {
            BuyItem(callerSlot, boughtItem);
            return true;
        }
        return false;
    }

    private void BuyItem(InventorySlot callerSlot, InventoryItem boughtItem)
    {
        if (!OnTryItemBuy.Invoke(boughtItem.CurrentItemData.GetCurrentBuyPrice()))
        {
            return;
        }
        if (boughtItem.CurrentItemData.Type == ItemType.Container)
        {
            BuyContaineredItem(callerSlot, boughtItem);
        }
        else
        {
            StorehouseInventory.GetEmptySlot().AdoptChild(boughtItem);
        }
        OnItemBought?.Invoke();
    }

    private void BuyContaineredItem(InventorySlot callerSlot, InventoryItem boughtItem)
    {
        List<ItemType> type = BuyExtensions.CreateTypes(boughtItem.CurrentItemData.SetTags);
        int count = BuyExtensions.CreateCount(boughtItem.CurrentItemData.SetTags);
        List<ItemRarity> generatedRarities = BuyExtensions.CreateRarityListToGenerate(boughtItem.CurrentItemData.Rarity, count);

        foreach (ItemRarity rarity in generatedRarities)
        {
            var slot = StorehouseInventory.GetEmptySlot();
            if (slot != null)
            {
                GameItemData.Assign(slot, GameItemData.Generator.GenerateAny(rarity, BuyExtensions.RandomFromList(type)));
            }
        }

        callerSlot.KillChild();
    }

    private bool HandleItemSell(InventorySlot slot, InventoryItem soldItem)
    {
        if (ShopSlots.Any(s => s.CurrentItem.CurrentItemData == soldItem.CurrentItemData))
        {
            return true;
        }

        if (soldItem.CurrentItemData.ItemData.Flags.Contains(ItemFlags.Sellable))
        {
            SellItem(soldItem.CurrentItemData);
            soldItem.OnSelfDestruction();
        }

        return true;
    }

    private void SellItem(ItemDefinition soldItem)
    {
        float finalPrice = soldItem.GetCurrentSellPrice();

        OnItemSold.Invoke((int)finalPrice);
        OnSpecificItemSold?.Invoke(soldItem);
    }

    void TryUpdate()
    {
        var upgradableItem = ArtisanResultSlots.FirstOrDefault(s => !s.IsFree);
        if (upgradableItem == null || !upgradableItem.CurrentItem.CurrentItemData.CanBeLeveled())
            return;

        var ingredientSlots = ArtisanIngredientsSlots.Where(s => !s.IsFree).ToList();
        if (ingredientSlots.Count == 0)
            return;

        UpgradeItem(ingredientSlots, upgradableItem.CurrentItem);
        OnItemUpgraded?.Invoke();
    }

    private void UpgradeItem(List<InventorySlot> ingredientSlots, InventoryItem upgradableItem)
    {
        float experience = 0.0f;
        foreach (var ingredient in ingredientSlots.Select(s => s.CurrentItem.CurrentItemData))
        {
            experience += CalculateSingleIngredientExperience(upgradableItem, ingredient);
        }

        upgradableItem.CurrentItemData.ItemLevel.CurrentExp += (uint) experience;

        foreach (InventorySlot ingredientSlot in ingredientSlots)
        {
            ingredientSlot.KillChild();
        }

        upgradableItem.UpdateDisplayedData();
    }

    private float CalculateSingleIngredientExperience(InventoryItem upgradableItem, ItemDefinition ingredient)
    {
        float ingredientBonus = ingredient.ItemData.UpgradeExperienceBase
                                * ingredient.ItemData.UpgradeExperiencePercentage
                                * ingredient.StackSize.CurrentCount;
        float maxExperience = 0.0f;

        foreach (var tagData in GameItemData.Tags)
        {
            var eligable = tagData.ArtisanUseEffectivities.FirstOrDefault(aue =>
                aue.TargetType == upgradableItem.CurrentItemData.Type);

            maxExperience = Mathf.Max(maxExperience,
                eligable.ExperienceBonus
                * ingredientBonus
            );
        }

        return maxExperience;
    }
}

public static class BuyExtensions
{
    private static readonly Random containerRandom = new Random();

    public static T RandomFromList<T>(List<T> list)
    {
        return list[containerRandom.Next(list.Count)];
    }
    public static List<ItemType> CreateTypes(string[] tags)
    {
        var set = new HashSet<ItemType>();

        foreach (string tag in tags)
        {
            if (tag == "Jewel")
                set.Add(ItemType.Jewel);
            if (tag == "Potion")
                set.Add(ItemType.Beverage);
            if (tag == "Gear")
            {
                set.Add(ItemType.Jewel);
                set.Add(ItemType.Armor);
                set.Add(ItemType.Shield);
                set.Add(ItemType.Weapon);
                set.Add(ItemType.MagicalWeapon);
            }
            if (tag == "Any")
            {
                set.Add(ItemType.Jewel);
                set.Add(ItemType.Beverage);
                set.Add(ItemType.Armor);
                set.Add(ItemType.Shield);
                set.Add(ItemType.Weapon);
                set.Add(ItemType.MagicalWeapon);
                set.Add(ItemType.Ingredient);
            }
        }

        return set.ToList();
    }
    public static int CreateCount(string[] tags)
    {
        foreach (string tag in tags)
        {
            if (tag == "Small")
                return 1;
            if (tag == "Medium")
                return 1 + containerRandom.Next(0, 1);
            if (tag == "Large")
            {
                return 2 + containerRandom.Next(0, 1);
            }
        }
        return 1;
    }

    public static List<ItemRarity> CreateRarityListToGenerate(ItemRarity rarity, int count)
    {
        List<ItemRarity> rarities = Enum.GetValues(typeof(ItemRarity)).Cast<ItemRarity>().ToList();
        rarities = rarities.Where(r => r <= rarity).ToList();

        List<ItemRarity> generatedRarities = new List<ItemRarity>() {rarity};
        count--;
        for (int i = 0; i < count; i++)
        {
            generatedRarities.Add(RandomFromList(rarities));
        }

        return generatedRarities;
    }
}