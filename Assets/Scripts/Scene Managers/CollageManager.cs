using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class CollageManager : AreaManager{
    public DialogueRunner dialogueRunner;
    
    [Header("NPCs")]
    public GameObject dani;

    [Header("Misc")]
    public Tile[] tvTile;
    public Tile[] slimeTile;
    public Tile[] coffeeTile;
    
    [Header("Game Objects for Interactions")]
    public GameObject LandfillEntrance;

    public GameObject StaticLandfill;
    public GameObject FootAnimation;
    public GameObject Warehouse;
    public AudioClip Sound;
    public AudioSource audioSource;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Intro
                break;
            case 1:
                // First round
                UpdateDialogueNode(dani, "DaniSigilAzulejo");
                break;
            case 2:
                //Post Azulejo
                UpdateDialogueNode(dani, "Dani5");
                break;
            //case 3:
                //UpdateDialogueNode(dani, "Dani3");
                //break;
            //case 4:
                //UpdateDialogueNode(dani, "Dani4");
                //break;
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void StartRound1(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 1, true);
    }

    [YarnCommand]
    public void StartRound2(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 2, true);
    }

    [YarnCommand]
    public void StartRound3(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 3, true);
    }

    [YarnCommand]
    public void EndAzulejoGame(){
        WorldState.Instance.UpdateNPCDialogueNode("Chase", "Chase4");
        WorldState.Instance.UpdateSceneState(Areas.Collage, 2, true);
        WorldState.Instance.UpdateSceneState(Areas.Felt, 2);
    }
    
    [YarnCommand]
    public void GoToSteamLinkSceneFromCollage()
    {
        SceneManager.LoadScene("STEAMLINK");
    }
    
    [YarnCommand]
    public void Givetv()
    {
        PlayerInventory.Instance.AddTilesToCollection(tvTile);
    }
    
    [YarnCommand]
    public void Giveslime()
    {
        PlayerInventory.Instance.AddTilesToCollection(slimeTile);
    }
    
    [YarnCommand]
    public void Givecoffee()
    {
        PlayerInventory.Instance.AddTilesToCollection(coffeeTile);
    }
    
    [YarnCommand]
    public void OpenLandfill()
    {
        LandfillEntrance.SetActive(true);
        StaticLandfill.SetActive(false);
    }
    
    [YarnCommand]
    public void PlayFootAnimation()
    {
        FootAnimation.SetActive(true);
        Warehouse.SetActive(false);
        audioSource.PlayOneShot(Sound, 0.7F);
    }
    
}
