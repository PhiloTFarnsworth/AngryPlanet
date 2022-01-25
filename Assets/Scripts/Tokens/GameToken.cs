using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameToken : MonoBehaviour
{
    //My code is getting real messy, so we need to start organizing a bit better.  That's
    //Where Game Token comes in.  GameToken will be the base object of anything we spawn 
    //into our playable world.  They have a coordinate, a type and hitbox.  We then assign
    //the details of the appropriate object template and we have a winner.
    public Vector3Int coordinates;

    //Since physics aren't really necessary in this grid implementation, it would instead 
    //be more helpful if we could just return the Token's immediate neighbors.  When we 
    //move or interact, we can call that data before acting.  Updates once per turn.
    private Dictionary<string, Vector3Int> neighbors = new Dictionary<string, Vector3Int>();

    
    //nextTurn will track how we order our tokens on the game board.  For static units, we'll
    //just set to -1, otherwise, it will be some increment of a turn.  When turns occur, we'll
    //adjust turn values based on the turn elapsed (for non -1 units).  If nextTurn is less than 
    //Zero, we add them to the turn queue, process, then set the nextTurn back to their next turn
    //value, adding the overflow from the previous turn.
    public int nextTurn;
    public int maxTurn;

    //Solid will determine whether a token can share space with another token.  Notable uses are
    //for non-fully grown plants and healing mists.
    public bool isSolid;

    //After some internal debate, all tokens should have a health variable.  Although some may not use
    //it.  But I think you force the player into choices if, for instance, an item spawns and an enemy
    //spawns pools acid on it.  If the item will despawn after being damaged, then the player has to wiegh
    //defeating the enemy or getting the item first.  
    public int MaxHealth;
    public int currentHealth;

    //Generic items don't have defense  
    public int defense = 0;

    public bool isConsumable = false;

    //Set the pertinent values, the create the object.  Comes down to personal preference, but
    //It should look cleaner if we take the Cube Coordinates.
    public void SetToken(string tokenName, Vector3Int tokenCoordinates, int newMaxTurn, bool Solid) {
        name = tokenName;
        coordinates = tokenCoordinates;
        maxTurn = newMaxTurn;
        nextTurn = newMaxTurn;
        isSolid = Solid;
    }

    public void SetMaxTurn(int newMaxTurn) {
        maxTurn = newMaxTurn;
        nextTurn = newMaxTurn;
    }

    public void SetDefense(int newDefense) {
        defense = newDefense;
    }

    //We'll have a generic initializer to set health
    public void SetHealth(int health) {
        MaxHealth = health;
        currentHealth = health;
    }

    public void SetConsumable(bool choice) {
        isConsumable = choice;
    }

    public bool GetConsumable() {
        return isConsumable;
    }

    public int GetMaxHealth() {
        return MaxHealth;
    }

    public int GetCurHealth() {
        return currentHealth;
    }

    public int GetDefense() {
        return defense;
    }

    public int GetMaxTurn() {
        return maxTurn;
    }

    public int GetNextTurn() {
        return nextTurn;
    }

    public Vector3Int GetCoordinates() {
        return coordinates;
    }

    public Dictionary<string, Vector3Int> GetNeighbors() {
        return neighbors;
    }

    public bool IsSolidToken() {
        return isSolid;
    }

    //Get to the cool methods.  Since the effects of falling below 0 will be different for each token, we'll put that logic there.
    public virtual void Damage(int damageAmt) {
        currentHealth = currentHealth - damageAmt;
    }

    //We'll inherit Behavior into each token class that does anything, and call it when it's time for the token
    //to take its move.
    public virtual void Behaviour() {

    }

    //returns true if a token takes a move.
    public void NextTurn(int turnValue) {
        if ((nextTurn - turnValue) < 0 ) {
            
            //Grab the remaining turn duration
            var remainingTurn = turnValue - nextTurn;
            //subtract the turn from next turn
            nextTurn = nextTurn - turnValue;

            //RecalculateNeighbors before we decide how we act.
            RecalculateNeighbors();
            //Do a thing.
            Behaviour();
            
            //reset turn to max turn and add our overflow
            nextTurn = maxTurn + nextTurn;

            //If we have a sufficiently fast unit, recursively call NextTurn, but starting
            //at the Maxturn, using our remaining turn duration.
            if (nextTurn < 0) {
                nextTurn = maxTurn;
                NextTurn(remainingTurn);
            }
        } else {
            //Not our turn.
            nextTurn = nextTurn - turnValue;
        }
    }

    //Much like walk, except we aren't passing in a direction, but a whole destination.
    public Vector3 Teleport(Vector3Int destination) {
        var newCoord = coordinates;
        if (GameObject.Find("GameBoard").GetComponent<GameBoard>().CheckSolid(destination) == false) {
            newCoord = destination;
        } else {
            var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
            eventLog.AddEventLog("You don't want to teleport there.");
        }
        coordinates = newCoord;
        var worldPosition = GameObject.Find("GameBoard").GetComponent<GridLayout>().CellToWorld(HexCoordinateUtility.CubeToOffset(newCoord));
        return worldPosition;
    }

    //We take the current cubic location and direction, then return the worldposition of the move.  
    //We should merge this with Teleport?  Maybe.
    public Vector3 WalkToken(Vector3Int Location, string Direction) {
        Vector3Int NewCoord;
        if (GameObject.Find("GameBoard").GetComponent<GameBoard>().CheckSolid((Location+HexCoordinateUtility.cubeDirections[Direction])) == false) { 
            NewCoord = Location + HexCoordinateUtility.cubeDirections[Direction];
        } else {
            NewCoord = Location;
        }
        var worldPosition = GameObject.Find("GameBoard").GetComponent<GridLayout>().CellToWorld(HexCoordinateUtility.CubeToOffset(NewCoord));
        coordinates = NewCoord;
        return worldPosition;
    }

    public void ToggleSolid() {
        if (isSolid == true) {
            isSolid = false;
        } else {
            isSolid = true;
        }
    }

    public void RecalculateNeighbors() {
        neighbors["NE"] = HexCoordinateUtility.cubeDirections["NE"] + coordinates;
        neighbors["E"] = HexCoordinateUtility.cubeDirections["E"] + coordinates;
        neighbors["SE"] = HexCoordinateUtility.cubeDirections["SE"] + coordinates;
        neighbors["SW"] = HexCoordinateUtility.cubeDirections["SW"] + coordinates;
        neighbors["W"] = HexCoordinateUtility.cubeDirections["W"] + coordinates;
        neighbors["NW"] = HexCoordinateUtility.cubeDirections["NW"] + coordinates;
    }

    //This creates a GameObject from an already present game object.
    public GameObject Spawn(GameObject prefab, Vector3Int HexCoord) {
        Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
        Vector3 WorldPosition = GameBoard.Map.CellToWorld(OffPosition);
        GameObject spawn = Instantiate(prefab, WorldPosition, Quaternion.identity);
        
        //spawn's parent should be the same as the parent of the token that spawned it.
        spawn.transform.parent = this.transform.parent.transform;
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(HexCoord, spawn);
        return spawn;
    }

    // Start is called before the first frame update.
    void Start()
    {
        RecalculateNeighbors();
    }

    // Update is called once per frame
    void Update()
    {   

    }
}
