using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : StatusState
{
    public Mob owner;
    public Slow(Mob afflicted) {
        owner = afflicted;
    }
    public override void Enter() {
        expiration = MyTimer.turnTimer + 100;
        owner.SetMaxTurn(owner.GetMaxTurn() + 10);
    }

    public override void Execute() {

    }    
    public override void Exit() {
        owner.SetMaxTurn(owner.GetMaxTurn() - 10);
    }
}
