using UnityEngine;

[CreateAssetMenu(fileName = "New MonsterPart", menuName = "Inventory System/Items/MonsterPart")]
public class MonsterPart : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.MonsterPart;
    }
}