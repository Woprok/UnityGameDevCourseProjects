using UnityEngine;

[CreateAssetMenu(fileName = "New Jewelry", menuName = "Inventory System/Items/Jewelry")]
public class Jewelry : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Jewelry;
    }
}