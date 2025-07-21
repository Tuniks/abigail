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
    private PowerSumoGame game;
    public GameObject ui;

    public PowerType power = PowerType.Pull;
    public int powerCost = 2;


    private bool isInsideUI = false;

    private void Start(){
        game = PowerManager.Instance.GetPowerSumoGame();
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
        game.ExecutePull(gameObject, 2, 100f, .7f);
    }

    // == PUSH ==
    private void ExecutePush(){
        game.ExecutePush(gameObject, 2, 100f, .5f);
    }

    // == TELEPORT ==
    private void ExecuteTeleport(){   
        game.ExecuteTeleport(gameObject, 2);
    }

    // == WALL ==
    private void ExecuteWall(){
        game.ExecuteWall(gameObject, 2);
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
