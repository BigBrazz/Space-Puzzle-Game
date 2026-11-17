
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField, Min(0.1f)] private float searchRadius = 4f;
    [SerializeField] private LayerMask interactableLayers;

    [Header("Prompt UI")]
    [SerializeField] private InteractionPrompt defaultPrompt;
    [SerializeField] private Transform promptParent;

    private readonly Collider[] results = new Collider[64];
    private Interactable currentTarget;
    private InteractionPrompt currentPrompt;
    private KeyCode interactionKey = KeyCode.E;
    private float holdTimer;
    private bool isHolding; 
    private bool dialogueOpen = false;

    private void Update()
    {
        if (dialogueOpen) return;
        
        Interactable nearest = FindNearestInteractable();

        if (nearest != currentTarget) SetTarget(nearest);
        if (currentTarget == null) return;

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
        if (target != null && target.InteractionKey != KeyCode.None) interactionKey = target.InteractionKey;
        else if (currentTarget != null && currentTarget.InteractionKey != KeyCode.None) interactionKey = currentTarget.InteractionKey;
        else interactionKey = KeyCode.E;

        currentTarget = target;
        isHolding = false;
        holdTimer = 0f;

        if (currentPrompt != null)
        {
            Destroy(currentPrompt.gameObject);
            currentPrompt = null;
        }

        if (currentTarget == null)
        {
            Debug.Log("No interactable target found.");
            return;  
        } 
        InteractionPrompt prefab = currentTarget.CustomPrompt != null
                ? currentTarget.CustomPrompt
                : defaultPrompt;

        if (prefab == null || promptParent == null){
            Debug.Log("Failed to create interaction prompt.");
            return;
        }
        currentPrompt = Instantiate(prefab, promptParent);
        currentPrompt.Show(
            string.Format(currentTarget.ActionText, interactionKey.ToString()),
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

            Debug.Log("Detected: " + interactable.name +
            " | Distance: " + distance +
            " | Action: " + interactable.ActionText);

            if (distance < nearestDistance)
            {
                nearest = interactable;
                nearestDistance = distance;
            }
        }

        return nearest;
    } 

    public void SetDialogueOpen(bool isOpen)
    {
        dialogueOpen = isOpen;

        if (isOpen)
        {
            isHolding = false;

            if (currentPrompt != null)
                currentPrompt.Hide();
        }
        else
        {
            SetTarget(null);
        }
    }
}
