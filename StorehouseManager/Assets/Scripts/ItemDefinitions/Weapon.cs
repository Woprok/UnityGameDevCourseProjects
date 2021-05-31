using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory System/Items/Weapon")]
public class Weapon : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Weapon;
    }
}