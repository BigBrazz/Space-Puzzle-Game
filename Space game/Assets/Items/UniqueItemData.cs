using System.Text;
using UnityEngine;

public abstract class UniqueItemData : ItemData
{
    [Tooltip("Tick to set the id manually instead of deriving it from the item name.")]
    [SerializeField] private bool overrideId;
    [SerializeField] private string id;

    public string Id => id;
    public override bool IsUnique => true;
    public override string Key => id;
}
