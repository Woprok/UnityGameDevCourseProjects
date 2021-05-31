using UnityEngine;

[CreateAssetMenu(fileName = "New Book", menuName = "Inventory System/Items/Book")]
public class Book : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Book;
    }
}