using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Category", menuName = "Inventory System/New Item Category")]
public class ItemCategory : ScriptableObject
{
    public List<ItemDefinition> Items = new List<ItemDefinition>();
    public ItemType CategoryType;
    public bool CreateAllRarityVariants = false;
    public string[] AssetFolders;

    public void Populate()
    {
        Items.Clear();
        foreach (string folder in AssetFolders)
        {
            var itms = Resources.LoadAll<ItemDefinition>($"ItemScripts/{folder}");
            foreach (ItemDefinition itm in itms)
            {
                if (itm.Type != this.CategoryType)
                    throw new InvalidDataException($"{itm}");
                if (CreateAllRarityVariants)
                {
                    foreach (var rarity in Enum.GetValues(typeof(ItemRarity)).Cast<ItemRarity>())
                    {
                        var next = itm.CreateCopy();
                        next.Rarity = rarity;
                        Items.Add(next);
                    }
                }
                else
                {
                    Items.Add(itm);
                }
            }
        }
    }
}