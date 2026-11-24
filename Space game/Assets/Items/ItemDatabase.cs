using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database")]
public class ItemDatabase : ScriptableObject
{
    private const string ResourcePath = "ItemDatabase";

    [SerializeField] private List<ItemData> items = new List<ItemData>();

    private static ItemDatabase instance;

    public static ItemDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ItemDatabase>(ResourcePath);
            }
            return instance;
        }
    }

    // prevents external code from modifying the list of items in the database
    public IReadOnlyList<ItemData> Items => items;

    public ItemData Find(string key)
    {
        foreach (ItemData item in items)
        {
            if (item != null && item.Key == key) { return item; }
        }
        return null;
    }

    public int Count<T>() where T : ItemData
    {
        int count = 0;
        foreach (ItemData item in items)
        {
            if (item is T) { count++; }
        }
        return count;
    }

// use this to update database when new items are added to the project 
#if UNITY_EDITOR
    [ContextMenu("Find All Items In Project")]
    private void FindAllItemsInProject()
    {
        items.Clear();
        HashSet<string> keys = new HashSet<string>();

        foreach (string guid in UnityEditor.AssetDatabase.FindAssets("t:ItemData"))
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item == null) { continue; }

            if (!keys.Add(item.Key))
            {
                Debug.LogWarning($"Duplicate item key '{item.Key}' at {path}.", item);
            }
            items.Add(item);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"ItemDatabase: found {items.Count} items.", this);
    }
#endif
}
