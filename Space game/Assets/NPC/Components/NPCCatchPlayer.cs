using UnityEngine;

// Gives the creature an actual consequence for catching the player, so
// "avoid the creatures" is a real mechanic and not just flavour text.
// Requires a Collider on this object (or a child) with "Is Trigger" ticked.
public class NPCCatchPlayer : MonoBehaviour
{
    [Tooltip("Optional - if left empty, this auto-finds the OxygenSystem already in the scene.")]
    [SerializeField] private OxygenSystem oxygenSystem;

    private bool hasCaughtPlayer = false;

    private void Awake()
    {
        if (oxygenSystem == null)
        {
            oxygenSystem = FindObjectOfType<OxygenSystem>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasCaughtPlayer || !other.CompareTag("Player")) return;
        if (oxygenSystem == null)
        {
            Debug.LogWarning("NPCCatchPlayer: no OxygenSystem found in the scene, cannot end the game.");
            return;
        }

        hasCaughtPlayer = true;
        Debug.Log("Player was caught by a creature!");
        oxygenSystem.GameOver();
    }
}
