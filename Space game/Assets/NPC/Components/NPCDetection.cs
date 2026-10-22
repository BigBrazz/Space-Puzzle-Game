using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDetection : MonoBehaviour
{
    private NPCController npc;

    private readonly Collider[] results = new Collider[32];

    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        npc = GetComponent<NPCController>();
    }

    public bool CanSeePlayer()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            npc.Data.DetectionRadius,
            results,
            playerLayer
        );

        for (int i = 0; i < count; i++)
        {
            Collider target = results[i];
            Vector3 direction = (target.bounds.center - transform.position);
            float distance = direction.magnitude;
            // raycast for sight detection ensuring that no obstacles are in the way of the NPC and the player
            if (Physics.Raycast(
                transform.position, 
                direction.normalized, 
                out RaycastHit hit, 
                distance,
                ~playerLayer,
                QueryTriggerInteraction.Ignore
            ))   
            {   
                return true;
            }
        }
        return false;
    }

    public Transform GetDetectedPlayer()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            npc.Data.DetectionRadius,
            results,
            playerLayer
        );

        for (int i = 0; i < count; i++)
        {
            Collider target = results[i];
            Vector3 direction = (target.bounds.center - transform.position);
            float distance = direction.magnitude;
            // raycast for sight detection ensuring that no obstacles are in the way of the NPC and the player
            if (Physics.Raycast(
                transform.position, 
                direction.normalized, 
                out RaycastHit hit, 
                distance,
                ~playerLayer,
                QueryTriggerInteraction.Ignore
            ))   
            {   
                return target.transform;
            }
        }
        return null;
    }
}
