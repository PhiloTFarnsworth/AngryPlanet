using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 
public class PlanetHUD : BaseState
{
    PlanetUIStack mainStack;

    public GameObject hud;

    public Player player;

    public PlanetHUD(GameObject thisPlayer)  {
        player = thisPlayer.GetComponent<Player>();
    }

    public void Enter() {
        //So when add this state, we instantiate our hud, marry it to our camera.
        hud = GameObject.Instantiate(AssetReference.hud, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        hud.transform.SetParent(Camera.main.transform, false);
        hud.name = "HUD";
    }

    public void Execute() {
        //We move our input, recently in the Player script, to here.  While on one level it feels wierd to incorporate all controls into
        //our heads up display, I think it will work well as most of our actions will be related to the HUD.  Mobile versions will be 
        //controlled exclusively by HUD buttons, and I think we can relate information through our controls (glowing directions) that
        //movement buttons on a desktop won't be intrusive, but will work as a compass.
        if (Input.anyKey) {
            //if (player == null) {
            //    player = GameObject.Find("Player").GetComponent<Player>();
            //}
            switch (Input.inputString) {
                case "9": case "e":
                    player.MoveWithCamera("NE");
                    break;
                case "6": case "d":
                    player.MoveWithCamera("E");
                    break;
                case "3": case "c":
                    player.MoveWithCamera("SE");
                    break;
                case "1": case "z":
                    player.MoveWithCamera("SW");
                    break;
                case "4": case "a":
                    player.MoveWithCamera("W");
                    break;
                case "7": case "q":
                    player.MoveWithCamera("NW");
                    break;
                default:
                    break;
            }
            //AbilityMode should be replaced by an ability select state.  TBD.
            // if (Player.AbilityMode == true) {
            //     var abilityButton = GameObject.Find("Ability1").GetComponent<AbilityButton>();
            //     var abilityCheck = abilityButton.GetComponentsInChildren<Text>();
            //     if (abilityCheck[0].text == "Teleport") {
            //         if (Input.GetKeyDown(KeyCode.Mouse0)) {
            //             Vector3 MouseToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            //             Vector3Int MouseCube = HexCoordinateUtility.OffsetToCube(GameObject.Find("GameBoard").GetComponent<GridLayout>().WorldToCell(MouseToWorld));
            //             player.TeleportWithCamera(MouseCube);
            //             Player.toggleAbilityMode();
            //             abilityButton.ResetCooldown();
            //             abilityButton.ChangeAbilityName();
            //         }
            //     } else {
            //         //do a fireball
            //         Vector3 MouseToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            //         Vector3Int MouseCube = HexCoordinateUtility.OffsetToCube(GameObject.Find("GameBoard").GetComponent<GridLayout>().WorldToCell(MouseToWorld));
            //         player.Fireball(MouseCube);
            //         Player.toggleAbilityMode();
            //         abilityButton.ResetCooldown();
            //         abilityButton.ChangeAbilityName();
            //     }
            // }
        }
    }

    public void Exit() {
        //Get rid of the hud.
        GameObject.Destroy(hud);
    }

}
