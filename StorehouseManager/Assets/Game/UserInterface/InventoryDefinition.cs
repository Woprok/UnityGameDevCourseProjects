using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryDefinition : MonoBehaviour
{
    /// <summary>
    /// Defines item that is holding slots.
    /// </summary>
    public GameObject SlotHolder;
    /// <summary>
    /// Defines item that is prefab for new slots.
    /// </summary>
    public GameObject SlotPrefab;

    public int MaxSlotCount = 16;
    public int StartSlotCount = 4;
    public int CurrentSlotCount = 0;

    public SlotType SlotType = SlotType.Undefined;

    private readonly List<InventorySlot> CurrentSlots = new List<InventorySlot>();
    
    void Awake()
    {
        CreateToCount(StartSlotCount);
    }

    public bool CanAddSlot => CurrentSlotCount < MaxSlotCount;
    public InventorySlot GetEmptySlot() => CurrentSlots.FirstOrDefault(s => s.IsFree);

    public InventorySlot AddEmptySlot()
    {
        if (CanAddSlot)
            return CreateSlot();
        return null;
    }

    private void CreateToCount(int desiredCount)
    {
        while (CurrentSlotCount < desiredCount)
        {
            AddEmptySlot();
        }
    }

    private InventorySlot CreateSlot()
    {
        CurrentSlotCount++;
        var slot = Instantiate(SlotPrefab, SlotHolder.transform).GetComponent<InventorySlot>();
        slot.SlotType = SlotType;
        slot.name = $"Slot {CurrentSlotCount}";
        CurrentSlots.Add(slot);
        return slot;
    }
}
