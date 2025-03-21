using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerInteractor : MonoBehaviour{
    public DialogueRunner dialogueRunner;
    
    public GameObject interactPrompt; 
    
    private PlayerController pc;

    private List<GameObject> interactables = new List<GameObject>();
    private bool isTalking = false;
    private Shop currentShop = null;

    void Start(){
        pc = GetComponent<PlayerController>();
    }

    void Update(){
        if(interactables.Count > 0 && !isTalking && currentShop == null){
            interactPrompt.SetActive(true);
        } else interactPrompt.SetActive(false);

        if(Input.GetKeyDown("e") && currentShop == null){
            if(!isTalking){
                AttemptInteraction();
            } 
        }

        if(Input.GetKeyDown("q") && currentShop != null && !isTalking){
            currentShop.HideShop();
            currentShop = null;
            pc.SetIsBusy(false);
        }
    }

    void LateUpdate(){
        if(dialogueRunner.IsDialogueRunning){
            isTalking = true;
            pc.SetIsBusy(true);
        } else {
            if(isTalking) pc.SetIsBusy(false);
            isTalking = false;
        }
    }

    private void AttemptInteraction(){
        foreach(GameObject interact in interactables){
            NPC npc = interact.GetComponent<NPC>();
            Shop shop = interact.GetComponent<Shop>();
            ScenePortal portal = interact.GetComponent<ScenePortal>();

            if(npc != null){
                string node = npc.GetCurrentNode();
                if(node != null){
                    StartConversation(node);
                    pc.SetIsBusy(true);
                    return;
                }
            } else if(shop != null){
                currentShop = shop;
                currentShop.ShowShop();
                string node = currentShop.GetCurrentNode();
                if(node != null) StartConversation(node);
                pc.SetIsBusy(true);
                return;
            } else if(portal) {
                string node = portal.AttemptTravel();
                if(node != null){
                    StartConversation(node);
                    pc.SetIsBusy(true);
                    return;
                }
            }
        }
    }

    private void StartConversation(string node){
        dialogueRunner.StartDialogue(node);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("NPC") || other.gameObject.CompareTag("Shop") || other.gameObject.CompareTag("Portal")){
            interactables.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("NPC") || other.gameObject.CompareTag("Shop") || other.gameObject.CompareTag("Portal")){
            if(interactables.Contains(other.gameObject)) interactables.Remove(other.gameObject);
        }
    }
}
