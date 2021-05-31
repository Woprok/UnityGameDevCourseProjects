using UnityEngine;

[CreateAssetMenu(fileName = "New GenericGood", menuName = "Inventory System/Items/GenericGood")]
public class GenericGood : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.GenericGood;
    }
}