using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public InventoryDefinition Inventory;

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if (item)
        {
            Inventory.AddItem(item.Definition, 1);
            Destroy(other.gameObject);
        }
    }

    private void Update()
    {
        KeyBinds();
    }

    private void KeyBinds()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Inventory.Save();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            Inventory.Load();
        }

    }

    private void OnApplicationQuit()
    {
        Inventory.Items.Clear();
    }
}