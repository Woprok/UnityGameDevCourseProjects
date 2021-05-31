using UnityEngine;

public abstract class ItemDefinition : ScriptableObject
{
    /// <summary>
    /// Visual.
    /// </summary>
    public GameObject Prefab;
    /// <summary>
    /// What kind if item is it.
    /// </summary>
    public ItemType Type;
    /// <summary>
    /// Some funny text.
    /// </summary>
    [TextArea(15,20)]
    public string Description;
}