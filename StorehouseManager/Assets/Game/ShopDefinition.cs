using UnityEngine;
using UnityEngine.UI;

public class ShopDefinition : MonoBehaviour
{
    public GameState GameState;
    public InventoryDefinition LootInventory;
    public InventoryDefinition StoreInventory;
    public int LootSlotCost = 1;
    public int StoreSlotCost = 1;

    public Button BuyLootSlotButton;
    public Button BuyStoreSlotButton;

    public void Awake()
    {
        BuyLootSlotButton.onClick.AddListener(BuyLootSlot);
        BuyStoreSlotButton.onClick.AddListener(BuyStoreSlot);
    }

    public void BuyLootSlot()
    {
        if (GameState.CurrentCurrency - LootSlotCost > 0)
        {
            GameState.CurrentCurrency -= LootSlotCost;
            LootInventory.AddSlot();
        }

        if (!LootInventory.CanAddSlot)
        {
            BuyLootSlotButton.gameObject.SetActive(false);
        }
    }
    public void BuyStoreSlot()
    {
        if (GameState.CurrentCurrency - StoreSlotCost > 0)
        {
            GameState.CurrentCurrency -= StoreSlotCost;
            StoreInventory.AddSlot();
        }

        if (!StoreInventory.CanAddSlot)
        {
            BuyStoreSlotButton.gameObject.SetActive(false);
        }
    }
}