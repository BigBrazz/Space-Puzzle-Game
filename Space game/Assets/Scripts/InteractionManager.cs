
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField, Min(0.1f)] private float searchRadius = 4f;
    [SerializeField] private LayerMask interactableLayers = LayerMask.GetMask("Interactable");

    [Header("Prompt UI")]
    [SerializeField] private InteractionPrompt defaultPrompt;
    [SerializeField] private Transform promptParent;

    private readonly Collider[] results = new Collider[64];
    private Interactable currentTarget;
    private InteractionPrompt currentPrompt;
    private KeyCode interactionKey = KeyCode.E;
    private float holdTimer;
    private bool isHolding;

    private void Update()
    {
        Interactable nearest = FindNearestInteractable();

        // Update the prompt when the target changes.
        if (nearest != currentTarget) SetTarget(nearest);
        // If there is no target then we don't need to check for input.
        if (currentTarget == null) return;

        // First frame of holding, reset the timer.
        if (Input.GetKeyDown(interactionKey))
        {
            if (currentTarget.HoldDuration <= 0f)
            {
                currentTarget.Trigger(gameObject);
                return;
            }
            isHolding = true;
            holdTimer = 0f;
            currentPrompt.SetProgress(0f);
        }

        if (!isHolding) return;
        
        if (!Input.GetKey(interactionKey))
        {
            isHolding = false;
            holdTimer = 0f;
            return;
        }

        holdTimer += Time.deltaTime;

        float progress = Mathf.Clamp01(holdTimer / currentTarget.HoldDuration);
        currentPrompt.SetProgress(progress);
        
        if (holdTimer >= currentTarget.HoldDuration)
        {
            isHolding = false;
            currentTarget.Trigger(gameObject);
        }
    }

    private void SetTarget(Interactable target)
    {
        if (currentTarget.InteractionKey != KeyCode.None) interactionKey = currentTarget.InteractionKey;
        
        currentTarget = target;
        isHolding = false;
        holdTimer = 0f;
        currentPrompt.SetProgress(0f);

        // Remove the previous prompt.
        if (currentPrompt != null)
        {
            Destroy(currentPrompt.gameObject);
            currentPrompt = null;
        }

        if (currentTarget == null) return;

        // Choose custom prefab or fallback to default.
        InteractionPrompt prefab =
            currentTarget.CustomPrompt != null
                ? currentTarget.CustomPrompt
                : defaultPrompt;

        if (prefab == null || promptParent == null)
            return;

        // Create and display the new prompt.
        currentPrompt = Instantiate(prefab, promptParent);

        currentPrompt.Show(
            currentTarget.ActionText,
            currentTarget.HoldDuration > 0f
        );
    }

    private Interactable FindNearestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            searchRadius,
            results,
            interactableLayers,
            QueryTriggerInteraction.Collide
        );

        Interactable nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider hit = results[i];

            Interactable interactable =
                hit.GetComponentInParent<Interactable>();

            if (interactable == null ||
                !interactable.CanInteract())
                continue;

            float distance = Vector3.Distance(
                transform.position,
                hit.ClosestPoint(transform.position)
            );

            if (distance > interactable.ActivationDistance)
                continue;

            if (distance < nearestDistance)
            {
                nearest = interactable;
                nearestDistance = distance;
            }
        }

        return nearest;
    }
}
