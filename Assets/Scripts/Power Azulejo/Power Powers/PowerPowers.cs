using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerType{
    Pull,
    Push,
    Teleport,
    Wall
}

public class PowerPowers : MonoBehaviour{
    private PowerSumoGame game;

    [Header("Instance UI")]
    public GameObject ui;
    public Button powerButton;
    public Image icon;
    public PowerPowersAnimation anim;

    [Header("Instance Data")]
    public PowerType power = PowerType.Pull;
    public int powerCost = 2;

    private bool isInsideUI = false;

    private void Start(){
        game = PowerManager.Instance.GetPowerSumoGame();
        InitializeUI();
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
        game.ExecutePull(gameObject, powerCost, 100f, .7f);
    }

    // == PUSH ==
    private void ExecutePush(){
        game.ExecutePush(gameObject, powerCost, 100f, .5f);
    }

    // == TELEPORT ==
    private void ExecuteTeleport(){   
        game.ExecuteTeleport(gameObject, powerCost);
    }

    // == WALL ==
    private void ExecuteWall(){
        game.ExecuteWall(gameObject, powerCost);
    }

    // ======== UI ========
    private void InitializeUI(){
        icon.sprite = PowerPowersVisualManager.Instance.GetPowerIcon(power);
        anim.SetSprite(PowerPowersVisualManager.Instance.GetPowerAnim(power));
    }
    
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
        
        if(game.CanPlayerExecutePower(gameObject, powerCost)){
            powerButton.interactable = true;
        } else powerButton.interactable = false;

        ui.SetActive(true);
    }

    public void ExitUIArea(){
        HideUI();
    }

    public bool IsInsideUI(){
        return isInsideUI;
    }

    public void Animate(){
        anim.StartAnimation();
    }
    
}
