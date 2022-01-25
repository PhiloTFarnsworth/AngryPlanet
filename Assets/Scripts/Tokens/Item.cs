using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : GameToken
{
    //So this class represents an item as it is displayed on the map.  Items are
    //consumable tokens which all mobs should be able to pick up.  In the current
    //iteration, we're likely just going to have a currency the player picks up, but
    //in the future, I'd like have a small armor/weapon system for vanilla attacks,
    //then a set of items which replicate abilities a number of times before disappearing.
    //So what makes a generic Item?  Well, on init, it should be a consumable token and it
    //should also not be a solid tile.  It should also have a name.  As for item specific 
    //behavior, we should have a virtual consume function.  We also will probably want 
    //animation cooroutines related to consumption and idle.  

    //Hardcoding a little crown animation for submission.
    private int[] ItemAnimation = {
        181,
        182,
        183,
        184,
        185,
        186,
    };

    private SpriteRenderer crown;

    IEnumerator idle() {
        for (int i = 0; i < ItemAnimation.Length; i++){
            crown.sprite = AssetReference.MainCreature[5][i + 181];
            if (i == ItemAnimation.Length - 1) {
                i = 0;
            }
            yield return new WaitForSeconds(.2f);            
        }
    }

    public void Init() {
        SetConsumable(true);
    }

    //With multiple items, we'd make this less precise, but for the moment we'll just increment
    //the player's points.
    public virtual void OnConsume() {
        var Player = GameObject.Find("Player").GetComponent<Player>();
        Player.IncPoints();
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddDeadToken(this.coordinates, this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {   
        crown = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        StartCoroutine("idle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
