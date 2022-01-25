using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePrompt : BaseState
{
    //We're an interface, so grab everything in the initialization and pass it into our stuff.
    string message;
    string prompt;
    int messageSize;

    public GameObject title; 

    public TitlePrompt(string aMessage, string aPrompt, int aMessageSize) {
        message = aMessage;
        prompt = aPrompt;
        messageSize = aMessageSize;
    }

    public void Enter()
    {
        //Grab our current title card, pass in the strings for message and prompt.
        title = GameObject.Instantiate(AssetReference.title, new Vector3(0,0,0), Quaternion.identity);
        title.name = "TitlePrompt";
        var texts = title.GetComponentsInChildren<Text>();
        texts[0].text = message;
        texts[0].fontSize = messageSize;
        texts[1].text = prompt;
    }

    public void Execute() {
        //no Inputs, but we may do some sort of turn counter that we update as characters move.
    }

    public void Exit()
    {
        //destroy the indicator.
    }
}