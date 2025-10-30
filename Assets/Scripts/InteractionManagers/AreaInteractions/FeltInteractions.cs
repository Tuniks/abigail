using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class FeltInteractions : AreaInteractions{
    [Header("Ryans Interaction")]
    public PlayableDirector ryanAnimator;
    public TilePickUp ryanFlyTile;
    public TilePickUp ryanSkullTile;
    public TilePickUp ryanGhostTile;
    
    [Header("Timothy Interaction")]
    public GameObject soccerUI;
    public GameObject soccerMinigame;
    public SoccerPlayer soccerPlayer;

    [Header("Fellini and Albert Interactions")]
    public GameObject hauntedUI;
    public GameObject hauntedMinigame;
    public HauntedGhosts hauntedGhosts;

    [Header("Sounds")]
    public AudioClip startMinigameSound;
    public AudioClip endMinigameSound;

    // Ryans
    [YarnCommand]
    public void RyansThrowFlyTile(){
        Tile flyTile = PlayerInteractor.instance.GetLastTileUsed();
        PlayerInventory.Instance.RemoveTileFromCollection(flyTile);
        ryanFlyTile.SetTile(flyTile);
        ryanAnimator.Play();
    }
    
    [YarnCommand] 
    public void RyansThrowSkullTile(){
        Tile skullTile = PlayerInteractor.instance.GetLastTileUsed();
        PlayerInventory.Instance.RemoveTileFromCollection(skullTile);
        ryanSkullTile.SetTile(skullTile);
        ryanAnimator.Play();
    }
    
    [YarnCommand] 
    public void RyansThrowGhostTile(){
        Tile ghostTile = PlayerInteractor.instance.GetLastTileUsed();
        PlayerInventory.Instance.RemoveTileFromCollection(ghostTile);
        ryanGhostTile.SetTile(ghostTile);
        ryanAnimator.Play();
    }

    // Timothy
    [YarnCommand]
    public void StartSoccerMinigame(){
        soccerPlayer.ResetGame();
        soccerUI.SetActive(true);
        soccerMinigame.SetActive(true);

        PlayerInteractor.instance.GetAudioSource().PlayOneShot(startMinigameSound);

        PlayerStatus.Instance.OnStartAzulejoConversation();
        PlayerInteractor.instance.HijackInteractor(EndSoccerMinigame);

    }

    public void EndSoccerMinigame(){
        soccerUI.SetActive(false);
        soccerMinigame.SetActive(false);

        PlayerInteractor.instance.GetAudioSource().PlayOneShot(endMinigameSound);

        PlayerStatus.Instance.OnEndAzulejoConversation();
        PlayerInteractor.instance.EndHijack();
    }

    // Fellini and Albert
    [YarnCommand]
    public void StartHauntedMinigame(){
        hauntedUI.SetActive(true);
        hauntedMinigame.SetActive(true);
        hauntedGhosts.ResetMinigame();

        PlayerInteractor.instance.GetAudioSource().PlayOneShot(startMinigameSound);

        PlayerStatus.Instance.OnStartAzulejoConversation();
        PlayerInteractor.instance.HijackInteractor(EndHauntedMinigame);
    }

    public void EndHauntedMinigame(){
        hauntedUI.SetActive(false);
        hauntedMinigame.SetActive(false);

        PlayerInteractor.instance.GetAudioSource().PlayOneShot(endMinigameSound);

        PlayerStatus.Instance.OnEndAzulejoConversation();
        PlayerInteractor.instance.EndHijack();
    }
}
