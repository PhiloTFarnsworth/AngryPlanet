using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAI : Mob
{
    public override void Behaviour() {
        //What does a mob do on a turn?  Well, if they are adjacent to a player, they should attack.
        //Failing that, they should check if a player is within x tiles.  If so, path towards player.
        //Finally, with no local player, they should wander onto neighboring non-solid tiles.
        Dictionary<string, Vector3Int> neighbors = GetNeighbors();
        bool attacked = false;
        //While we check for the player, we should also check for possible walk moves.
        List<string> possibleMoves = new List<string>();
        foreach(KeyValuePair <string, Vector3Int> neighborTile in neighbors) {
            var activeTiles = GameObject.Find("GameBoard").GetComponent<GameBoard>();
            if (activeTiles.TrackedTile(neighborTile.Value) == true) {
                HashSet<GameObject> tokenList = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(neighborTile.Value);
                foreach(GameObject token in tokenList) {
                    //Debug.Log(token);
                    if (token.GetComponent<Player>() != null) {
                        //Debug.Log("Hit Player");
                        Attack(token, strength);
                        attacked = true;
                    }
                }
                if (activeTiles.CheckSolid(neighborTile.Value) == false) {
                    possibleMoves.Add(neighborTile.Key);
                }
            }
        }
        
        //Debug.Log(this);
        //Debug.Log(possibleMoves.Count);
        if (attacked == false) {
            if (possibleMoves.Count > 0) {
                //Debug.Log("I tried to move");
                var moveRoll = Random.Range(0, possibleMoves.Count);
                var direction = possibleMoves[moveRoll];
                StopCoroutine("walk");
                transform.position = WalkToken(coordinates, direction);
                StartCoroutine("walk");
            }
        }
        possibleMoves.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        RenderMob();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
