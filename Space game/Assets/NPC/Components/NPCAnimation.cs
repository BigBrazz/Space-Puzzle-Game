using UnityEngine;

// Procedural animation (no imported animation clips needed): makes the
// creature visibly "alive" with an idle bob, and turns it to face the
// player while it's chasing them.
public class NPCAnimation : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The child mesh/model that bobs up and down. Leave empty - it auto-finds a child named 'Visual', or falls back to this object.")]
    [SerializeField] private Transform visual;

    [Header("Idle Bob")]
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 2f;

    [Header("Turning")]
    [SerializeField] private float turnSpeed = 6f;

    private Vector3 basePosition;
    private NPCController npc;

    private void Awake()
    {
        npc = GetComponent<NPCController>();

        if (visual == null)
        {
            Transform childVisual = transform.Find("Visual");
            // IMPORTANT: never fall back to this object itself when it also
            // has NPCMovement - bobbing the same transform that movement
            // writes to fights with the movement every frame.
            visual = childVisual != null ? childVisual : transform;
        }

        basePosition = visual.localPosition;
    }

    private void Update()
    {
        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        visual.localPosition = basePosition + Vector3.up * bobOffset;

        if (npc == null || npc.Target == null) return;

        Vector3 direction = npc.Target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}
