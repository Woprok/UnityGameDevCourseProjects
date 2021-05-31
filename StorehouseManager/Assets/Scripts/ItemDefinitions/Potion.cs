using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory System/Items/Potion")]
public class Potion : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Potion;
    }
}