using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateMachine
{
    public NPCStateInterface CurrentState {get; private set;}

    public void ChangeState(NPCStateInterface newState)
    {
        if (newState == null) return;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    // Update is called once per frame
    public void Update()
    {
        CurrentState?.Update();
    }
}
