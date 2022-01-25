using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLog : MonoBehaviour
{
    public Text[] textAreas;
    public string[] startText = {
        "'I need more obtanium!' screeched the captain as he locks you in the teleporter's containment field",
        "As the machine begins to hum, he scolds you 'Pick up the sparking rocks, or you'll need to find'",
        "'another ride off this forsaken rock'. He pushes a button and the world shimmers and flashes.",
        "Suddenly, you find yourself on the surface. Crash! A wall of hostile plants shoots from the ground.",
        "Better move fast, this is one angry planet...",
    };

    public void AddEventLog(string Event) {
        string newEvent = "Turn " + ((int)MyTimer.turnTimer) + ": " + Event;
        //Debug.Log(newEvent);
        for (int i = textAreas.Length - 1; i > -1; i--) {
            if (i == 0) {
                textAreas[i].GetComponent<Text>().text = newEvent;
            } else {
                textAreas[i].GetComponent<Text>().text = textAreas[i-1].text;
            }
        }
    }

    //We'll use a coroutine to start a level, which informs the player they are
    //on the alien planet, that they need to pick up the sparking rocks, and they
    //aren't there to make friends.
    IEnumerator StartScroll() {
        var player = GameObject.Find("Player").GetComponent<Player>();
        GetComponent<AudioSource>().Play();
        player.togglePlayerLock();
        for (int i=0; i < startText.Length; i++){
            AddEventLog(startText[i]);
            //Some sort of sound effect.
            yield return new WaitForSeconds(1f);
        }
        player.togglePlayerLock();
        GetComponent<AudioSource>().Pause();
        player.GetComponentsInChildren<ParticleSystem>()[2].Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartScroll");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
