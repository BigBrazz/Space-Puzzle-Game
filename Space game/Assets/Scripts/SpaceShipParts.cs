
using UnityEngine;

public class SpaceShipParts : MonoBehaviour
{
    // Every SpaceshipPartData asset registered in the ItemDatabase.
    // If the database is missing, return int.MaxValue so the escape can't trigger by accident.
    public static int totalParts =>
        ItemDatabase.Instance != null
            ? ItemDatabase.Instance.Count<SpaceshipPartData>()
            : int.MaxValue;

    public static int partsCollected =>
        PlayerInventory.Instance != null
            ? PlayerInventory.Instance.GetItemCountInCategory(ItemCategory.SpaceshipPart)
            : 0;

    [Tooltip("Part given on pickup")]
    [SerializeField] private SpaceshipPartData part;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
        if (inventory == null)
            inventory = PlayerInventory.Instance;

        if (inventory == null)
        {
            Debug.LogWarning("SpaceShipParts: no PlayerInventory found on the player.", this);
            return;
        }

        // TODO MAKE IT SO THAT THE PARTS ARE THE PARTS THE PLAYER PICKS UP
        SpaceshipPartData partToGive = part;
        if (partToGive == null && ItemDatabase.Instance != null)
        {
            for (int i = 0; i < ItemDatabase.Instance.Items.Count; i++)
            {
                // Skip parts the player already owns
                if (ItemDatabase.Instance.Items[i] is SpaceshipPartData partData && !inventory.HasItem(partData))
                {
                    partToGive = partData;
                    break;
                }
            }
            
        }

        if (!inventory.AddToInventory(partToGive))
            return;

        collected = true;

        Debug.Log("Picked up " + partToGive.ItemName + ". Spaceship parts collected: " + partsCollected + "/" + totalParts);

        Destroy(gameObject);
    }

}
