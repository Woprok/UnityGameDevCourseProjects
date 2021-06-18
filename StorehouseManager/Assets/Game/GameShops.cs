using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game;
using Assets.Game.UserInterface;
using UnityEngine;

public class GameShops : MonoBehaviour
{
    public Game GameData;
    public GameItemData GameItemData;
    public GameState GameState;
    public Queue<IExpansion> Expansions = new Queue<IExpansion>();
    public Queue<IQuest> Quests = new Queue<IQuest>();

    void Awake()
    {
        Expansions.Enqueue(
            new InventoryExpansion(GameData.LootInventory, GameState)
            {
                Cost = 150,
                Name = "Expand loot slots"
            }
            );
        Expansions.Enqueue(
            new InventoryExpansion(GameData.StorehouseInventory, GameState)
            {
                Cost = 75,
                Name = "Expand storehouse slots"
            }
            );

        Expansions.Enqueue(new PernamentIncomeExpansion(GameState));
        Expansions.Enqueue(new PernamentReputationExpansion(GameState));
        Quests.Enqueue(
            new SimpleTakeRewardQuest()
            {
                QuestName = "Learning 1",
                DescriptionText = "Quests are completed once you take reward. For reward to appear you have to complete progress.",
                ProgressText = "Take quest reward from slot nearby and place it in storehouse to progress.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Junk
            });        
        Quests.Enqueue(
            new SimpleTakeRewardQuest()
            {
                QuestName = "Learning 2",
                DescriptionText = "Your goal is to achieve highest possible score within time limit. " +
                                  "In upper bar you can see time, reputation title, reputation and current currency.",
                ProgressText = "Take reward.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Ingredient
            });
        Quests.Enqueue(
            new SimpleTakeRewardQuest()
            {
                QuestName = "Learning 3",
                DescriptionText = "Each item has different type, rarity and tags. " +
                                  "These determine reputation, price and eligible placements.",
                ProgressText = "Take reward.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Part
            });
        Quests.Enqueue(
            new SimpleWaitQuest()
            {
                QuestName = "Learning 4",
                DescriptionText = "Loot is generated everytime adventurers reach city. " +
                                  "Observe adventurers and wait for next loot. " +
                                  "If Loot area is filled, you will loose once adventurers reach city.",
                ProgressText = "Wait for a reward to spawn. " +
                               "You can determine how long will it take based on path adventurers are taking.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Shield
            });
        Quests.Enqueue(
            new SimpleShopBuyQuest()
            {
                QuestName = "Learning 5",
                DescriptionText = "Town offers many boons. " +
                                  "You can buy lootbox by righclicking it in shop. " +
                                  "If you do not have enough money, it will not work.",
                ProgressText = "Buy any lootbox from Generic Goods. " +
                               "Once you click, you will see some items appear in storehouse!",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Armor
            });
        Quests.Enqueue(
            new SimpleShopSellQuest()
            {
                QuestName = "Learning 6",
                DescriptionText = "Currency can be earned by selling items. " +
                                  "You can sell any item you own, these are in Loot, Storehouse, Artisan and Quest.",
                ProgressText = "Sell item in quest slot.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Jewel
            });
        Quests.Enqueue(
            new SimpleStoreQuest()
            {
                QuestName = "Learning 7",
                DescriptionText = "Reputation can be earned by storing items. " +
                                  "You can store any item you own, which has type of weapon, armor, shield, magical weapon, jewel " +
                                  "or beverage.",
                ProgressText = "Store any item you own!",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Weapon
            });
        Quests.Enqueue(
            new SimpleUpgradeQuest()
            {
                QuestName = "Learning 8",
                DescriptionText = "You can upgrade items. " +
                                  "Any weapon, armor, shield, magical weapon or jewel can be updated up to max level. " +
                                  "Other items can be stacked, to reduce amount of slots they occupies.",
                ProgressText = "Upgrade any item.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.MagicalWeapon
            });
        Quests.Enqueue(
            new SimpleExpansionBuyQuest()
            {
                QuestName = "Learning 9",
                DescriptionText = "You can buy expansions. " +
                                  "These are helpful to hold more items or get some cool bonuses.",
                ProgressText = "Buy any expansion!",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Ingredient
            });
        Quests.Enqueue(
            new SimpleDeliverQuest()
            {
                QuestName = "Learning 10",
                DescriptionText = "You should now understand the basics. " +
                                  "Do last thing for me. " +
                                  "Deliver any item to me, by placing it in quest slot...",
                ProgressText = "Deliver any item to me, by placing it in quest slot.",
                RewardRarity = ItemRarity.Common,
                RewardType = ItemType.Ingredient
            });
        int i = 0;
        foreach (ItemRarity itemRarity in Enum.GetValues(typeof(ItemRarity)).Cast<ItemRarity>())
        {
            i++;
            //for (int i = 1; i < 4; i++)
            {
                Quests.Enqueue(
                    new SimpleDeliverQuest()
                    {
                        QuestName = $"Series {itemRarity}: Weapon Delivery {i}",
                        DescriptionText = "Deliver any weapon.",
                        ProgressText = "Deliver item to me, by placing it in quest slot.",
                        RewardType = ItemType.Ingredient,
                        RewardRarity = itemRarity,
                        DesiredeType = ItemType.Weapon
                    });
                Quests.Enqueue(
                    new SimpleDeliverQuest()
                    {
                        QuestName = $"Series {itemRarity}: Armor Delivery {i}",
                        DescriptionText = "Deliver any armor.",
                        ProgressText = "Deliver item to me, by placing it in quest slot.",
                        RewardType = ItemType.Ingredient,
                        RewardRarity = itemRarity,
                        DesiredeType = ItemType.Armor
                    });
                Quests.Enqueue(
                    new SimpleDeliverQuest()
                    {
                        QuestName = $"Series {itemRarity}: Jewel Delivery {i}",
                        DescriptionText = "Deliver any jewel.",
                        ProgressText = "Deliver item to me, by placing it in quest slot.",
                        RewardType = ItemType.Ingredient,
                        RewardRarity = itemRarity,
                        DesiredeType = ItemType.Jewel
                    });
                Quests.Enqueue(
                    new SimpleDeliverQuest()
                    {
                        QuestName = $"Series {itemRarity}: Shield Delivery {i}",
                        DescriptionText = "Deliver any shield.",
                        ProgressText = "Deliver item to me, by placing it in quest slot.",
                        RewardType = ItemType.Ingredient,
                        RewardRarity = itemRarity,
                        DesiredeType = ItemType.Shield
                    });
                Quests.Enqueue(
                    new SimpleDeliverQuest()
                    {
                        QuestName = $"Series {itemRarity}: Magical weapon Delivery {i}",
                        DescriptionText = "Deliver any magical weapon.",
                        ProgressText = "Deliver item to me, by placing it in quest slot.",
                        RewardType = ItemType.Ingredient,
                        RewardRarity = itemRarity,
                        DesiredeType = ItemType.MagicalWeapon
                    });
            }
        }
    }

    void Update()
    {
        UpdateExpansions();
        UpdateQuests();
        UpdateShops();
    }

    void UpdateShops()
    {
        foreach (var shopSlot in GameData.Shop.ShopSlots.Where(ss => ss.IsFree))
        {
            GameItemData.CreateSpecificFor(shopSlot, ItemType.Container);
        }
    }

    void UpdateQuests()
    {
        foreach (QuestDefinition quest in GameData.Shop.Quests.Where(q=>q.CurrentQuest != null))
        {
            quest.CurrentQuest.UpdateProgress(GameData);
            if (quest.CurrentQuest.IsCompleted && quest.CurrentQuest.Tag == "FINAL")
            {
                GameData.FinishedLastQuest();
            }
        }

        if (Quests.Count > 0)
            foreach (QuestDefinition quest in GameData.Shop.Quests
                .Where(exp => exp.CurrentQuest == null || exp.CurrentQuest.IsCompleted))
            {
                quest.SetQuest(Quests.Dequeue());
            }
        else
        {
            Quests.Enqueue(new SimpleDeliverQuest()
            {
                QuestName = "Endless Dragon Trafficking",
                DescriptionText = "Deliver any dragon item to me and I will give you something nice in return.",
                ProgressText = "Deliver specified item to me.",
                RewardRarity = ItemRarity.Artifact,
                DesiredeSubname = "Dragon",
                Tag = "FINAL"
            });
        }
    }

    void UpdateExpansions()
    {
        if (Expansions.Count > 0)
            foreach (ExpansionDefinition expansion in GameData.Shop.Expansions
                .Where(exp => exp.CurrentExpansion == null || exp.CurrentExpansion.IsCompleted))
            {
                expansion.SetExpansion(Expansions.Dequeue());
            }
    }
}

public interface IExpansion
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public bool IsCompleted { get; }
    public bool OnBuy();
}

public class PernamentReputationExpansion : IExpansion
{
    private readonly GameState currentState;
    private const int ReputationGain = 100;

    public PernamentReputationExpansion(GameState currentState)
    {
        this.currentState = currentState;
    }

    public string Name { get; set; } = "Advertise guild collection";
    public int Cost { get; set; } = 1000;
    public bool IsCompleted => false;
    public bool OnBuy()
    {
        if (currentState.CurrentCurrency - Cost > 0)
        {
            currentState.CurrentCurrency -= Cost;
            currentState.CurrentReputation += ReputationGain;
            return true;
        }

        return false;
    }
}

public class PernamentIncomeExpansion : IExpansion
{
    private readonly GameState currentState;
    private const int PassiveGain = 1;

    public PernamentIncomeExpansion(GameState currentState)
    {
        this.currentState = currentState;
    }

    public string Name { get; set; } = "Increase guild tax";
    public int Cost { get; set; } = 100;
    public bool IsCompleted => false;
    public bool OnBuy()
    {
        if (currentState.CurrentCurrency - Cost > 0)
        {
            currentState.CurrentCurrency -= Cost;
            currentState.BasePassiveCurrencyGainPerReputationLevel += PassiveGain;

            return true;
        }

        return false;
    }
}

public class InventoryExpansion : IExpansion
{
    private readonly InventoryDefinition inventory;
    private readonly GameState currentState;

    public InventoryExpansion(InventoryDefinition inventory, GameState currentState)
    {
        this.inventory = inventory;
        this.currentState = currentState;
    }

    public string Name { get; set; }
    public int Cost { get; set; }
    public bool IsCompleted => !inventory.CanAddSlot;
    public bool OnBuy()
    {
        if (currentState.CurrentCurrency - Cost >= 0)
        {
            currentState.CurrentCurrency -= Cost;
            inventory.AddEmptySlot();

            return true;
        }

        return false;
    }
}