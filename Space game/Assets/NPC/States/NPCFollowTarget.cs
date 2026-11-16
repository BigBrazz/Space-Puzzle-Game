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
            npc.ChangeState(new NPCIdleState(npc));
            return;
        }
    }

    public void Update()
    {
        Transform detectedPlayer = npc.Detection.GetDetectedPlayer();
        if (detectedPlayer == null)
        {
            npc.ChangeState(new NPCIdleState(npc));
            return;
        }

        npc.Movement.MoveTo(detectedPlayer.position);
    }

    public void Exit() {}
}
