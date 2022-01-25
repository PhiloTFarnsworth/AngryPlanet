using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://forum.unity.com/threads/c-proper-state-machine.380612/  Basing our General purpose
//state machine on Kelso's posts.  While I'm still super unclear on what the difference
//between an abstract and interface exactly is, I can appreciate the simple styling.  
public interface BaseState
{
    //I'm not the most knowledgable in state machines, so let's rundown what we're going to
    //put where:

    //Enter will have all our initializing details.  We'll create objects from prefabs, start
    //any idle animations, ect.
    void Enter();
    //We're going to call Execute during our updates.  As this is a turn based game, the
    //only Executes we should see should be for player input.
    void Execute();
    //Whatever extra destruction stuff we need.
    void Exit();
}
