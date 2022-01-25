using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DamageCloud : GameToken
{

    public override void Behaviour() {
        //Clouds should exist for a time, then disappear.  They shouldn't be able
        //to be damaged by players or mobs, but I suppose down the line clouds can
        //be used to neutralize other clouds, so we'll tie the disappearing to token 
        //health.
        var currentTile = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(coordinates);
        //Debug.Log("behaviour called.");
        foreach(GameObject token in currentTile) {
            if (token != this.gameObject) {
                if (token.GetComponent<Player>() == null) {
                    //Damage
                    if (token.GetComponent<Plant>() != null) {
                        token.GetComponent<Plant>().Damage(10);
                    } else if (token.GetComponent<Mob>() != null) {
                        token.GetComponent<Mob>().Damage(10);
                    }
                } else {
                    //Damage Player
                    token.GetComponent<Player>().Damage(10);
                    string attackEvent = token.GetComponent<Player>().name + " was terribly burnt for ";
                    var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
                    eventLog.AddEventLog(attackEvent + "10" + " damage!" );
                    var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
                    characterPanel.UpdateHealth(token);
                }
            }
        }
        //Destroy itself after 20 turns. 
        Damage(5);
    }

    public override void Damage(int damageAmt) {
        currentHealth = currentHealth - damageAmt;
        if (currentHealth < 1) {
            GameObject.Find("GameBoard").GetComponent<GameBoard>().AddDeadToken(this.coordinates, this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetHealth(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


