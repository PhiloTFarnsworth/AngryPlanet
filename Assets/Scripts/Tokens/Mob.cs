using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public abstract class Mob : GameToken
{
    private int level;
    public List<int> dna = new List<int>();
    private int maxLevel;
    private int tilesTraveled;
    private Color color;
    private Color[] spectrum = {
        Color.white,
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        Color.magenta,
    };
    public Dictionary<int, List<int>> MyAnimation = new Dictionary<int, List<int>>();
    public int strength;
    public int speed;

    public StateQueue statusQueue = new StateQueue();

    public string GetColor() {
        string theColor;
        if (color == Color.red) {
            theColor = "red";
        } else if (color == Color.yellow) {
            theColor = "yellow";
        } else if (color == Color.green) {
            theColor = "green";
        } else if (color == Color.blue) {
            theColor = "blue";
        } else {
            theColor = "magenta";
        }
        return theColor;
    }

    public void Init(int lvl, List<int> mobDna, Color mobColor) {
        level = lvl;
        dna.Clear();
        if (mobDna[0] == -1) {
            for (int i = 0; i < level; i++) {
                dna.Add(Random.Range(0, 255));
            }
        } else {
            dna = mobDna;
        }
        int colorRoll; 
        if (mobColor == Color.white) {
            colorRoll = Random.Range(1,6);
            color = spectrum[colorRoll];
        } else {
            if (mobColor == Color.red) {
                colorRoll = 1;
            } else if (mobColor == Color.yellow) {
                colorRoll = 2;
            } else if (mobColor == Color.green) {
                colorRoll = 3;
            } else if (mobColor == Color.blue) {
                colorRoll = 4;
            } else {
                colorRoll = 5;
            }
            color = mobColor;
        }
        SetInitAttributes(colorRoll);
        for (int i = 0; i < level; i++) {
            int startAnim = (dna[i] / 8) * 8; 
            for (int j = 0; j < 8; j++) {
                if (MyAnimation.TryGetValue(i, out List<int> values)) {
                    MyAnimation[i].Add(startAnim + j);
                } else {
                    MyAnimation.Add(i, new List<int>());
                    MyAnimation[i].Add(startAnim + j);
                }   
            }
        }
    }
    
    public void SetInitAttributes(int roll) {
        strength = 6 - roll;
        SetDefense(6 - roll);
        speed = roll;
        //As a placeholder, the slowest units will move in 1.4 turns, while the fastest move
        //in 0.6 turns.
        var MaxTurn = 16;
        SetMaxTurn((MaxTurn - 2*speed));
        SetHealth(10 + strength + GetDefense());
    }

    //While it might not be necessary to track mob tiles, perhaps we an use that functionality
    //later, with monsters in pursuit for a period of time can mutate or level up.  Depending
    //on their abilities, it could be worth it for the player to shepard enemies which have
    //area effect abilities, or impercise projectile attacks.  With this stat we can make that
    //more difficult over the long term, so a lucky early enemy roll doesn't trivialize a run.
    public int GetTilesTraveled() {
        return tilesTraveled;
    }
    public void IncrementTilesTraveled() {
        tilesTraveled++;
    }

    public List<int> GetDna() {
        return dna;
    }

    public void Attack(GameObject adversary, int attackPower) {
        int defense = adversary.GetComponent<GameToken>().defense;
        int damage;
        string attackEvent = name + " hit " + adversary.name + " for ";
        if (attackPower - defense > 0) {
            adversary.GetComponent<GameToken>().Damage(attackPower-(int)(defense/2));
            damage = (attackPower-(int)(defense/2));
        } else {
            adversary.GetComponent<GameToken>().Damage(1);
            damage = 1;
        }
        if (adversary.GetComponent<Player>() != null) {
            var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
            characterPanel.UpdateHealth(adversary);
        }
        var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
        eventLog.AddEventLog(attackEvent + damage + " damage!" );
    }

    //This works, but we need a destroy list for GameBoard, an put this at the end of an update cycle.
    public override void Damage(int damageAmt) {
        currentHealth = currentHealth - damageAmt;
        StopCoroutine("walk");
        StartCoroutine("Splat");
        if (currentHealth < 1) {
            // do dying stuff.
            GameObject.Find("GameBoard").GetComponent<GameBoard>().AddDeadToken(this.coordinates, this.gameObject);
        }
    }

    public void RenderMob() {
        maxLevel = AssetReference.MainCreature.Count;
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        // foreach(SpriteRenderer render in renderers) {
        //     Debug.Log(render);
        // }
        for (int i = 0; i < level; i++) {
            renderers[i+2].sprite = AssetReference.MainCreature[i][dna[i]];
            renderers[i+2].color = Color.white;
        }
        renderers[1].color = color;
    }


    //Since we're going to be using IEnumerators for animations, it becomes apparent
    //We'll want one for every action, to indicate an action did take place.  This, 
    //along with the event log, should create some additional clarity on the action
    //taking place between turns.
    IEnumerator walk() {
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < MyAnimation[1].Count; i++) {
            renderers[3].sprite = AssetReference.MainCreature[1][MyAnimation[1][i]];
            if (i == MyAnimation[1].Count) {
                renderers[3].sprite = AssetReference.MainCreature[1][dna[1]];
            }
            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator Splat() {
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < 8; i++) {
            if (i % 2 == 0) {
                for (int j = 0; j<level;j++) {
                    renderers[j+2].color = Color.red;
                }
            } else {
                for (int j = 0; j<level;j++) {
                    renderers[j+2].color = Color.white;
                }
            }
            yield return new WaitForSeconds(.15f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
