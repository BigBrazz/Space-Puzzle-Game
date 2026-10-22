
using UnityEngine;

public class SpaceShipParts : MonoBehaviour
{
    public static int partsCollected = 0;
    public static int totalParts = 5;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;
        partsCollected++;

        Debug.Log("Spaceship parts collected: " + partsCollected + "/" + totalParts);

        Destroy(gameObject);
    }

}
