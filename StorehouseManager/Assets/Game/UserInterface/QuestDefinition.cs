using TMPro;
using UnityEngine;

public interface IQuest
{
    public string QuestName { get; set; }
    public string DescriptionText { get; set; }
    public string ProgressText { get; set; }
    public ItemRarity RewardRarity { get; set; }
    public ItemType? RewardType { get; set; }
    public bool IsCompleted { get; }
    public QuestDefinition Owner { get; set; }
    public string Tag { get; set; }
    void UpdateProgress(global::Game game);
}

public abstract class Quest : IQuest
{
    public string QuestName { get; set; }
    public string DescriptionText { get; set; }
    public string ProgressText { get; set; }
    public ItemRarity RewardRarity { get; set; }
    public ItemType? RewardType { get; set; } = null;
    public bool IsCompleted { get; protected set; } = false;
    public QuestDefinition Owner { get; set; }
    public string Tag { get; set; }
    public abstract void UpdateProgress(Game game);
}

public class SimpleTakeRewardQuest : Quest
{
    private bool itemSpawned = false;
    public override void UpdateProgress(global::Game game)
    {
        if (!itemSpawned)
        {
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
        }

        if (itemSpawned && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }
}

public class SimpleWaitQuest : Quest
{
    private bool itemSpawned = false;
    private bool waitingStarted = false;
    private bool waitingFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!waitingStarted)
        {
            waitingStarted = true;
            game.UnitLootManager.OnNewLoot.AddListener(CompleteWaiting);
            return;
        }

        if (waitingFinished && !itemSpawned)
        {
            game.UnitLootManager.OnNewLoot.RemoveListener(CompleteWaiting);
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }

        if (waitingFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }

    private void CompleteWaiting()
    {
        waitingFinished = true;
    }
}

public class SimpleShopBuyQuest : Quest
{
    private bool itemSpawned = false;
    private bool itemBoughtStarted = false;
    private bool itemBoughtFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!itemBoughtStarted)
        {
            itemBoughtStarted = true;
            game.Shop.OnItemBought.AddListener(CompleteWaiting);
            return;
        }
        if (itemBoughtFinished && !itemSpawned)
        {
            game.Shop.OnItemBought.RemoveListener(CompleteWaiting);
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }
        if (itemBoughtFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }
    private void CompleteWaiting()
    {
        itemBoughtFinished = true;
    }
}

public class SimpleStoreQuest : Quest
{
    private bool itemSpawned = false;
    private bool itemStoreStarted = false;
    private bool itemStoreFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!itemStoreStarted)
        {
            itemStoreStarted = true;
            game.Storehouse.OnStore.AddListener(CompleteWaiting);
            return;
        }
        if (itemStoreFinished && !itemSpawned && Owner.QuestSlot.IsFree)
        {
            game.Storehouse.OnStore.AddListener(CompleteWaiting);
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }
        if (itemStoreFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }

    private void CompleteWaiting(int onStore)
    {
        itemStoreFinished = true;
    }
}

public class SimpleShopSellQuest : Quest
{
    private ItemDefinition slotItemDefinition;
    private bool itemSpawned = false;
    private bool itemSoldStarted = false;
    private bool itemSoldFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!itemSoldStarted)
        {
            itemSoldStarted = true;
            slotItemDefinition = game.GameItemData.Generator.GenerateAny(RewardRarity, RewardType);
            game.GameItemData.Assign(Owner.QuestSlot, slotItemDefinition);
            game.Shop.OnSpecificItemSold.AddListener(CompleteWaiting);
            return;
        }
        if (itemSoldFinished && !itemSpawned)
        {
            game.Shop.OnSpecificItemSold.AddListener(CompleteWaiting);
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }
        if (itemSoldFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }

    private void CompleteWaiting(ItemDefinition itemDefinition)
    {
        if (itemDefinition == slotItemDefinition)
            itemSoldFinished = true;
    }
}

public class SimpleUpgradeQuest : Quest
{
    private bool itemSpawned = false;
    private bool itemUpgradeStarted = false;
    private bool itemUpgradeFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!itemUpgradeStarted)
        {
            itemUpgradeStarted = true;
            game.Shop.OnItemUpgraded.AddListener(CompleteWaiting);
            return;
        }
        if (itemUpgradeFinished && !itemSpawned)
        {
            game.Shop.OnItemUpgraded.RemoveListener(CompleteWaiting);
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }
        if (itemUpgradeFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }
    private void CompleteWaiting()
    {
        itemUpgradeFinished = true;
    }
}

public class SimpleExpansionBuyQuest : Quest
{
    private bool itemSpawned = false;
    private bool itemUpgradeStarted = false;
    private bool itemUpgradeFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!itemUpgradeStarted)
        {
            itemUpgradeStarted = true;
            foreach (ExpansionDefinition expansion in game.Shop.Expansions)
            {
                expansion.OnExpansionBuy.AddListener(CompleteWaiting);
            }
            return;
        }
        if (itemUpgradeFinished && !itemSpawned)
        {
            foreach (ExpansionDefinition expansion in game.Shop.Expansions)
            {
                expansion.OnExpansionBuy.RemoveListener(CompleteWaiting);
            }
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }
        if (itemUpgradeFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }
    private void CompleteWaiting()
    {
        itemUpgradeFinished = true;
    }
}

public class SimpleDeliverQuest : Quest
{
    public string DesiredeSubname { get; set; } = null;
    public ItemRarity? DesiredeRarity { get; set; } = null;
    public ItemType? DesiredeType { get; set; } = null;
    private bool itemSpawned = false;
    private bool itemFindStarted = false;
    private bool itemFindFinished = false;
    public override void UpdateProgress(Game game)
    {
        if (!itemFindStarted)
        {
            itemFindStarted = true;
            return;
        }

        if (!itemFindFinished)
            TryComplete();

        if (itemFindFinished && !itemSpawned)
        {
            game.GameItemData.CreateSpecificFor(Owner.QuestSlot, RewardType, RewardRarity);
            itemSpawned = true;
            ProgressText = "Take reward.";
        }
        if (itemFindFinished && Owner.QuestSlot.IsFree)
        {
            IsCompleted = true;
        }
    }

    private void TryComplete()
    {
        if (Owner.QuestSlot.IsFree)
            return;

        var itmData = Owner.QuestSlot.CurrentItem.CurrentItemData;
        if (!string.IsNullOrEmpty(DesiredeSubname) && !itmData.Name.Contains(DesiredeSubname))
            return;
        if (DesiredeRarity.HasValue && itmData.Rarity != DesiredeRarity.Value)
            return;
        if (DesiredeType.HasValue && itmData.Type != DesiredeType.Value)
            return;

        itemFindFinished = true;
        Owner.QuestSlot.KillChild();
    }
}


public class QuestDefinition : MonoBehaviour
{
    public TextMeshProUGUI QuestName;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI ProgressText;
    public InventorySlot QuestSlot;
    public IQuest CurrentQuest;

    public void SetQuest(IQuest quest)
    {
        CurrentQuest = quest;
        QuestName.text = quest.QuestName;
        DescriptionText.text = quest.DescriptionText;
        ProgressText.text = quest.ProgressText;
        quest.Owner = this;
    }
}
