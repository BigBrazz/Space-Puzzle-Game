using UnityEngine;

public class NPCDetection : MonoBehaviour
{
    private NPCController npc;

    private readonly Collider[] results = new Collider[32];

    [Tooltip("No longer required for detection (tag-based now), kept for compatibility.")]
    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        npc = GetComponent<NPCController>();
    }

    public bool CanSeePlayer()
    {
        return GetDetectedPlayer() != null;
    }

    // Finds the player purely by the "Player" tag within DetectionRadius -
    // avoids relying on a dedicated Player physics layer, since this project
    // doesn't have one set up (everything sits on Default).
    public Transform GetDetectedPlayer()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            npc.Data.DetectionRadius,
            results
        );

        for (int i = 0; i < count; i++)
        {
            if (results[i].CompareTag("Player"))
            {
                return results[i].transform;
            }
        }
        return null;
    }
}
