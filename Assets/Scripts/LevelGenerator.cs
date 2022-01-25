using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    //UNITY EDITOR STUFF
    //These are blank objects just to organize our workflow.
    private GameObject plantPrototype;
    private GameObject mobPrototype;
    private GameObject itemPrototype;

    //Counters, just to disambiguate objects created.
    private static int plantsCreated = 0;
    private static int mobsCreated = 0;
    private static int itemsCreated = 0;
    //END UNITY EDITOR STUFF

    private GridLayout gameMap;

    // Since we're going to use our Hex Range calculator, we'll need a configurable Generation distance
    public static int GenerationDistance = 15;

    public Tilemap TileMap;

    private GameObject player;

    //The prototype animated tile.  We probably should just move this to Asset Reference and create our tiles at runtime,
    //but not at this exact moment.
    public AnimatedTile GroundTile;

    // We'll get our tiles and paint the ground just to demonstrate.  This list will also control how we create new tiles
    // and delete old ones.
    private Vector3Int GenerationPoint = new Vector3Int(0,0,0); 

    public static int level;

    public static int GetPlantsCreated() {
        return plantsCreated;
    }

    public static void IncPlantsCreated() {
        plantsCreated++;
    }

    private void PlacePlant(Vector3Int HexCoord, int growthLevel) {
        Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
        Vector3 WorldPosition = gameMap.CellToWorld(OffPosition);
        GameObject Growth = Instantiate(AssetReference.Prefabs["Plant"], WorldPosition, Quaternion.identity);
        var aPlant = Growth.GetComponent<Plant>();
        Growth.transform.parent = plantPrototype.transform;
        var newName = "Plant" + plantsCreated;
        aPlant.SetToken(newName, HexCoord, 1, false);
        aPlant.Init(new List<int> {-1}, growthLevel, false);
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(HexCoord, Growth);
        plantsCreated++;
    }

    private void PlaceMob(Vector3Int HexCoord) {
        Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
        Vector3 WorldPosition = gameMap.CellToWorld(OffPosition);
        GameObject mob = Instantiate(AssetReference.Prefabs["Mob"], WorldPosition, Quaternion.identity);
        mob.transform.parent = mobPrototype.transform;
        var newName = "Mob" + mobsCreated;
        mob.GetComponent<MobAI>().SetToken(newName, HexCoord, 1, true);
        mob.GetComponent<MobAI>().Init(Random.Range(2,6), new List<int> {-1}, Color.white);
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(HexCoord, mob);
        mobsCreated++;
    }

    private void PlaceItem(Vector3Int HexCoord) {
        Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
        Vector3 WorldPosition = gameMap.CellToWorld(OffPosition);
        GameObject item = Instantiate(AssetReference.Prefabs["Item"], WorldPosition, Quaternion.identity);
        item.transform.parent = itemPrototype.transform;
        var newName = "Unobtanium" + itemsCreated;
        item.GetComponent<GameToken>().SetToken(newName, HexCoord, 1, false);
        item.GetComponent<Item>().Init();
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(HexCoord, item);
        itemsCreated++;
    }

    private void PlaceTile(Vector3Int HexCoord) {
        //Translate our cube coordinates and place the tile.
        Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
        Vector3 WorldPosition = gameMap.CellToWorld(OffPosition);

        int levelRange = (level%8) * 4;

        //Choose a random tile from the starting tiles.
        int SpriteChoice = Random.Range(levelRange, levelRange + 4);
        //create our animation arrays
        Sprite[] Animation = new Sprite[8];
        //Choose the animation set
        for (int i = 0; i < 8; i++) {
            Animation[i] = (AssetReference.AnimatedTiles[SpriteChoice*8+i]);
        }
        
        //Use our created AnimatedTile to create a new one
        AnimatedTile newTile = GroundTile;
        newTile.m_AnimatedSprites = Animation;

        //Set that tile.
        TileMap.SetTile(OffPosition, newTile);
        //ReadCoordinates doubles as an initializer for our gameboard.
        GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(HexCoord);
        
        //Rotate Tile to increase variety.  Scale down as well.
        int rotation = Random.Range(0, 6);
        TileMap.SetTransformMatrix(OffPosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f,0f, rotation*60f), new Vector3(.67f,.67f,0)));
    }

    //Create Player -- This is totally gross, big switch is terrible.  We'll come back to this.
    private void PlacePlayer() {
        
        player = Instantiate(AssetReference.Prefabs["Player"], gameMap.CellToWorld(HexCoordinateUtility.CubeToOffset(GenerationPoint)), Quaternion.identity);
        player.transform.parent = gameMap.transform;
        player.GetComponent<Player>().SetToken("Player", GenerationPoint, 1, true);
        //If DNA1 == -1, that means we want to spawn a fresh player, otherwise, load our data.
        if (PlayerPrefs.GetInt("DNA1") == -1) { 
            player.GetComponent<Player>().Init(2, new List<int> {-1}, Color.white);
            player.GetComponent<Player>().SaveDna(player.GetComponent<Mob>().GetDna());
            switch(player.GetComponent<Mob>().GetColor()) {
                case "red": PlayerPrefs.SetString("Color", "red");
                break;
                case "yellow": PlayerPrefs.SetString("Color", "yellow");
                break;
                case "green": PlayerPrefs.SetString("Color", "green");
                break;
                case "blue": PlayerPrefs.SetString("Color", "blue");
                break;
                case "magenta": PlayerPrefs.SetString("Color", "magenta");
                break;
                default: PlayerPrefs.SetString("Color", "white");
                break;
            }
        } else {
            List<int> readDna = new List<int>();
            readDna.Add(PlayerPrefs.GetInt("DNA1"));
            readDna.Add(PlayerPrefs.GetInt("DNA2"));
            Color playerColor;
            switch(PlayerPrefs.GetString("Color")) {
                case "red": playerColor = Color.red;
                break;
                case "yellow": playerColor = Color.yellow;
                break;
                case "green": playerColor = Color.green;
                break;
                case "blue": playerColor = Color.blue;
                break;
                case "magenta": playerColor = Color.magenta;
                break;
                default: playerColor = Color.white;
                break;
            }
            player.GetComponent<Player>().Init(2, readDna, playerColor);
        }
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(GenerationPoint, player);
        PlanetUIStack.ourStack.Push(new PlanetHUD(player));
        var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
        characterPanel.SetCharacterPanel(player);
    }

    public void GenerateTiles(List<Vector3Int> activeTiles, Vector3Int generationPoint) {
        List<Vector3Int> newTiles = HexCoordinateUtility.HexRange(generationPoint, GenerationDistance); 
        foreach (Vector3Int hexCoord in newTiles) {
            Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(hexCoord);
            Vector3 WorldPosition = GameObject.Find("GameBoard").GetComponent<GridLayout>().CellToWorld(OffPosition);
            if (activeTiles.Contains(hexCoord)) {
                //We've already done generation here.
            } else {
                // These Tiles are new.
                PlaceTile(hexCoord);
                if (OffPosition.y < -3 + player.GetComponent<Mob>().GetTilesTraveled() / 5 && OffPosition.y > -8) {
                    PlacePlant(hexCoord, 4);
                } else {
                    if (Random.Range(0,20) < 1) {
                        PlacePlant(hexCoord, Random.Range(1,3));
                    } else {
                        if (Random.Range(0,16) < 1) {
                            PlaceMob(hexCoord);
                        } 
                        if (Random.Range(0,25) < 2) {
                            PlaceItem(hexCoord);
                        }
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Associate our level with the underlying GameBoard.
        gameMap = GameObject.Find("GameBoard").GetComponent<GridLayout>();
        
        //Clear our TileMap to make room for the real board.  We may actually build a crash/starting site for consistancy,
        //In which case we can remove this.
        TileMap.ClearAllTiles();

        //Unity editor organization stuff
        //Create an uninitialized main plant to attach our generated plants to. 
        plantPrototype = Instantiate(AssetReference.Prefabs["Blank"], gameMap.CellToWorld(HexCoordinateUtility.CubeToOffset(GenerationPoint)), Quaternion.identity);
        plantPrototype.transform.parent = gameMap.transform;
        plantPrototype.name = "Plants";


        //Create an uninitialized main mob to attach to.
        mobPrototype = Instantiate(AssetReference.Prefabs["Blank"], gameMap.CellToWorld(HexCoordinateUtility.CubeToOffset(GenerationPoint)), Quaternion.identity);
        mobPrototype.transform.parent = gameMap.transform;
        mobPrototype.name = "Mobs";

        //Ditto for Items
        itemPrototype = Instantiate(AssetReference.Prefabs["Blank"], gameMap.CellToWorld(HexCoordinateUtility.CubeToOffset(GenerationPoint)), Quaternion.identity);
        itemPrototype.transform.parent = gameMap.transform;
        itemPrototype.name = "Items";
        //End Unity editor organization stuff

        PlacePlayer();

        //We pass an empty list to Generate Tiles because we don't have any yet.
        GenerateTiles(new List<Vector3Int>(), GenerationPoint);
        level++;
    }

    // Update is called once per frame
    void Update()
    {

    }
}




//Legacy Code

//Old Update that worked-ish
        // //So We're going to use Timer to delegate events.   
        // if (MyTimer.turnTaken == true) {
        //     player.GetComponent<Player>().togglePlayerLock();
            
        //     //TODO:  Make this not horrible.  Level generator controls far too much.
        //     //Since we cleanup objects based on whether they have a tile underneath, we'll have to check
        //     //every GameToken's turn timer, and have them act if they are triggered.  For extra fun, we 
        //     //Also have to reassign their GameBoard dictionary reference based on their new location.
        //     var turnValue = player.GetComponent<GameToken>().GetMaxTurn();
        //     //Debug.Log(turnValue);
        //     List<GameObject> movedTokens = new List<GameObject>();
        //     List<Vector3Int> origCoords = new List<Vector3Int>();

        //     //This is the icky part, we want to move all our update logic to game board.
        //     foreach(Vector3Int HexCoord in TilesActive) {
        //         HashSet<GameObject> tileContents = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(HexCoord);
        //         foreach(GameObject token in tileContents) {
        //             //Debug.Log(token);
        //             if (token != null) {
        //                 if (token.GetComponent<Player>() == null && token.GetComponent<GameToken>() != null) {
        //                     if (movedTokens.Contains(token)) {
        //                         //We've already moved.
        //                     } else {
        //                         //Debug.Log(token);
        //                         movedTokens.Add(token);
        //                         origCoords.Add(HexCoord);
        //                         try
        //                         {
        //                             token.GetComponent<GameToken>().NextTurn(turnValue);    
        //                         }
        //                         catch (System.Exception)
        //                         {
        //                             Debug.Log(token);
        //                             Debug.Log("Deleted Token hanging about.");
        //                             continue;
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     for (int i = 0; i < movedTokens.Count; i++) {
        //         //Debug.Log(origCoords[i]);
        //         GameObject.Find("GameBoard").GetComponent<GameBoard>().RemoveToken(origCoords[i], movedTokens[i]);
        //         GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(movedTokens[i].GetComponent<GameToken>().GetCoordinates(), movedTokens[i]);
        //     }
        //     movedTokens.Clear();
        //     origCoords.Clear();

        //     int progress = (HexCoordinateUtility.HexDistance(player.GetComponent<Mob>().GetCoordinates(), new Vector3Int(0, 0, 0))/20) + (level - 1); 


        //     //All the Tile Generation here.
        //     if (0 < HexCoordinateUtility.HexDistance(GenerationPoint, player.GetComponent<Mob>().GetCoordinates())) {
        //         //get our hexRange list
        //         GenerationPoint =  player.GetComponent<Mob>().GetCoordinates();
        //         List<Vector3Int> NewTiles = HexCoordinateUtility.HexRange(GenerationPoint, GenerationDistance);
        //         GenerateTiles(TilesActive, player.GetComponent<Mob>().GetCoordinates());

        //         //With TileActive updated, we can go back and remove all Tiles that are on TilesActive
        //         //but not on New Tiles
        //         //Also crazy, C# will let you take remove and add items as you iterate, but it won't like it.
        //         List<Vector3Int> TilesToDelete = new List<Vector3Int>();
        //         foreach(Vector3Int HexCoord in TilesActive) {
        //             if (NewTiles.Contains(HexCoord)) {

        //             } else {
        //                 Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
        //                 TileMap.SetTile(OffPosition, null);
        //                 TilesToDelete.Add(HexCoord);
        //             }
        //         }
        //         GenerationPoint = player.GetComponent<GameToken>().GetCoordinates();
        //         foreach(Vector3Int HexCoord in TilesToDelete) {
        //             if (TilesActive.Contains(HexCoord)) {
        //                 TilesActive.Remove(HexCoord);
        //                 GameObject.Find("GameBoard").GetComponent<GameBoard>().AddDeadTokenList(HexCoord, GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(HexCoord));
        //             }
        //         }
        //         TilesToDelete.Clear();
        //     }
        //     GameObject.Find("GameBoard").GetComponent<GameBoard>().ClearUnboundedTokens(TilesActive);
        //     player.GetComponent<Player>().togglePlayerLock();
        // }
        // MyTimer.turnTaken = false;