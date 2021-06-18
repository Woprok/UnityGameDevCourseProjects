using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UnitLootManager
{
    private GameItemData ItemData;
    private InventoryDefinition LootInventory;
    public List<UnitLogic> CurrentUnits;
    public UnityEvent OnNewLoot = new UnityEvent();
    public UnityEvent OnMissingSpace = new UnityEvent();
    public int CurrentUnitCount = 0;
    public int MaxUnitCount = 4;

    public void Initialize(InventoryDefinition lootInventory, UnitLogic[] startingUnits, GameItemData itemData)
    {
        LootInventory = lootInventory;
        CurrentUnits = startingUnits.ToList();
        ItemData = itemData;
        CurrentUnitCount = 0;

        foreach (UnitLogic unit in CurrentUnits)
        {
            unit.OnPathFinished.AddListener(OnAdventureFinish);
            unit.SetDisable();
        }

        AddNewUnit();
    }

    public void AddNewUnit()
    {
        CurrentUnitCount = Math.Min(MaxUnitCount, ++CurrentUnitCount);
        for (int i = 0; i < CurrentUnitCount; i++)
        {
            CurrentUnits[i].SetEnabled();
        }
    }

    private void OnAdventureFinish()
    {
        var slot = LootInventory.GetEmptySlot();
        if (slot == null)
        {
            OnMissingSpace?.Invoke();
        }
        else
        {
            ItemData.CreateAnyFor(slot);
            OnNewLoot?.Invoke();
        }
    }
}

public class Game : MonoBehaviour
{
    // MonoData
    public BannerDefinition Banner;
    public InventoryDefinition LootInventory;
    public InventoryDefinition StorehouseInventory;
    public StorehouseDefinition Storehouse;
    public ShopDefinition Shop;
    public UnitLogic[] Units;
    // Runners
    public GameState GameState;
    public GameItemData GameItemData;
    // LogicManagers
    public UnitLootManager UnitLootManager;
    public bool AchievedGoals => AchievedLevel && AchievedQuest;
    public bool AchievedLevel { get; private set; } = false;
    public bool AchievedQuest { get; private set; } = false;

    public void Start()
    {
        UnitLootManager = new UnitLootManager();
        UnitLootManager.Initialize(LootInventory, Units, GameItemData);
        UnitLootManager.OnMissingSpace.AddListener(OnGameOver);

        SubscribeStorehouseStore();
        SubscribeShop();

        GameState.OnGameTimerRunnedOut.AddListener(OnTimeExpired);
        GameState.OnGameCompleted.AddListener(OnGoalAchieved);
        GameState.OnTimeMilestone.AddListener(AddUnit);
    }

    private void AddUnit()
    {
        UnitLootManager.AddNewUnit();
    }

    private void OnGoalAchieved()
    {
        AchievedLevel = true;
    }

    private void SubscribeShop()
    {
        Shop.GameItemData = GameItemData;
        Shop.StorehouseInventory = StorehouseInventory;
        Shop.OnTryItemBuy = GameState.TryReduceCurrency;
        Shop.OnItemSold.AddListener(GameState.CurrencyChange);
    }

    private void SubscribeStorehouseStore()
    {
        Storehouse.OnStore.AddListener(GameState.ReputationChange);
    }

    private void OnGameOver()
    {
        PlayerPrefsExtension.SaveScore(Banner.ReputationName.text, Banner.ReputationValue.text, false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    private void OnGameWin()
    {
        PlayerPrefsExtension.SaveScore(Banner.ReputationName.text, Banner.ReputationValue.text, true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void OnTimeExpired()
    {
        if (AchievedGoals)
            OnGameWin();
        else
            OnGameOver();
    }

    public void FinishedLastQuest()
    {
        AchievedQuest = true;
    }
}
