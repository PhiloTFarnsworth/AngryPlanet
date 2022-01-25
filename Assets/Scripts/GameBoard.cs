using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    //GameBoard holds the world coordinates in a convenient place.
    public static GridLayout Map;

    //tokenLegend will track any spawned tokens for convenient lookup.  We could use
    //Gridlayout PositionProperties, but I feel like it would be easier to check tokenLegend[coords][i]
    //as opposed to GetGridlayout(coords, objectname, attribute).  If we end up with custom tiles that 
    //are not tokens, it's possible, but I feel like we're better served keeping tiles super simple and
    //doing all modifications through our token system.

    private Dictionary<Vector3Int, HashSet<GameObject>> tokenLegend = new Dictionary<Vector3Int, HashSet<GameObject>>();
    private Dictionary<Vector3Int, HashSet<GameObject>> DepartedTokens = new Dictionary<Vector3Int, HashSet<GameObject>>();

    private Vector3Int GenerationPoint = new Vector3Int(0,0,0); 

    //We seperated solids from the token map because we were getting some wierd bugs, but with this refactor we're going to
    //toss the solidmap and just read from the token map itself.  Besides cutting off some length, we want to have gameboard
    //be a singular source of truth.
    public bool CheckSolid(Vector3Int Coordinates) {
        bool solid = false; 
        if (tokenLegend.TryGetValue(Coordinates, out HashSet<GameObject> values)) {
            foreach(GameObject token in tokenLegend[Coordinates]) {
                if (token.GetComponent<GameToken>().isSolid == true) {
                    solid = true;
                }
            }
        } else {
            tokenLegend.Add(Coordinates, new HashSet<GameObject>());
        }
        return solid;
    }

    //Add and remove tokens from our tokenLegend
    public void AddToken(Vector3Int Coordinates, GameObject agameObject) {
        if (tokenLegend.TryGetValue(Coordinates, out HashSet<GameObject> values)) {
            tokenLegend[Coordinates].Add(agameObject);
        } else {
            tokenLegend.Add(Coordinates, new HashSet<GameObject>());
            tokenLegend[Coordinates].Add(agameObject);
        }
    }
    //Removes a single gameobject from the map
    public void RemoveToken(Vector3Int Coordinates, GameObject agameObject) {
        tokenLegend[Coordinates].Remove(agameObject);
    }

    //Clear multiple tokens from the map, used when we delete a tile from generation.
    public void ClearTokens(Vector3Int Coordinates) {
        if (tokenLegend.TryGetValue(Coordinates, out HashSet<GameObject> values)) {
            tokenLegend.Remove(Coordinates);
        }
    }

    public bool TrackedTile(Vector3Int Coordinates) {
        if (tokenLegend.TryGetValue(Coordinates, out HashSet<GameObject> values)) {
            return true;
        } else {
            return false;
        }
    }

    public void ClearBoard() {
        foreach (KeyValuePair<Vector3Int, HashSet<GameObject>> legend in tokenLegend)
        {
            AddDeadTokenList(legend.Key, legend.Value);
        }
    }

    //Return the list of game objects at a give coordinate.
    public HashSet<GameObject> ReadCoordinates(Vector3Int Coordinates) {
        if (tokenLegend.TryGetValue(Coordinates, out HashSet<GameObject> values)) {

        } else {
            tokenLegend.Add(Coordinates, new HashSet<GameObject>());
        }
        return tokenLegend[Coordinates];
    }
    //Dictionary of our destroyed tokens for clean up.
    public void AddDeadToken(Vector3Int Coordinates, GameObject agameObject) {
        if (DepartedTokens.TryGetValue(Coordinates, out HashSet<GameObject> values)) {
            DepartedTokens[Coordinates].Add(agameObject);
        } else {
            DepartedTokens.Add(Coordinates, new HashSet<GameObject>());
            DepartedTokens[Coordinates].Add(agameObject);
        }
    }

    public void AddDeadTokenList(Vector3Int Coordinates, HashSet<GameObject> gameObjectList) {
        if (DepartedTokens.TryGetValue(Coordinates, out HashSet<GameObject> values)) {
            DepartedTokens[Coordinates] = gameObjectList;
        } else {
            DepartedTokens.Add(Coordinates, new HashSet<GameObject>());
            DepartedTokens[Coordinates] = gameObjectList;
        }
    }
    public void RemoveDeadTokens(Vector3Int Coordinates) {
        DepartedTokens.Remove(Coordinates);
    }

    //Giving our little critters the ability to move means they could move past the borders of our generated map.
    //Pass a list of coordinates that you want active, and this will weed out the unbounded tokens.  We'll also
    //Access the tilemap and null out the ground tile.
    public void ClearUnboundedTokens(List<Vector3Int> CoordinatesList) {
        foreach(KeyValuePair <Vector3Int, HashSet<GameObject>> legend in tokenLegend) {
            if (CoordinatesList.Contains(legend.Key)) {
                //Cool
            } else {
                //Not cool
                AddDeadTokenList(legend.Key, legend.Value);
                Vector3Int offPosition = HexCoordinateUtility.CubeToOffset(legend.Key);
                GameObject.Find("GameBoard").GetComponent<LevelGenerator>().TileMap.SetTile(offPosition, null);
            }
        }
    }

    //We'll run clean up at the end of every turn increment.  This just checks if we have any dead tokens or 
    //tiles outside our generation area, safely removes them from the tokenLegend and destroys them.
    public void CleanUp() {
        if (DepartedTokens.Count > 0 && tokenLegend.Count > 0) {
            List<GameObject> toDestroy = new List<GameObject>();
            List<Vector3Int> deadCoords = new List<Vector3Int>();
            List<Vector3Int> ActiveTiles = HexCoordinateUtility.HexRange(GameObject.Find("Player").GetComponent<Player>().coordinates, LevelGenerator.GenerationDistance);
            foreach (KeyValuePair<Vector3Int, HashSet<GameObject>> deadObjectList in DepartedTokens)
            {
                if (ActiveTiles.Contains(deadObjectList.Key)) {
                    foreach(GameObject deadObject in deadObjectList.Value) {
                        toDestroy.Add(deadObject);
                        deadCoords.Add(deadObjectList.Key);
                        RemoveToken(deadObjectList.Key, deadObject);
                    }
                } else {
                    foreach(GameObject deadObject in deadObjectList.Value) {
                        toDestroy.Add(deadObject);
                        deadCoords.Add(deadObjectList.Key);
                    }
                    ClearTokens(deadObjectList.Key);
                }
            }
            //Debug.Log(toDestroy);
            //Go backwards to not deal with index shifting
            for (int i = toDestroy.Count - 1; i > -1 ; i--) {
                if (DepartedTokens.TryGetValue(deadCoords[i], out HashSet<GameObject> values)) {
                    RemoveDeadTokens(deadCoords[i]);
                }
                //Debug.Log(i);
                var onDeck = toDestroy[i];
                toDestroy.RemoveAt(i);
                //Debug.Log(onDeck);
                try
                {
                    if (onDeck.GetComponent<Player>() == null) {
                        Destroy(onDeck);
                    }   
                }
                catch (System.Exception)
                {
                    Debug.Log("Nothing to destroy");   
                    throw;
                }
            }
            toDestroy.Clear();
            deadCoords.Clear();
            DepartedTokens.Clear();

            //This shouldn't be necessary, but we're getting strange conditions where nulls are hanging onto our
            //token legend.  For the time being though, I think this ~should~ solve the phantom tokens.  The most 
            //recent example of this was found very rarely as Damage Clouds decayed, as well as occuring when player
            //is significantly slowed (2 slow status), and usually in tandem with Damage clouds.
            foreach(KeyValuePair<Vector3Int, HashSet<GameObject>> legend in tokenLegend) {
                legend.Value.Remove(null);
            }
        }
    }

    //We've been running take turn through our timer, passing the move and level generation through our level generator and
    //passing all the details over to our gameboard.  That's no bueno.  We're going to move take turn to the GameBoard, call
    //any level generation functions from Level Generator, and do all our updates to the game board here.  This should both 
    //significantly reduce our update logic and reduce errors in updating our gameboard.

    //This is also an interesting juncture where we can create more expressiveness between turns.  If we process turns every tenth
    //of a turn, then this should yield more clarity on who moved when, and allow players better insight into what turn the opposing
    //mobs will move.  Since turns are practically a resource on their own, more granularity and detail should benefit the tactical
    //element of gameplay.
    public IEnumerator TakeTurn() {
        PlanetUIStack.ourStack.Push(new TurnCounter());
        int turnIncrement = 1;
        Player player = GameObject.Find("Player").GetComponent<Player>(); 
        //Since players don't have behaviors, we can run NextTurn to reset the Player's NextTurn value to their MaxTurn.
        player.NextTurn(turnIncrement);
        //So the idea here is we'll run the logic until it's the player's next turn.
        while (player.GetNextTurn() != 0) {
            //first, We'll push a turn indicator to our to our PlanetUIStack. This Both locks player input and indicates the turn.
            
            //Move all Tokens, since we're doing this in small jumps, we can just call next turn without tracking whether tokens
            //need to move twice, as opposed to before where that could happen.  I feel kinda silly that this wasn't in the first
            //implementation.
            List<GameObject> moversAndShakers = new List<GameObject>();
            foreach(KeyValuePair<Vector3Int, HashSet<GameObject>> tileContents in tokenLegend) {
                foreach(GameObject token in tileContents.Value) {
                    Debug.Log(token);
                    if (token.GetComponent<GameToken>().GetNextTurn() == 0) {
                        //add the token to the movelist
                        moversAndShakers.Add(token);
                    } else {
                        token.GetComponent<GameToken>().NextTurn(turnIncrement);
                    }
                    //Eventually we'll update status queue here, but it is not this day.
                        // if (token.GetComponent<Mob>() != null) {
                        //     if (token.GetComponent<Mob>().statusQueue.states.Count > 0) {
                        //         StatusState status = token.GetComponent<Mob>().statusQueue.Peek();
                        //         if (status.expiration <= MyTimer.turnTimer) {
                        //             token.GetComponent<Mob>().statusQueue.Dequeue();
                        //         }
                        //     }
                        // }
                }
            }
            foreach(GameObject token in moversAndShakers) {
                RemoveToken(token.GetComponent<GameToken>().coordinates, token);
                token.GetComponent<GameToken>().NextTurn(turnIncrement);
                AddToken(token.GetComponent<GameToken>().coordinates, token);
            }
            //Now we need to check if we need to run any level generation
            if (player.coordinates != GenerationPoint) {
                //Do Generation
                List<Vector3Int> activeTiles = new List<Vector3Int>(tokenLegend.Keys);
                GameObject.Find("GameBoard").GetComponent<LevelGenerator>().GenerateTiles(activeTiles, player.coordinates);
                GenerationPoint = player.coordinates;
            }
            //This is a little goofy, but instead of updating we'll just destroy our turncounter object, then push a new one.  We should
            //look up later what the performance implications are.
            
        
            //Update our turn various turn counters elsewhere.
            MyTimer.turnTimer = MyTimer.turnTimer + turnIncrement;
            GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>().UpdateTurn();
            GameObject.Find("Ability1").GetComponent<AbilityButton>().DecrementCooldown(turnIncrement);
            CleanUp();
            yield return new WaitForSeconds(.05f);
        }
        //Cleanup for wayward tokens.  Since we're locking player input between turns, it doesn't matter as much if we distort the map, then perform
        //our cleanup at the end of the take turn cycle.
        List<Vector3Int> wantedTiles =  HexCoordinateUtility.HexRange(player.coordinates, LevelGenerator.GenerationDistance);
        ClearUnboundedTokens(wantedTiles);
        CleanUp();
        PlanetUIStack.ourStack.Pop();
    }


    void awake() 
    {
        Map = GameObject.Find("GameBoard").GetComponent<GridLayout>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Map = GameObject.Find("GameBoard").GetComponent<GridLayout>();
    }

    // Update is called once per frame
    void Update()
    {
        //I think this'll work well.
        
    }
}
