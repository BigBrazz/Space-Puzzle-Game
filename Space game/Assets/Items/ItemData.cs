using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField, TextArea] private string description;

    public string ItemName => itemName;
    public string Description => description;
    public abstract ItemCategory Category { get; }
    public virtual bool IsUnique => false;

    // The key the inventory stores this item under.
    public virtual string Key => itemName;

    protected virtual void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(itemName))
        {
            itemName = name;
        }
    }

    public override string ToString()
    {
        return itemName;
    }
}
