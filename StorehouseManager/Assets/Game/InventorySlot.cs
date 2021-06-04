using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    internal InventoryItem CurrentItem = null;

    public void OnDrop(PointerEventData eventData)
    {
        // Accept if empty.
        if (CurrentItem != null)
            return;

        var inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (inventoryItem == null)
            return;

        // we can add more conditions here like  if (slotType == dragItem.TypeOfItem)
        inventoryItem.Owner.CurrentItem = null;
        inventoryItem.Owner = this;
        CurrentItem = inventoryItem;
    }
}