using System.Collections;
using System.Collections.Generic; 
using UnityEngine;


//Our build of a StateStack, to allow us to interact with the topmost state while
//preserving everything below.  As of now, mostly useful to create menus and pop-ups.
public class StateStack
{
    Stack<BaseState> States = new Stack<BaseState>();

    //Always nice to have a built in stack data type.  We'll use this to layer menus,
    //so if we are on a certain prompt, we can put our normal controls on hold, then
    //restore them once the player pops the dialogue.
    public void Push(BaseState state) {
        States.Push(state);
        state.Enter();
    }

    public void Pop() {
        //This might be tricky, we'll see what happens when we pop.
        if (States.Count > 0) {
            var toDestroy = States.Pop();
            toDestroy.Exit();
        }
    }
    public void Update() {
        if (States.Count > 0) {
            BaseState currentState = States.Peek();
            currentState.Execute();
        }
    }
}
