using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //When a player starts a game, we set our DNA #'s to -1, to signify we want to roll
        //a new monster.
        PlayerPrefs.SetInt("DNA1", -1);
        PlayerPrefs.SetInt("DNA2", -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
