using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("NPC Data")]
    [Tooltip("Optional - if left empty, this auto-fills from the NPCData component on this same object.")]
    [SerializeField] private NPCData npcData;

    // Components
    public NPCMovement Movement { get; private set; }
    public NPCDetection Detection { get; private set; }
    public NPCAnimation Animation { get; private set; }

    private NPCStateMachine StateMachine;
    public NPCData Data => npcData;
    public Transform Target { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<NPCMovement>();
        Detection = GetComponent<NPCDetection>();
        Animation = GetComponent<NPCAnimation>();

        if (npcData == null)
        {
            npcData = GetComponent<NPCData>();
        }

        StateMachine = new NPCStateMachine();
    }

    private void Start()
    {
        ChangeState(new NPCIdleState(this));
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public void ChangeState(NPCStateInterface newState)
    {
        StateMachine.ChangeState(newState);
    }

    public void SetTarget(Transform target)
    {
        this.Target = target;
    }
}
