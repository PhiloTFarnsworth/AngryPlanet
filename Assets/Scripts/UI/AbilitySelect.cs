using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelect : BaseState
{

    public Player player;
    public void Enter() {
        //TODO: Create range mask over Game Board, to indicate tiles player can click.
    }

    public void Execute() {
        //TODO: Create Halo Effect on Selected Tile.
        if (Input.GetMouseButtonDown(0)) {
            if (player == null) {
                player = GameObject.Find("Player").GetComponent<Player>();
            }
            var abilityButton = GameObject.Find("Ability1").GetComponent<AbilityButton>();
            var abilityCheck = abilityButton.GetComponentsInChildren<Text>();
            
            Vector3 MouseToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector3Int MouseCube = HexCoordinateUtility.OffsetToCube(GameObject.Find("GameBoard").GetComponent<GridLayout>().WorldToCell(MouseToWorld));
            if (abilityCheck[0].text == "Teleport") {
                player.TeleportWithCamera(MouseCube);
            } else {
                //do a fireball
                player.Fireball(MouseCube);
            }
            abilityButton.ResetCooldown();
            abilityButton.ChangeAbilityName();
            PlanetUIStack.ourStack.Pop();
        }
    }

    public void Exit() {
        //Some cleaning
    }
}
