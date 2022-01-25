using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Again, borrowing off https://forum.unity.com/threads/c-proper-state-machine.380612/ the
//design proposed by Kelso 
public class StateMachine
{
    BaseState currentState;
    public void ChangeState(BaseState newState)
    {
        if (currentState != null)
            currentState.Exit();
 
        currentState = newState;
        currentState.Enter();
    }
 
    public void Update()
    {
        if (currentState != null) currentState.Execute();
    }
}
