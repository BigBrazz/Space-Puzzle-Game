using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdleState : NPCStateInterface
{
    private readonly NPCController npc;

    public NPCIdleState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        npc.Movement.Stop();
        //TODO make animations
        //npc.Animation.PlayIdle();
    }

    public void Update()
    {
        // Check if a player has been detected
        Transform detectedPlayer = npc.Detection.GetDetectedPlayer();
        if (detectedPlayer != null)
        {
            npc.SetTarget(detectedPlayer);
            npc.ChangeState(new NPCFollowTarget(npc));
            return;
        }
    }

    public void Exit() {}
}
