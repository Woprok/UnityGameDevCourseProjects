using System;
using Assets.Game;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

[Serializable]
public enum SlotType
{
    Undefined,
    IncomingLoot,
    Storage,
    StoreArea,
    Shop,
    Quest,
    ArtisanIngredient,
    ArtisanTargetResult
}

public static class SlotExtensions
{
    public static bool CanTakeItems(this SlotType slot)
    {
        switch (slot)
        {
            case SlotType.Storage:
            case SlotType.StoreArea:
            case SlotType.Shop:
            case SlotType.Quest:
            case SlotType.ArtisanIngredient:
            case SlotType.ArtisanTargetResult:
                return true;
            case SlotType.IncomingLoot:
                return false;
            case SlotType.Undefined:
                throw new ArgumentException(nameof(slot));
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
        }
    }
    public static bool CanStackItems(this SlotType slot)
    {
        switch (slot)
        {
            case SlotType.Storage:
            case SlotType.ArtisanIngredient:
            case SlotType.StoreArea:
            case SlotType.Quest:
                return true;
            case SlotType.IncomingLoot:
            case SlotType.Shop:
            case SlotType.ArtisanTargetResult:
                return false;
            case SlotType.Undefined:
                throw new ArgumentException(nameof(slot));
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
        }
    }
    
    public static bool CanAcceptItem(this SlotType slot, ItemType type)
    {
        switch (slot)
        {
            case SlotType.IncomingLoot:
                return false;
            case SlotType.Storage:
                return true;
            case SlotType.StoreArea:
                return type.IsStoreableItem();
            case SlotType.Shop:
                return true;
            case SlotType.Quest:
                return true;
            case SlotType.ArtisanIngredient:
                return type.IsArtisanIngredient();
            case SlotType.ArtisanTargetResult:
                return type.IsGear();
            case SlotType.Undefined:
                throw new ArgumentException(nameof(slot));
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
        }
    }
}

/// <summary>
/// Slot can hold InventoryItem.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public SlotType SlotType = SlotType.Undefined;
    public GameObject SlotTemporaryIcon;

    public InventoryItem CurrentItem { get; private set; } = null;
    public bool IsFree => CurrentItem == null;
    public bool CanChildMove { get; set; } = true;

    void Start()
    {
        Assert.IsTrue(SlotType != SlotType.Undefined);
    }

    /// <summary>
    /// Set item as member of a slot.
    /// </summary>
    /// <param name="child"></param>
    public void AdoptChild(InventoryItem child)
    {
        if (child.HasParent)
            child.LeaveParent();

        Assert.IsNull(CurrentItem);
        SlotTemporaryIcon.SetActive(false);
        CurrentItem = child;
        CurrentItem.AcceptParent(this);
    }

    /// <summary>
    /// Item stops being member of a slot.
    /// </summary>
    public void AbandonChild()
    {
        Assert.IsNotNull(CurrentItem);
        CurrentItem.OnDisinherited();
        CurrentItem = null;
        SlotTemporaryIcon.SetActive(true);
    }

    /// <summary>
    /// Force child to commit suicide.
    /// </summary>
    public void KillChild()
    {
        Assert.IsNotNull(CurrentItem);
        CurrentItem.OnSelfDestruction();
    }

    /// <summary>
    /// Execute drop action.
    /// </summary>
    /// <param name="eventData">Will contain item that is expected to drop here.</param>
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem eventSourceItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (!eventSourceItem.CurrentParent.CanChildMove)
            return;
        // Source Item must be present.
        if (eventSourceItem == null)
            throw new ArgumentException($"{nameof(InventorySlot)} dropped object is not item.");

        // Slot must be valid for taking items.
        if (!SlotType.CanTakeItems())
            return;
        // Slot must be eligible for source item.
        if (!SlotType.CanAcceptItem(eventSourceItem.CurrentItemData.Type))
            return;

        if (newItemCallback != null && newItemCallback.Invoke(this, eventSourceItem))
        {
            return;
        }

        // Stacking happens if is not free and other item is also in stack-able category
        if (!IsFree && SlotType.CanStackItems() && CurrentItem.CanStackWith(eventSourceItem))
        {
            CurrentItem.UpdateStackSize(eventSourceItem);
            return;
        }

        // On free we can insert item.
        if (IsFree)
        {
            AdoptChild(eventSourceItem);
            return;
        }
    }

    private Func<InventorySlot, InventoryItem, bool> newItemCallback;
    private Func<InventorySlot, InventoryItem, bool> onSecondaryButtonInteraction;
    /// <summary>
    /// Function should return false if callback handles dropping event completely.
    /// </summary>
    public void SetDropCallback(Func<InventorySlot, InventoryItem, bool> newCallback)
    {
        this.newItemCallback = newCallback;
    }

    public void SetLostCallback(Func<InventorySlot, InventoryItem, bool> secondaryButtonInteractionHandler)
    {
        this.onSecondaryButtonInteraction = secondaryButtonInteractionHandler;
    }

    public void HandleInteraction()
    {
        onSecondaryButtonInteraction?.Invoke(this, CurrentItem);
    }
}