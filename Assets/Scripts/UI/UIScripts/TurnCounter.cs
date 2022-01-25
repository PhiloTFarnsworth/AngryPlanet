using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounter : BaseState
{

    public GameObject turnCounter;

    public void Enter() {
        turnCounter = GameObject.Instantiate(Resources.Load("Prefabs/UIfabs/TurnCounter") as GameObject, new Vector3(0,0,0), Quaternion.identity);
        
    }
    
    public void Execute() {
        turnCounter.GetComponentInChildren<Text>().text = "Turns:" + MyTimer.turnTimer;
    }
    
    public void Exit() {
        //Later Tater.
        UnityEngine.Object.Destroy(turnCounter);
    }
}
