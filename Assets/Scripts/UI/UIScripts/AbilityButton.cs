using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class AbilityButton : MonoBehaviour
{
    //We're going to call a single ability button a success.  Eventually,
    //We'll need to support several abilities, and give monsters access
    //to those abilities.  So we probably won't have all that information
    //on a button.

    private Text Cooldown;
    private float coolDownTimer;

    public void DecrementCooldown(float turnvalue) {
        coolDownTimer = coolDownTimer - turnvalue;
        if (coolDownTimer < 0) {
            coolDownTimer = 0;
        }
        Cooldown.text = "Cooldown: " + coolDownTimer.ToString("#0.0") + " turns";
    }   

    public void ResetCooldown() {
        coolDownTimer = 50;
        Cooldown.text = "Cooldown: " + coolDownTimer.ToString("#0.0") + " turns";
    }

    public void ChangeAbilityName() {
        var Texts = this.GetComponentsInChildren<Text>();
        if (Texts[0].text == "Teleport") {
            Texts[0].text = "Fireball";
        } else {
            Texts[0].text = "Teleport";
        }
         
    }

    [SerializeField]
    private UnityEvent OnMouseClick = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Button>().OnClick.Add(Player.toggleAbilityMode());
        var Texts = this.GetComponentsInChildren<Text>(); 
        Cooldown = Texts[1];
    }

    // Update is called once per frame
    void Update()
    {
            if(Input.GetMouseButtonDown(0))
            {
                OnMouseClick.Invoke();   
            }
    }

    public void MouseClicked()
    {
        GetComponent<AudioSource>().Play(); 
        if (coolDownTimer <= 0) {
            //Player.toggleAbilityMode();
            PlanetUIStack.ourStack.Push(new AbilitySelect());           
        } else {
            //Some sort of acknowledgement that I implemented cooldowns.
            Debug.Log("Ability Unavailable");
        }
    }
}
