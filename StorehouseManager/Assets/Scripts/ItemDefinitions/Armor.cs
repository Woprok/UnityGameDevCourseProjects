using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory System/Items/Armor")]
public class Armor : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Armor;
    }
}