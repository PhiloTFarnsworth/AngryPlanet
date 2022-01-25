using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusState : BaseState
{
    //Expiration will track when we dequeue the status
    public int expiration;

    //TurnBehavior is what a status does on a per-turn basis, if it does anything at all.
    //I'm thinking we call this on stuff like poison or heal, where we call this behavior every couple
    //turns.
    public virtual void TurnBehaviour() {

    }

    public virtual void Enter() {

    }
    
    public virtual void Execute() {
    
    }
    
    public virtual void Exit() {

    }
}
