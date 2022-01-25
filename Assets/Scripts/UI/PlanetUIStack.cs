using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetUIStack : MonoBehaviour
{
    //Okay, so our first experiment with implementing our Planetside UI as a stack.
    //The benefits of doing so allows us to tie player controls to the specific UI
    //chosen, while also being reasonably extensible to other scenes.
    public static StateStack ourStack = new StateStack();

    // Start is called before the first frame update
    void Awake()
    {
        //Push the opening text to screen.  TODO: Create an opening state, so players
        //can't move while story is being coroutined.
        //var player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //Do all that fun update stuff.
        ourStack.Update();
        //For the time being, we'll just list universal keys here.
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
