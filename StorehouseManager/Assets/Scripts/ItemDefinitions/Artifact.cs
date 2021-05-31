using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Inventory System/Items/Artifact")]
public class Artifact : ItemDefinition
{
    public void Awake()
    {
        Type = ItemType.Artifact;
    }
}