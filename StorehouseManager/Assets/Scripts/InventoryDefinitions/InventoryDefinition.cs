using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryDefinition : ScriptableObject, ISerializationCallbackReceiver
{
    public string SavePath;
    private ItemDatabase Database;
    public List<InventorySlot> Items = new List<InventorySlot>();

    private void OnEnable()
    {
#if UNITY_EDITOR
        Database = (ItemDatabase) AssetDatabase.LoadAssetAtPath("Assets/PredefinedScripts/SpecialDatabase.asset", typeof(ItemDatabase));
#else
        Database = Resources.Load<ItemDatabase>("SpecialDatabase");
#endif
    }

    public void AddItem(ItemDefinition ITEM, int AMOUNT)
    {
        var hasItem = FindItem(ITEM);
        if (hasItem != null)
        {
            hasItem.Amount += AMOUNT;
        }
        else
        {
            Items.Add(new InventorySlot(Database.GetId[ITEM], ITEM, AMOUNT));
        }
    }

    private InventorySlot FindItem(ItemDefinition ITEM)
    {
        return Items.FirstOrDefault(slot => slot.Item == ITEM);
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        foreach (InventorySlot slot in Items)
        {
            slot.Item = Database.GetItem[slot.Id];
        }
    }

    public void Save()
    {
        string data = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SaveFile);
        bf.Serialize(file, data);
        file.Close();
    }

    public string SaveFile => $"{Application.persistentDataPath}/{SavePath}.Save";

    public void Load()
    {
        if (File.Exists(SaveFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SaveFile, FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }
}
