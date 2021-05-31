[System.Serializable]
public class InventorySlot
{
    public int Id;
    public ItemDefinition Item;
    public int Amount;

    public InventorySlot(int ID, ItemDefinition ITEM, int AMOUNT)
    {
        Id = ID;
        Item = ITEM;
        Amount = AMOUNT;
    }
}