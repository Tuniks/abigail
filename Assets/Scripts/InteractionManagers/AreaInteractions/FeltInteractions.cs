using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class FeltInteractions : AreaInteractions{
    [Header("Ryans Interaction")]
    public PlayableDirector ryanAnimator;
    public TilePickUp ryanFlyTile;
    
    [Header("Timothy Interaction")]
    public GameObject soccerUI;
    public GameObject soccerMinigame;
    public SoccerPlayer soccerPlayer;

    [Header("Fellini and Albert Interactions")]
    public GameObject hauntedUI;
    public GameObject hauntedMinigame;
    public HauntedGhosts hauntedGhosts;

    // Ryans
    [YarnCommand]
    public void RyansThrowFlyTile(){
        Tile flyTile = PlayerInteractor.instance.GetLastTileUsed();
        PlayerInventory.Instance.RemoveTileFromCollection(flyTile);
        ryanFlyTile.SetTile(flyTile);
        ryanAnimator.Play();
    }

    // Timothy
    [YarnCommand]
    public void StartSoccerMinigame(){
        soccerPlayer.ResetGame();
        soccerUI.SetActive(true);
        soccerMinigame.SetActive(true);

        PlayerInteractor.instance.StartAzulejoConvo();
        PlayerInteractor.instance.HijackInteractor(EndSoccerMinigame);

    }

    public void EndSoccerMinigame(){
        soccerUI.SetActive(false);
        soccerMinigame.SetActive(false);

        PlayerInteractor.instance.EndAzulejoConvo();
        PlayerInteractor.instance.EndHijack();
    }

    // Fellini and Albert
    [YarnCommand]
    public void StartHauntedMinigame(){
        hauntedUI.SetActive(true);
        hauntedMinigame.SetActive(true);
        hauntedGhosts.ResetMinigame();

        PlayerInteractor.instance.StartAzulejoConvo();
        PlayerInteractor.instance.HijackInteractor(EndHauntedMinigame);
    }

    public void EndHauntedMinigame(){
        hauntedUI.SetActive(false);
        hauntedMinigame.SetActive(false);
        PlayerInteractor.instance.EndAzulejoConvo();
        PlayerInteractor.instance.EndHijack();
    }
}
