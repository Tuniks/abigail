using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerAudioVisualManager : MonoBehaviour{
    public static PowerAudioVisualManager Instance;

    [Header("Colors")]
    public Color playerActiveColor;
    public Color playerInactiveColor;
    public Color enemyActiveColor;
    public Color enemyInactiveColor;

    [Header("Music")]
    public AudioClip introMusic;
    public AudioClip gameMusic;

    [Header("Sounds")]
    public AudioClip hitFriend;
    public AudioClip hitEnemy;
    
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

    public Color GetActiveColor(bool isPlayerTile){
        if(isPlayerTile) return playerActiveColor;

        return enemyActiveColor;
    }

    public Color GetInactiveColor(bool isPlayerTile){
        if(isPlayerTile) return playerInactiveColor;

        return enemyInactiveColor;
    }

    public AudioClip GetHitFriendClip(){
        return hitFriend;
    }

    public AudioClip GetHitEnemyClip(){
        return hitEnemy;
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
