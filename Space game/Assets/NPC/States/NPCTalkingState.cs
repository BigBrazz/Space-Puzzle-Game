using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkingState : NPCStateInterface
{
    private readonly NPCController npc;

    public NPCTalkingState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        npc.Movement.Stop();
    }

    public void Update() {}
    public void Exit() {}
}
