using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : GameToken
{
    //Render Stuff
    private List<int> MyAnimation = new List<int>(); 
    private bool gfxRefresh = false;
    private List<SpriteRenderer> flowerRenderer = new List<SpriteRenderer>();
    //Individual stuff
    public List<int> PlantDNA = new List<int>();
    public int growthLevel;
    public bool growthChange = false; 


    //We'll create a custom struct to pass our information

    //Our Init essentially allows us to quickly set our desired DNA and growth levels after
    //Instanciating the parent object. 
    public void Init(List<int> dna, int growthLvl, bool dummy) {
        //if we send an list with starting with a -1, just make random.
        if (dna[0] == -1) {
            PlantDNA.Clear();
            for (int i = 0; i < 4; i++) {
                PlantDNA.Add(Random.Range(0,256));
            }
        } else {
            PlantDNA = dna;
        }
        growthLevel = growthLvl;
        
        growthChange = true;
        gfxRefresh = true;
        
        int StartAnim = (PlantDNA[3] / 8) * 8; 
        MyAnimation.Clear();
        for (int i = 0; i < 8; i++) {
            MyAnimation.Add(StartAnim + i);
        }
        if (growthLevel == 4) {
            if (dummy == false) {
                isSolid = true;
            }
        }
        SetHealth(10);
        SetMaxTurn(Random.Range(20, 40));
    }
    //We'll increment the growthLevel, refresh our render.  If Growth is 4, then on their
    //Move they'll randomly spread to a non-occupied neighboring tile.
    public void Grow() {
        var activeTiles = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        if (growthLevel < 4) {
            growthLevel++;
            growthChange = true;
            gfxRefresh = true;
        }
        if (growthLevel == 4) {
            isSolid = true;
            var possibleGrows = new List<string>(); 
            Dictionary<string, Vector3Int> neighbors = this.GetNeighbors();
            foreach(KeyValuePair <string, Vector3Int> neighborTile in neighbors) {
                if (activeTiles.TrackedTile(neighborTile.Value) == true) {
                    HashSet<GameObject> tokenList = activeTiles.ReadCoordinates(neighborTile.Value);
                    bool occupied = false;    
                    foreach(GameObject token in tokenList) {
                        if (token.GetComponent<Plant>() != null) {
                            occupied = true;
                        }
                    }
                    if (occupied == false) {
                        possibleGrows.Add(neighborTile.Key);
                    }
                }
            }
            if (possibleGrows.Count > 0) {
                var choice = possibleGrows[(Random.Range(0, possibleGrows.Count))];
                var HexCoord = coordinates + HexCoordinateUtility.cubeDirections[choice];
                if (activeTiles.TrackedTile(HexCoord)) {
                    GameObject newPlant = Spawn(AssetReference.Prefabs["Plant"], HexCoord);
                    var newName = "Plant" + LevelGenerator.GetPlantsCreated();
                    newPlant.GetComponent<GameToken>().SetToken(newName, HexCoord, 1, false);
                    newPlant.GetComponent<Plant>().Init(new List<int> {-1}, 1, false);
                    LevelGenerator.IncPlantsCreated();
                }
            }
        }
    }

    //Damage plant damages the plant, then checks if health is below zero, if so,
    //reset the growth back to the roots, "heal" plant, reset the render and unsolidify
    //it.
    public override void Damage(int damageAmt) {
        currentHealth = currentHealth - damageAmt;
        if (currentHealth < 1) {
            growthLevel = 1;
            growthChange = true;
            gfxRefresh = true;
            SetHealth(10);
            ResetRender();
            isSolid = false;
        }
    }

    //Plants Grow.  They maybe also damage Player when adjacent.
    public override void Behaviour() {
        Grow();
        var currentTile = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(coordinates);
        foreach(GameObject token in currentTile) {
            if (token.GetComponent<Mob>() != null) {
                token.GetComponent<Mob>().Damage(1);
            }
            if (token.GetComponent<Player>() != null) {
                string attackEvent = name + " hit " + token.GetComponent<Player>().name + " for ";
                var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
                eventLog.AddEventLog(attackEvent + "1" + " damage!" );
                var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
                characterPanel.UpdateHealth(token);
            }
        } 
    }

    
    IEnumerator AnimateSprites() {
        for (int i = 0; i < MyAnimation.Count; i++) {
            for (int j = 0; j < flowerRenderer.Count; j++) {
                flowerRenderer[j].sprite = AssetReference.MasterPlant[3][MyAnimation[i]];
                yield return new WaitForSeconds(Random.Range(.1f,.25f));
            }
            if (i == MyAnimation.Count - 1) {
                i = 0;
            }
        }
    }

    //Plants don't really die, but the player can knock them down to their first
    //stage of growth.  To represent this, we just need to set all our renders to
    //transparent.
    public void ResetRender() {
        SpriteRenderer[] rawRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in rawRenderers)
        {
            renderer.color = new Color(0,0,0,0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Generic Debug
        //Init(new List<int> {-1}, 0);
        SpriteRenderer[] rawRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        List<SpriteRenderer> Renderers = new List<SpriteRenderer>();
        for (int i = 1; i < rawRenderers.Length; i++) {
            Renderers.Add(rawRenderers[i]);
            //Debug.Log(Renderers[i-1].name);
        }
        for (int i = 0; i < 7; i++) {
            flowerRenderer.Add(Renderers[i*5+4]);
        }
        StartCoroutine("AnimateSprites");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (gfxRefresh == true) {
            if (growthChange == true) {
            //We'll render based on our growth level
            //Debug.Log(growthChange);

            //This is a little complicated, but we have here an array of 36 sprites, which should be (1) parentHex, 
            //then square(invisible), PlantBase, PlantStem, PlantBud, PlantFlower x 6.
                SpriteRenderer[] rawRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
                List<SpriteRenderer> Renderers = new List<SpriteRenderer>();
                for (int i = 1; i < rawRenderers.Length; i++) {
                    Renderers.Add(rawRenderers[i]);
                    //Debug.Log(Renderers[i-1].name);
                } 
                if (Renderers.Count == 35) {
                    switch(growthLevel) {
                        case 0:
                            //Do Nothing, they begin rendered invisible
                            break;
                        case 1:
                            for (int i = 0; i < 7; i++) {
                                if (Random.Range(0, 6) > 0) {
                                    Renderers[5*i+1].sprite = AssetReference.MasterPlant[0][PlantDNA[0]];
                                    Renderers[5*i+1].color = new Color(1,1,1,1);
                                    if (Random.Range(0, 3) == 2) {
                                        Renderers[5*i].transform.localScale += new Vector3(.03f,.03f,0);
                                    }
                                }
                            }
                            break;
                        case 2:
                            for (int i = 0; i < 7; i++) {
                                if (Random.Range(0, 6) > 1) {
                                    for (int j = 1; j < 3; j++) {
                                        Renderers[5*i+j].sprite = AssetReference.MasterPlant[j-1][PlantDNA[j-1]];
                                        Renderers[5*i+j].color = new Color(1,1,1,1);
                                        if (Random.Range(0, 2) > 0) {
                                            Renderers[5*i].transform.localScale += new Vector3(.03f,.03f,0);
                                        }
                                    }
                                }
                            }
                            break;
                        case 3:
                            for (int i = 0; i < 7; i ++) {
                                if (Random.Range(0, 6) > 0) {
                                    for (int j = 1; j < 4; j++) {
                                        Renderers[5*i+j].sprite = AssetReference.MasterPlant[j-1][PlantDNA[j-1]];
                                        Renderers[5*i+j].color = new Color(1,1,1,1);
                                        if (Random.Range(0, 2) > 0) {
                                            Renderers[5*i].transform.localScale += new Vector3(.03f,.03f,0);
                                        }
                                    }
                                }
                            }
                            break;
                        case 4:
                            for (int i = 0; i < 7; i++) {
                                if (Random.Range(0, 6) > -1) {
                                    for (int j = 1; j < 5; j++) {
                                        Renderers[5*i+j].sprite = AssetReference.MasterPlant[j-1][PlantDNA[j-1]];
                                        Renderers[5*i+j].color = new Color(1,1,1,1);
                                        if (Random.Range(0, 2) > 0) {
                                            Renderers[5*i].transform.localScale += new Vector3(.04f,.04f,0);
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            Debug.Log("Gerror");
                            break;
                        
                    }
                    Renderers.Clear();
                    growthChange = false;
                }
            }
        }
    }
}

