using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetReference : MonoBehaviour
{
    //I realized that loading graphics for each object spawned was not terribly
    //Wise, so SpriteReference will load sprites once for any scene it's attached
    //to.  We'll also attach prefabs for generation here as well.
    public static List<Sprite[]> MasterPlant = new List<Sprite[]>();
    public static List<Sprite[]> MainCreature = new List<Sprite[]>();
    public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
    public static Sprite[] AnimatedTiles;
    public static GameObject hud;
    public static GameObject title;
    public static GameObject turnCounter;

    void Awake()
    {
        MasterPlant.Add(Resources.LoadAll<Sprite>("Sprites/Plant/PlantBases"));
        MasterPlant.Add(Resources.LoadAll<Sprite>("Sprites/Plant/PlantStems"));
        MasterPlant.Add(Resources.LoadAll<Sprite>("Sprites/Plant/PlantBuds"));
        MasterPlant.Add(Resources.LoadAll<Sprite>("Sprites/Plant/PlantFlowers"));
        MainCreature.Add(Resources.LoadAll<Sprite>("Sprites/Mob/EntityTorso"));
        MainCreature.Add(Resources.LoadAll<Sprite>("Sprites/Mob/EntityLeg"));
        MainCreature.Add(Resources.LoadAll<Sprite>("Sprites/Mob/EntityArm"));
        MainCreature.Add(Resources.LoadAll<Sprite>("Sprites/Mob/EntityWing"));
        MainCreature.Add(Resources.LoadAll<Sprite>("Sprites/Mob/EntityHead"));
        MainCreature.Add(Resources.LoadAll<Sprite>("Sprites/Mob/EntityCrown"));
        Prefabs["Player"] = Resources.Load("Prefabs/Player") as GameObject;
        Prefabs["Plant"] = Resources.Load("Prefabs/VegetationTile") as GameObject;
        Prefabs["Mob"] = Resources.Load("Prefabs/Mob") as GameObject;
        Prefabs["Item"] = Resources.Load("Prefabs/Item") as GameObject;
        Prefabs["Blank"] = Resources.Load("Prefabs/BaseObject") as GameObject;
        Prefabs["DamageCloud"] = Resources.Load("Prefabs/DamageCloud") as GameObject;
        AnimatedTiles = Resources.LoadAll<Sprite>("Sprites/Tiles/AnimatedTile");
        hud = Resources.Load("Prefabs/UIfabs/Hud") as GameObject;
        title = Resources.Load("Prefabs/UIfabs/Title") as GameObject;
        turnCounter = Resources.Load("Prefabs/UIfabs/TurnCounter") as GameObject; 
    }

    void Start() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
