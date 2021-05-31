using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory System/Items/Food")]
public class Food : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Food;
    }
}