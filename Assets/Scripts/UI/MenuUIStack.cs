using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuUIStack : MonoBehaviour
{
    //We can again abstract our Start menu with something we're going to call menu
    //UI stack.  We're going to refactor into the planetUIStack once we're a little 
    //more comfortable.
    public static StateStack ourStack = new StateStack();

    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        //Push the opening text to screen.  TODO: Create an opening state, so players
        //can't move while story is being coroutined.
        //ourStack.Push(new PlanetHUD(this));
        sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName) {
            case "Title":
                ourStack.Push(new TitlePrompt(
                    "Goo Gods: Angry Planet",
                    "Press Enter",
                    48
                ));
                break;
            case "GameOver":
                ourStack.Push(new TitlePrompt(
                    "Game Over!",
                    "Obtanium won't gather itself",
                    48
                ));
                break;
            case "Win":
                ourStack.Push(new TitlePrompt(
                    "The captain beams you back aboard the ship.  You've earned your keep... for now.",
                    "Press Enter to Continue",
                    24
                ));
                break;
            default:
                ourStack.Push(new TitlePrompt(
                    "You did an unexpected thing...",
                    "Press Enter",
                    48
                ));
                break;
        }
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
        //All roads lead to rome, for the time being at least.
        if (Input.GetAxis("Submit") == 1) {
			SceneManager.LoadScene("Game");
		}
    }
}
