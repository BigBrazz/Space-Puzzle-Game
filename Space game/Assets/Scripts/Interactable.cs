using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Prompt Setting")]
    [SerializeField] private string actionText = "Interact";
    [SerializeField, Min(0.1f)] private float activationDistance = 3f;
    [SerializeField, Min(0f)] private float holdDuration = 0f;
    [SerializeField] private InteractionPrompt customPrompt;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Events")]
    [SerializeField] private UnityEvent onTriggered;
    
    public InteractionPrompt CustomPrompt => customPrompt;
    public string ActionText => actionText;
    public float ActivationDistance => activationDistance;
    public float HoldDuration => holdDuration;
    public KeyCode InteractionKey => interactionKey;

    public bool CanInteract() {
        return enabled && gameObject.activeInHierarchy;
    }
    
    public virtual void Trigger(GameObject player) {
        if (!CanInteract()) return;
        Debug.Log($"Triggered {gameObject.name} by {player.name}");
        onTriggered?.Invoke();
    }
}
