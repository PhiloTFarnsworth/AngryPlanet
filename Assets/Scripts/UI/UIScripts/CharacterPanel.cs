using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    public Text[] textAreas;
   
    private List<string> codex = new List<string>();
    private List<string> codexCaps = new List<string>();
    private List<string> baseLetters = new List<string>();
    private List<string> baseLettersCaps = new List<string>();
    private string[] additionalConsonantsLower = {
        "þ",
        "ñ",
        "kn",//"ǯ",
        "ph",//"я",
        "st",//"љ",
        "gr",//"д",
        "th",//"ф",
        "ǧ",
        "ǩ",
        "ch",//"ψ",
        "br",//"σ",
        "rh",//"ͳ",
    };
    private string[] additionalConsonantUpper = {
        "Þ",
        "Ñ",
        "Kn",//"Ǯ",
        "Ph",//"Я",
        "St",//"Љ",
        "Gr",//Д",
        "Th",//"Ф",
        "Ǧ",
        "Ǩ",
        "Ch",//"Ψ",
        "Br",//"Σ",
        "Rh",//"Ͳ",
    };

    private string[] comboLetters = {
        "",
        "'",
        "æ",
        "á",
        "ì",
        "ø",
        "œ",
        "ü",
    };

    private string[] vowels = {
        "a",
        "e",
        "i",
        "o",
        "u",
        "y"
    };

    //Character Panel will display information about our creature.  This would be
    //for initialization and levelups.
    public void SetCharacterPanel(GameObject Player) {
        var dna = Player.GetComponent<Player>().GetDna();
        var playerName = "";
        //Debug.Log("AAAAAAAAAAAAAA");
        //Debug.Log(dna[0]);
        for (int i = 0; i < dna.Count; i++) {
            if (i == 0) {
                playerName += codexCaps[dna[i]];
            } else {
                playerName += codex[dna[i]];
            }
            if ((i&1) == 0 && dna[i] < 64) {
                int vowelIndex = 0;
                for (int j = 0; j < dna.Count; j ++) {
                    vowelIndex += dna[j];
                }
                playerName += vowels[vowelIndex%6];
            }
        }
        textAreas[0].GetComponent<Text>().text = "Your Name: " + playerName;
        textAreas[1].GetComponent<Text>().text = "Health: " + Player.GetComponent<Player>().GetCurHealth() + "/" + Player.GetComponent<Player>().GetMaxHealth();
        textAreas[2].GetComponent<Text>().text = "Strength: " + Player.GetComponent<Player>().strength;
        textAreas[3].GetComponent<Text>().text = "Defense: " + Player.GetComponent<Player>().GetDefense();
        textAreas[4].GetComponent<Text>().text = "Speed: " + Player.GetComponent<Player>().speed;
        textAreas[5].GetComponent<Text>().text = "Turns Elapsed: " + MyTimer.turnTimer;
        textAreas[6].GetComponent<Text>().text = "Obtanium Acquired: " + Player.GetComponent<Player>().points;
    }

    public void UpdateTurn() {
        textAreas[5].GetComponent<Text>().text = "Turns Elapsed: " + MyTimer.turnTimer.ToString("#0.0");
    }

    public void UpdateHealth(GameObject Player) {
        textAreas[1].GetComponent<Text>().text = "Health: " + Player.GetComponent<Player>().GetCurHealth() + "/" + Player.GetComponent<Player>().GetMaxHealth();
    }

    public void UpdateScore(GameObject Player) {
        textAreas[6].GetComponent<Text>().text = "Obtanium Acquired: " + Player.GetComponent<Player>().points + "/" + (50 + (LevelGenerator.level - 1) * 5);
    }
    // Start is called before the first frame update
    void Awake()
    {
        //Build our base letters of the language, 20 consonants 
        for (int i = 0; i < 26; i++) {
            char placeholder;
            if (i != 0 && i != 4 && i != 8 && i != 14 && i != 20 && i != 24) {
                placeholder = (char)(i+97);
                baseLetters.Add(placeholder.ToString());
                placeholder = (char)(i+65);
                baseLettersCaps.Add(placeholder.ToString());
            }
        }
        // and 12 special consonants
        for (int i = 0; i < 12; i++) {
            baseLetters.Add(additionalConsonantsLower[i]);
            baseLettersCaps.Add(additionalConsonantUpper[i]);
        }

        //Now we build the Codices from our baseLetters and ComboLetters
        for (int i = 0; i < 32; i++) {
            for (int j = 0; j < 8; j++) {
                string placeholder = baseLetters[i] + comboLetters[j];
                codex.Add(placeholder);
                placeholder = baseLettersCaps[i] + comboLetters[j];
                codexCaps.Add(placeholder);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
