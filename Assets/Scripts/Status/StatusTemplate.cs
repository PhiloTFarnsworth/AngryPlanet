using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusTemplate : StatusState
{

    public Mob owner;
    public StatusTemplate(Mob afflicted) {
        owner = afflicted;
    }
    public override void Enter() {
        expiration = MyTimer.turnTimer + 100;
        //Buff Here
    }

    public override void Execute() {

    }    
    public override void Exit() {
        //Unbuff here
    }
}
