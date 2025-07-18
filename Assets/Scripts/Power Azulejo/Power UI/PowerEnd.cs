using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerEnd : MonoBehaviour{
    public TextMeshProUGUI text;

    public void Initialize(bool playerWon){
        if(playerWon){
            text.text = "You won";
        } else text.text = "You lose";
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            EndEnd();
        }    
    }

    private void EndEnd(){
        PowerManager.Instance.TriggerEndEnd();
    }
}
