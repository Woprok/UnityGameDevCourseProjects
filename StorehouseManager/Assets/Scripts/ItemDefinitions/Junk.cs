using UnityEngine;

[CreateAssetMenu(fileName = "New Junk", menuName = "Inventory System/Items/Junk")]
public class Junk : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Junk;
    }
}