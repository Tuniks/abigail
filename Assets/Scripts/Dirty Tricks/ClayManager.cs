using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


public class ClayManager : MonoBehaviour{
    public GameObject antiqueShop;
    public NPC Chase;

    public string azulejoScene = "CLAY_azulejo";
    public string partyScene = "MomsHousePartyStart";

    private bool antiqueShopVisited = false;

    void Update(){
        if(antiqueShop.activeSelf){
            if(!antiqueShopVisited){
                AdvanceDialogue();
            }
            antiqueShopVisited = true;
        }
    }

    void AdvanceDialogue(){
        string[] nodes = new string[1]{
            "PostAntiqueShop"
        };
        
        Chase.SetNewDialogueNode(nodes);
    }

    [YarnCommand]
    public void GoToClayAzulejo(){
        SceneManager.LoadScene(azulejoScene);
    }

    [YarnCommand]
    public void GoToPartyScene(){
        SceneManager.LoadScene("MomsHousePartyStart");
    }
}
