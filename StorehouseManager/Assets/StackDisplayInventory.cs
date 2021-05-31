using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StackDisplayInventory : MonoBehaviour
{
    public InventoryDefinition Inventory;

    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int Y_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public Dictionary<InventorySlot, GameObject> DisplayedInventory = new Dictionary<InventorySlot, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    private void CreateDisplay()
    {
        int i = 0;
        foreach (InventorySlot slot in Inventory.Items)
        {
            i = CreateItem(slot, i);
        }
    }

    private int CreateItem(InventorySlot slot, int i)
    {
        var instance = Instantiate(slot.Item.Prefab, Vector3.zero, Quaternion.identity, transform);
        instance.GetComponent<RectTransform>().localPosition = GetPosition(i++);
        instance.GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount.ToString("n0");
        DisplayedInventory.Add(slot, instance);
        return i;
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), 
                           Y_START + ((-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMN))), 
                           0f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int i = 0;
        foreach (InventorySlot slot in Inventory.Items)
        {
            if (DisplayedInventory.ContainsKey(slot))
            {
                DisplayedInventory[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.Amount.ToString("n0");
            }
            else
            {
                i = CreateItem(slot, i);
            }

            i++;
        }
    }
}
