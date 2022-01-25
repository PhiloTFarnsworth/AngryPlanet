using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer : MonoBehaviour
{
    // We need a flag that let's the game know we've moved.  Important to note
    // That this only works for single entities, like a player or the gameboard.
    // On the birght side, we'll only advance time when the player acts, so not a
    // huge deal.
    public static bool turnTaken = false;
    public static bool playerNameChanged = false;
    public static float timer = 0.0f;
    public static int turnTimer = 1;
    
    public static void TakeTurn(int turnLength) {
        turnTaken = true;
        turnTimer = turnTimer + turnLength;
        var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
        characterPanel.UpdateTurn();
        var AbilityButton = GameObject.Find("Ability1").GetComponent<AbilityButton>();
        AbilityButton.DecrementCooldown(turnLength);
        //Debug.Log(turnTimer);
    }

    public static void ClearTurn() {
        turnTimer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //Debug.Log(timer);
    }
}
