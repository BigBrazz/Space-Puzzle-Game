using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData : MonoBehaviour
{
    [Header("General")]
    public string Name;
    public string Type;

    [Header("Detection")]
    public float DetectionRadius;

    [Header("Movement")]
    public float MovementSpeed;

    [Header("Behavior")]
    public bool CanFollowPlayer;

    [Header("Conversation")]
    public NPCConversation Conversation;
    
}
