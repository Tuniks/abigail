using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSumoTutorial : MonoBehaviour{
    public GameObject[] pages;
    private int current = 0;

    public void Initialize(){
        current = 0;
        SetPage(current);
    }

    private void Update(){
        if(Input.GetMouseButtonDown(0)){
            SetPage(current+1);
        } else if (Input.GetMouseButtonDown(1)){
            SetPage(current-1);
        }
    }

    private void SetPage(int index){
        if(index < 0) return;
        if(index >= pages.Length){
            EndTutorial();
            return;
        }

        current = index;
        for(int i = 0; i < pages.Length; i++){
            if(i == current){
                pages[i].SetActive(true);
            } else pages[i].SetActive(false);
        }
    }

    private void EndTutorial(){
        PowerManager.Instance.TriggerTutorialEnd();
    }
}
