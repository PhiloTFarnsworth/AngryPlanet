using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : Mob
{
    public static bool AbilityMode = false;

    public int points;

    private bool PlayerLock = false;
    private int fireBallsSpawned;

    public int statuses;

    public void SaveDna(List<int> dna) {
        PlayerPrefs.SetInt("DNA1", dna[0]);
        Debug.Log(PlayerPrefs.GetInt("DNA1"));
        PlayerPrefs.SetInt("DNA2", dna[1]);
    }

    public void IncPoints() {
        points += 5;
    }

    public void ResetPoints() {
        points = 0;
    }

    public static void toggleAbilityMode() {
        if (AbilityMode == true) {
            AbilityMode = false;
        } else {
            AbilityMode = true;
        }
    }

    public void togglePlayerLock() {
        if (PlayerLock == true) {
            PlayerLock = false;
        } else {
            PlayerLock = true;
        }
    }

    public void MoveWithCamera(string Direction) {
        var PositionCheck = transform.position;
        StopCoroutine("walk");
        GameObject.Find("GameBoard").GetComponent<GameBoard>().RemoveToken(coordinates, this.gameObject);
        StartCoroutine("walk");
        transform.position = WalkToken(coordinates, Direction);
        if (PositionCheck != transform.position) {
            var NewCam = HexCoordinateUtility.CubeToOffset(coordinates);
            Camera.main.transform.position = GameObject.Find("GameBoard").GetComponent<GridLayout>().CellToWorld(new Vector3Int(NewCam.x, NewCam.y, -10));
            IncrementTilesTraveled();
            var ItemCheck = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(coordinates);
            var consumedItems = new List<GameObject>();
            foreach(GameObject token in ItemCheck) {
                if (token.GetComponent<GameToken>().GetConsumable()) {
                    token.GetComponent<Item>().OnConsume();
                    GetComponentsInChildren<AudioSource>()[3].Play();
                    consumedItems.Add(token);
                    var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
                    characterPanel.UpdateScore(this.gameObject);
                    if (points > (50 + (LevelGenerator.level - 1) * 5)) {
                        StartCoroutine("win");
                    }
                }
            }
            for (int i = 0; i > consumedItems.Count; i++) {
                GameObject.Find("GameBoard").GetComponent<GameBoard>().RemoveToken(this.coordinates, consumedItems[i]);
            }
            consumedItems.Clear();
        } else {
            var AttackPositionObjects = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(
                (HexCoordinateUtility.OffsetToCube(GameObject.Find("GameBoard").GetComponent<GridLayout>().WorldToCell(transform.position))
                + HexCoordinateUtility.cubeDirections[Direction]));
                
            //Player will auto-attack solid objects when trying to move onto them.  
            foreach(GameObject objectListed in AttackPositionObjects) {
                if (objectListed.GetComponent<GameToken>().IsSolidToken()) {
                    Attack(objectListed, strength);
                    Debug.Log("I ran into: ");
                    Debug.Log(objectListed);
                }
            }
        }
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(coordinates, this.gameObject);
        StartCoroutine(GameObject.Find("GameBoard").GetComponent<GameBoard>().TakeTurn());
    }

    public void TeleportWithCamera(Vector3Int destination) {
        var PositionCheck = transform.position;
        GameObject.Find("GameBoard").GetComponent<GameBoard>().RemoveToken(coordinates, this.gameObject);
        transform.position = Teleport(destination);
        if (PositionCheck != transform.position) {
            var NewCam = HexCoordinateUtility.CubeToOffset(coordinates);
            Camera.main.transform.position = GameObject.Find("GameBoard").GetComponent<GridLayout>().CellToWorld(new Vector3Int(NewCam.x, NewCam.y, -10));
            IncrementTilesTraveled();
        }
        var ItemCheck = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(coordinates);
        var consumedItems = new List<GameObject>();
        foreach(GameObject token in ItemCheck) {
            if (token.GetComponent<GameToken>().GetConsumable()) {
                token.GetComponent<Item>().OnConsume();
                GetComponentsInChildren<AudioSource>()[3].Play();
                consumedItems.Add(token);
                var characterPanel = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
                characterPanel.UpdateScore(this.gameObject);
                if (points > (50 + (LevelGenerator.level - 1) * 5)) {
                    StartCoroutine("win");
                } else if (points == (50 + (LevelGenerator.level - 1) * 5)) {
                    var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
                    eventLog.AddEventLog("'Get me one more and I'll bring you up'");
                }
            }
        }
        GameObject.Find("GameBoard").GetComponent<GameBoard>().AddToken(coordinates, this.gameObject);
        statusQueue.Enqueue(new Slow(this));
        StartCoroutine(GameObject.Find("GameBoard").GetComponent<GameBoard>().TakeTurn()); 
    }

    public void Fireball(Vector3Int target) {
        List<Vector3Int> affectedTiles = new List<Vector3Int>();
        affectedTiles = HexCoordinateUtility.HexRange(target, 2);
        foreach(Vector3Int hexCoord in affectedTiles) {
            HashSet<GameObject> affectedObjects = GameObject.Find("GameBoard").GetComponent<GameBoard>().ReadCoordinates(hexCoord);
            foreach(GameObject token in affectedObjects) {
                //Spicy Damage, should clear plants and put a significant dent in any critters.
                if (token.GetComponent<DamageCloud>() == null) {
                    if (token.GetComponent<Mob>() != null) {
                        Attack(token, 10);
                    } else if (token.GetComponent<Player>() != null) {
                        Attack(token, 10);
                    } else if (token.GetComponent<Plant>() != null) {
                        Attack(token, 10);
                    }
                }
            }
            var fireball = Spawn(AssetReference.Prefabs["DamageCloud"], hexCoord);
            string newName = "FireBall" + fireBallsSpawned; 
            fireball.GetComponent<GameToken>().SetToken(newName, hexCoord, 10, false);
            fireBallsSpawned++;
        }
        var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
        eventLog.AddEventLog("'That was one spicy meatball!");
        statusQueue.Dequeue();
        StartCoroutine(GameObject.Find("GameBoard").GetComponent<GameBoard>().TakeTurn());
        fireBallsSpawned = 0;
    }

    public override void Damage(int damageAmt) {
        currentHealth = currentHealth - damageAmt;
        StartCoroutine("Splat");
        if (currentHealth < 1) {
            // do dying stuff.  Game Over. Lock player input and coroutine.
            StartCoroutine("die");
        } else {
            GetComponentsInChildren<AudioSource>()[2].Play();
        }
    }

    IEnumerator win() {
        PlanetUIStack.ourStack.Pop();
        //Do some win stuff here
        GetComponentsInChildren<ParticleSystem>()[1].Play();
        GetComponentsInChildren<ParticleSystem>()[2].Play();
        GetComponentsInChildren<AudioSource>()[1].Play();
        var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
        eventLog.AddEventLog("'I guess that will do, for now.'");
        PlayerLock = true;
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < MyAnimation[1].Count; i++) {
            renderers[2].sprite = AssetReference.MainCreature[0][MyAnimation[0][i]];
            renderers[3].sprite = AssetReference.MainCreature[1][MyAnimation[1][i]];
            if (dna.Count > 2) {
                renderers[4].sprite = AssetReference.MainCreature[2][MyAnimation[2][i]];
            }
            if (i == MyAnimation[1].Count) {
                renderers[3].sprite = AssetReference.MainCreature[1][dna[1]];
            }
            yield return new WaitForSeconds(.25f);
        }
        GetComponentsInChildren<ParticleSystem>()[2].Stop();
        GetComponentsInChildren<ParticleSystem>()[1].Stop();
        MyTimer.ClearTurn();
        ResetPoints();
        SceneManager.LoadScene("Win");
    }

    IEnumerator die() {
        //We pop off the HUD, which disables our ability to move.
        PlanetUIStack.ourStack.Pop();
        //Do a die animation
        GetComponentsInChildren<ParticleSystem>()[0].Play();
        GetComponentsInChildren<AudioSource>()[0].Play();
        PlayerLock = true;
        var eventLog = GameObject.Find("EventLog").GetComponentInChildren<EventLog>();
        eventLog.AddEventLog("'Sigh, another failure.'");
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < MyAnimation[1].Count; i++) {
            renderers[2].sprite = AssetReference.MainCreature[0][MyAnimation[0][i]];
            renderers[3].sprite = AssetReference.MainCreature[1][MyAnimation[1][i]];
            if (dna.Count > 2) {
                renderers[4].sprite = AssetReference.MainCreature[2][MyAnimation[2][i]];
            }
            if (i == MyAnimation[1].Count) {
                renderers[3].sprite = AssetReference.MainCreature[1][dna[1]];
            }
            yield return new WaitForSeconds(.25f);
        }
        GetComponentsInChildren<ParticleSystem>()[0].Pause();
        MyTimer.ClearTurn();
        LevelGenerator.level = 0;
        ResetPoints();
        PlayerPrefs.SetInt("DNA1", -1);
        PlayerPrefs.SetInt("DNA2", -1);
        SceneManager.LoadScene("GameOver");
    }
    // Start is called before the first frame update
    void Start()
    {
        RenderMob();
        GetComponentsInChildren<ParticleSystem>()[0].Pause();
        GetComponentsInChildren<ParticleSystem>()[1].Pause();
        fireBallsSpawned = 0;
        //Put player at 0 turns.
        NextTurn(GetMaxTurn());
    }

    // Update is called once per frame
    void Update()
    {
        //Beautiful clean area.  See PlanetUIs for controls.
        statuses = statusQueue.states.Count;
    }
}
