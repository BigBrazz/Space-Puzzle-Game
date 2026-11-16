using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{

    [Header("Events")]
    [SerializeField] private UnityEvent<NPCDialogueNode, NPCController> newDialogue;
    [SerializeField] private UnityEvent<NPCDialogueNode, NPCController> conversationEnded;
    [SerializeField] private UnityEvent<NPCDialogueNode, int, NPCController> conversationStarted;

    [Header("User Interface")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private UIManager uiManager;

    private NPCController currentNPC;
    private NPCConversation currentConversation;
    private int currentDialogueNodeIndex = -1;
    
    public bool InConversation => currentConversation != null;

    private void EnableUI()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);
        }
    }

    private void DisableUI()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }
    }

    public void StartConversation(NPCController npc, int dialogueNodeIndex = 0)
    {
        if (InConversation) return;
        currentNPC = npc;
        currentConversation = npc.Data.Conversation;
        currentDialogueNodeIndex = dialogueNodeIndex;
        conversationStarted?.Invoke(currentConversation.GetNode(currentDialogueNodeIndex), currentDialogueNodeIndex, currentNPC);
    }

    public void EndConversation()
    {
        conversationEnded?.Invoke(GetCurrentDialogueNode(), currentNPC);
        currentNPC = null;
        currentConversation = null;
        currentDialogueNodeIndex = -1;
    }

    public void SelectChoice(int choiceIndex)
    {
        if (!InConversation) return;

        newDialogue?.Invoke(GetCurrentDialogueNode(), currentNPC);

        var currentNode = currentConversation.GetNode(currentDialogueNodeIndex);
        if (currentNode == null || choiceIndex < 0 || choiceIndex >= currentNode.choices.Length)
            return;

        var choice = currentNode.choices[choiceIndex];
        currentDialogueNodeIndex = choice.nextNodeIndex;
    }

    private NPCDialogueNode GetCurrentDialogueNode()
    {
        if (!InConversation) return null;
        return currentConversation.GetNode(currentDialogueNodeIndex);
    }
}
