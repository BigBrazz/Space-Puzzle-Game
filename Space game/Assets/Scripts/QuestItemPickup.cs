using UnityEngine;

public class QuestItemPickup : MonoBehaviour
{
    [Tooltip("Item given on pickup. If empty, the Energy Cell is used.")]
    [SerializeField] private ItemData item;

    public void Pickup(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.Log("Player inventory not found");
            return;
        }

        if (item == null && ItemDatabase.Instance != null)
        {
            item = ItemDatabase.Instance.Find(QuestItemData.EnergyCellName);
        }

        if (inventory.AddToInventory(item))
        {
            Destroy(gameObject);
        }
    }
}
