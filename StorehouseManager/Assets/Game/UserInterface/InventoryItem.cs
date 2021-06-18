using System;
using Assets.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents Item. Must have assigned Item.
/// </summary>
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public InventoryItemTooltip ItemTooltip;
    public CanvasGroup CanvasGroup;
    // Color of rarity
    public Image ItemOuterBorder;
    // Not modified for now.
    public Image ItemInnerBorder;
    public Image ItemIcon;
    public TextMeshProUGUI ItemText;

    public ItemDefinition CurrentItemData { get; private set; } = null;
    public InventorySlot CurrentParent { get; private set; } = null;
    public bool HasParent => CurrentParent != null;

    public void SetItem(ItemDefinition item)
    {
        CurrentItemData = item;
        UpdateDisplayedData();
    }

    public void ClearItem()
    {
        CurrentItemData = null;

        ItemOuterBorder.color = Color.black;
        ItemIcon.sprite = null;
        ItemText.text = string.Empty;
    }

    public void UpdateDisplayedData()
    {
        ItemOuterBorder.color = CurrentItemData.Rarity.ToColor();
        ItemIcon.sprite = CurrentItemData.Icon;
        var stackText = CurrentItemData.StackSize.CurrentCount == CurrentItemData.StackSize.MinCount
            ? string.Empty
            : $"{CurrentItemData.StackSize.CurrentCount}";
        ItemText.text = $"{stackText}";
        if (ItemTooltip.isActiveAndEnabled)
            ItemTooltip.ShowTooltip(CurrentItemData);
    }

    public bool CanStackWith(InventoryItem eventSourceItem)
    {
        return CurrentItemData.IsStackableWith(eventSourceItem.CurrentItemData);
    }

    public void UpdateStackSize(InventoryItem eventSourceItem)
    {
        // Do stacking
        var otherSize = CurrentItemData.StackWith(eventSourceItem.CurrentItemData);
        UpdateDisplayedData();
        if (otherSize == 0)
        {
            // Destroy other if stack is reduced to 0
            eventSourceItem.OnSelfDestruction();
        }
        else
        {
            eventSourceItem.UpdateDisplayedData();
        }
    }
    
    #region MoveLogic

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CurrentParent.CanChildMove)
            return;
        LeaveCell();
        CanvasGroup.blocksRaycasts = false;
        ItemTooltip.HideTooltip();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CurrentParent.CanChildMove)
            return;
        FollowPointer(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CurrentParent.CanChildMove)
            return;
        EnterCell();
        CanvasGroup.blocksRaycasts = true;
    }

    private bool IsDown = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemTooltip.ShowTooltip(CurrentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsDown || !CurrentParent.CanChildMove)
            ItemTooltip.HideTooltip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDown = true;
        ItemTooltip.ShowTooltip(CurrentItemData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDown = false;
        ItemTooltip.HideTooltip();
    }

    private void BecomeFree()
    {
        this.transform.SetParent(null);
    }

    private void EnterCell()
    {
        this.transform.SetParent(CurrentParent.transform);
        this.transform.position = CurrentParent.transform.position;
    }

    /// <summary>
    /// Move away from real parent to a real parent parent for a dragging period.
    /// </summary>
    private void LeaveCell()
    {
        this.transform.SetParent(GameObject.FindGameObjectWithTag("InventoryMain").transform);
    }

    private void FollowPointer(Vector2 delta)
    {
        this.transform.position += new Vector3(delta.x, delta.y);
    }

    /// <summary>
    /// Destroy self and all existing connections.
    /// </summary>
    public void OnSelfDestruction()
    {
        ClearItem();
        LeaveParent();
        Destroy(this);
    }

    /// <summary>
    /// Make parent to abandon you.
    /// </summary>
    public void LeaveParent()
    {
        Assert.IsNotNull(CurrentParent);
        CurrentParent.AbandonChild();
    }

    /// <summary>
    /// Called by parent when creating connection.
    /// </summary>
    public void AcceptParent(InventorySlot inventorySlot)
    {
        Assert.IsNull(CurrentParent);
        CurrentParent = inventorySlot;
        EnterCell();
    }

    /// <summary>
    /// Called by Parent when removing connection.
    /// </summary>
    public void OnDisinherited()
    {
        CurrentParent = null;
        BecomeFree();
    }

    /// <summary>
    /// Handle right click interaction if eligible.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CurrentParent.HandleInteraction();
        }
    }
    #endregion
}