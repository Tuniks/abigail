using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerType{
    Pull,
    Push,
    Teleport,
    Wall
}

public class PowerPowers : MonoBehaviour{
    public GameObject ui;

    public PowerType power = PowerType.Pull;
    public int powerCost = 2;


    private bool isInsideUI = false;

    public void Initialize(Tile tile){

    }

    void Update(){
        
    }

    // ======= GENERAL POWER BEHAVIOR ======
    public void ExecutePower(){
        switch(power){
            case PowerType.Pull:
                ExecutePull();
                break;
            case PowerType.Push:
                ExecutePush();
                break;
            case PowerType.Teleport:
                ExecuteTeleport();
                break;
            case PowerType.Wall:
                ExecuteWall();
                break;
        }
    }

    // == PULL ==
    private void ExecutePull(){

    }

    // == PUSH ==
    private void ExecutePush(){
        
    }

    // == TELEPORT ==
    private void ExecuteTeleport(){
        
    }

    // == WALL ==
    private void ExecuteWall(){
        
    }

    // ======== UI ========
    public void ToggleUI(){
        bool state = ui.activeSelf;
        if(state){
            HideUI();
        } else ShowUI();
    }

    private void HideUI(){
        isInsideUI = false;
        ui.SetActive(false);
    }

    private void ShowUI(){
        isInsideUI = true;
        ui.SetActive(true);
    }

    public void ExitUIArea(){
        HideUI();
    }

    public bool IsInsideUI(){
        return isInsideUI;
    }

    
}
