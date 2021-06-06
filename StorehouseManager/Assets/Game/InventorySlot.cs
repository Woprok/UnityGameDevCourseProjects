using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    internal InventoryItem CurrentItem = null;
    public ItemTypeAcceptance CanAccept = ItemTypeAcceptance.Any;

    public bool IsFree => CurrentItem == null;

    public void OnDrop(PointerEventData eventData)
    {
        // Can take items from drop ?
        if (CanAccept == ItemTypeAcceptance.None)
            return;
        
        // Accept if empty.
        if (CurrentItem != null)
            return;

        var inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (inventoryItem == null)
            return;


        if (!CanAccept.CanAccept(inventoryItem.Item.Type))
            return;

        inventoryItem.Owner.CurrentItem = null;
        inventoryItem.Owner = this;
        CurrentItem = inventoryItem;
    }
}