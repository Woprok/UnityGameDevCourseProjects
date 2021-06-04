using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    internal InventorySlot Owner = null;
    private CanvasGroup CanvasGroup;

    // Its quite easy to add additional effects like highlight on OnBeginDrag.
    // We can find all objects of specified type GameObject.FindObjectsOfType<InventorySlot>().Where(slot => slot is SOMETHING);
    // This effect should be reversed on the OnEndDrag.

    public void Start()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        Owner = GetComponentInParent<InventorySlot>();
        Owner.CurrentItem = this;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Each item must be spawned with to a InventorySlot
        if (Owner == null)
        {
            throw new InvalidOperationException("InventoryItem must be placed in InventorySlot");
        }
        // Move away from real parent to a real parent parent for a dragging period.
        this.transform.SetParent(GameObject.FindGameObjectWithTag("InventoryMain").transform);
        CanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Return to parent, parents are allowed to replace each other, but item cant decide that.
        this.transform.SetParent(Owner.transform);
        this.transform.position = Owner.transform.position;
        CanvasGroup.blocksRaycasts = true;
    }
}