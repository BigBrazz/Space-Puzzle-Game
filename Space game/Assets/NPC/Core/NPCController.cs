using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("NPC Data")]
    [SerializeField] private NPCData npcData;

    // Components
    public NPCMovement Movement { get; private set; }
    public NPCDetection Detection { get; private set; }
    public NPCAnimation Animation { get; private set; }

    private NPCStateMachine StateMachine;
    public NPCData Data => npcData; 
    public Transform Target {get; private set; }

    private void Awake()
    {
        Movement = GetComponent<NPCMovement>();
        Detection = GetComponent<NPCDetection>();
        Animation = GetComponent<NPCAnimation>();

        StateMachine = new NPCStateMachine();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(new NPCIdleState(this));
    }

    // Update is called once per frame
    void Update()
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
