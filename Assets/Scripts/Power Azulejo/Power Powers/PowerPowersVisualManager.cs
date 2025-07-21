using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPowersVisualManager : MonoBehaviour{
    public static PowerPowersVisualManager Instance;
    
    [Header("Pull")]
    public Sprite pullIcon;
    public Sprite pullAnim;

    [Header("Push")]
    public Sprite pushIcon;
    public Sprite pushAnim;

    [Header("Teleport")]
    public Sprite teleportIcon;
    public Sprite teleportAnim;

    [Header("Wall")]
    public Sprite wallIcon;
    public Sprite wallAnim;

    private void Awake(){
        Instance = this;
    }

    public Sprite GetPowerIcon(PowerType power){
        switch(power){
            case PowerType.Pull:
                return pullIcon;
            case PowerType.Push:
                return pushIcon;
            case PowerType.Teleport:
                return teleportIcon;;
            case PowerType.Wall:
                return wallIcon;
        }

        return wallIcon;
    }

    public Sprite GetPowerAnim(PowerType power){
        switch(power){
            case PowerType.Pull:
                return pullAnim;
            case PowerType.Push:
                return pushAnim;
            case PowerType.Teleport:
                return teleportAnim;;
            case PowerType.Wall:
                return wallAnim;
        }

        return wallAnim;
    }

}
