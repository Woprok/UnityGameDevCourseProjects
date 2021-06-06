using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryDefinition : MonoBehaviour
{
    public int MaxSlotCount = 16;
    public int StartSlotCount = 4;
    public int CurrentSlotCount = 0;

    public ItemTypeAcceptance CanAccept = ItemTypeAcceptance.Any;

    public GameObject SlotHolder;
    /// <summary>
    /// Defines item that is prefab for new slots.
    /// </summary>
    public GameObject SlotType;

    private readonly List<InventorySlot> CurrentSlots = new List<InventorySlot>();
    
    void Awake()
    {
        CreateToCount(StartSlotCount);
    }

    public bool CanAddSlot => CurrentSlotCount < MaxSlotCount;
    public InventorySlot HasEmptySlot => CurrentSlots.FirstOrDefault(s => s.IsFree);

    public void AddSlot()
    {
        if (CanAddSlot)
            CreateSlot();
    }

    private void CreateToCount(int desiredCount)
    {
        while (CurrentSlotCount < desiredCount)
        {
            AddSlot();
        }
    }

    private void CreateSlot()
    {
        CurrentSlotCount++;
        var slot = Instantiate(SlotType, SlotHolder.transform).GetComponent<InventorySlot>();
        slot.CanAccept = CanAccept;
        slot.name = $"Slot {CurrentSlotCount}";
        CurrentSlots.Add(slot);
    }
}
