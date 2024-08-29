using System;
using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

public class FSM : MonoBehaviour
{
    public BaseState state;

    public void ChangeState(BaseState nextState)
    {
        if (state != null)
        {
            state.OnStateExit();
        }

        state = nextState;
        state.OnStateEnter();
    }

    private void Update()
    {
        if (state != null)
        {
            state.OnStateUpdate();
        }
    }

    public PlayerState.PlayerState GetState()
    {
        return state.GetState();
    }
}
