using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


public class ClayManager : MonoBehaviour{
    public GameObject antiqueShop;
    public NPC Kate;
    public NPC Chase;

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
        Kate.SetNewDialogueNode(nodes);
    }

    [YarnCommand]
    public void GoToPartyScene(){
        SceneManager.LoadScene("MomsHousePartyStart");
    }
}
