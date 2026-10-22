using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData : MonoBehaviour
{
    [Header("General")]
    public string npcName;
    public string npcType;

    [Header("Detection")]
    public float DetectionRadius;

    [Header("Movement")]
    public float MovementSpeed;
    
}
