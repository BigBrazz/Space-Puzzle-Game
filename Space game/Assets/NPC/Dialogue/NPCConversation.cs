using System;
using UnityEngine;

/*

This is to handle conversations for the npcs
This stores every single dialogue that npc will have
Different conversations can have the same dialogue nodes

FLAGS 
all are optional
required flags: these are flags that must be set for this response to be available
blocking flags: if any of these flags are set, this response is blocked
flags to set: these are flags that will be set after choosing this response
flags to clear: these are flags that will be cleared after choosing this response

this is so that you can have unique dialogue options based on what you say, or what you do
*/

// this is the conversation
[CreateAssetMenu(fileName = "NewNPCConversation", menuName = "Dialogue/NPC Conversation")]
public class NPCConversation : ScriptableObject
{
    [Tooltip("Dialogue nodes.")]
    public NPCDialogueNode[] nodes = { new NPCDialogueNode() };

    public NPCDialogueNode GetNode(int index)
    {
        if (nodes == null || index < 0 || index >= nodes.Length)
            return null;
        return nodes[index];
    }
}

// this is one line
[Serializable]
public class NPCDialogueNode
{
    [Tooltip("optional name for the node.")]
    public string nodeName;
    
    [TextArea(3, 8)]
    public string dialogueText;

    [Tooltip("The responses the player can choose. With no choices, show a way to leave.")]
    public NPCDialogueChoice[] choices = new NPCDialogueChoice[0];
}

[Serializable]
public class NPCDialogueChoice
{
    [TextArea(1, 3)]
    public string responseText;

    [Tooltip("Index in the conversation's Nodes array. -1 ends the conversation.")]
    [Min(-1)]
    public int nextNodeIndex = -1;

    [Header("Unlock Conditions")]
    [Tooltip("All of these progress flags must be set for this response to be available.")]
    public string[] requiredFlags = new string[0];

    [Tooltip("If one of these progress flags is set, this response is blocked.")]
    public string[] blockingFlags = new string[0];

    [Tooltip("Progress flags to set after choosing this response")]
    public string[] flagsToSet = new string[0];

    [Tooltip("Progress flags to clear after choosing this response.")]
    public string[] flagsToClear = new string[0];
}
