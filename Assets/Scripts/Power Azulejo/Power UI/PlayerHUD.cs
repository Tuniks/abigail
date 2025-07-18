using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour{
    public PowerSumoGame game;
    public TextMeshProUGUI staminaTxt;

    private int currentStam = -1;

    void Update(){
        if(game.IsGameOn() && game.IsPlayerTurn()){
            int newStam = game.GetPlayerStamina();
            if(newStam != currentStam){
                currentStam = newStam;
                string txt = "stamina";
                for(int i = 0; i < currentStam; i++){
                    txt += " " + "•";
                }
                staminaTxt.text = txt;
            }
            staminaTxt.gameObject.SetActive(true);
        } else staminaTxt.gameObject.SetActive(false);
    }
}
