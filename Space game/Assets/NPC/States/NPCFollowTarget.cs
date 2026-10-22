using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollowTarget : NPCStateInterface
{
    private readonly NPCController npc;
    private Transform target;

    public NPCFollowTarget(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        target = npc.Target;
        if (target == null)
        {
            Debug.LogWarning("NPCFollowTarget: Target is null. Cannot follow.");
            npc.ChangeState(new NPCIdleState(npc));
            return;
        }
    }

    public void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("NPCFollowTarget: Target is null. Cannot follow.");
            npc.ChangeState(new NPCIdleState(npc));
            return;
        }
        
        Transform detectedPlayer = npc.Detection.GetDetectedPlayer();
        if (detectedPlayer == null)
        {
            npc.ChangeState(new NPCIdleState(npc));
            return;
        }

        npc.Movement.MoveTo(target.position);
    }

    public void Exit() {}
}
