using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public sealed class InventoryEntry
    {
        public ItemData Item { get; }
        public int Count { get; internal set; }

        internal InventoryEntry(ItemData item, int count)
        {
            Item = item;
            Count = count;
        }
    }

    public static PlayerInventory Instance { get; private set; }

    public event Action<ItemData, int> InventoryChanged;

    private readonly Dictionary<string, InventoryEntry> entries = new Dictionary<string, InventoryEntry>();

    private void Awake()
    {
        // Required: SpaceShipParts.partsCollected and other scripts read the inventory through Instance.
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool AddToInventory(ItemData item, int amount = 1)
    {
        if (item == null || string.IsNullOrEmpty(item.Key) || amount <= 0) { return false;}

        if (entries.TryGetValue(item.Key, out InventoryEntry existing))
        {
            if (item.IsUnique || existing.Item.IsUnique) {return false;}
            existing.Count += amount;
        }
        else
        {
            if (item.IsUnique && amount != 1) {return false;}
            entries.Add(item.Key, new InventoryEntry(item, amount));
        }
        InventoryChanged?.Invoke(item, amount);
        return true;
    }

    public bool RemoveFromInventory(string key, int amount = 1)
    {
        if (string.IsNullOrEmpty(key) || amount <= 0) {return false;}

        if (!entries.TryGetValue(key, out InventoryEntry entry) || entry.Count < amount) { return false; }

        entry.Count -= amount;
        if (entry.Count == 0)
        {
            entries.Remove(key);
        }

        InventoryChanged?.Invoke(entry.Item, -amount);
        return true;
    }

    public bool RemoveFromInventory(ItemData item, int amount = 1)
    {
        return item != null && RemoveFromInventory(item.Key, amount);
    }

    public bool HasItem(string key, int amount = 1)
    {
        return GetItemCount(key) >= amount;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        return item != null && HasItem(item.Key, amount);
    }

    public int GetItemCount(string key)
    {
        if (string.IsNullOrEmpty(key)) { return 0; }

        return entries.TryGetValue(key, out InventoryEntry entry) ? entry.Count : 0;
    }

    public int GetItemCount(ItemData item)
    {
        return item != null ? GetItemCount(item.Key) : 0;
    }

    public int GetItemCountInCategory(ItemCategory category)
    {
        int total = 0;
        foreach (InventoryEntry entry in entries.Values)
        {
            if (entry.Item.Category == category)
            {
                total += entry.Count;
            }
        }
        return total;
    }

    public List<InventoryEntry> GetAllItems()
    {
        return new List<InventoryEntry>(entries.Values);
    }

    public void ClearInventory()
    {
        foreach (InventoryEntry entry in GetAllItems())
        {
            RemoveFromInventory(entry.Item.Key, entry.Count);
        }
    }

    public void RemoveAllOfCategory(ItemCategory category)
    {        
        List<InventoryEntry> itemsToRemove = new List<InventoryEntry>();
        // two loop to avoid iteration bugs while modifying
        foreach (InventoryEntry entry in entries.Values)
        {
            if (entry.Item.Category == category)
            {
                itemsToRemove.Add(entry);
            }
        }

        foreach (InventoryEntry entry in itemsToRemove)
        {
            RemoveFromInventory(entry.Item.Key, entry.Count);
        }
    }
}
