using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorehouseStore : MonoBehaviour
{
    public UnityEvent OnItemStore;
    public Button StorehouseStoreButton;
    public InventorySlot[] StorehouseStoreSlots;

    public void Awake()
    {
        StorehouseStoreButton.onClick.AddListener(TryStoreItems);
    }

    public void TryStoreItems()
    {
        if (StorehouseStoreSlots.Any(s => !s.IsFree))
        {
            foreach (InventorySlot slot in StorehouseStoreSlots.Where(s => !s.IsFree))
            {
                var itemToDestroy = slot.CurrentItem;
                slot.CurrentItem = null;
                itemToDestroy.Owner.CurrentItem = null;
                itemToDestroy.Owner = null;
                itemToDestroy.transform.SetParent(null);

                Destroy(itemToDestroy);
            }

            OnItemStore.Invoke();
        }
    }
}
