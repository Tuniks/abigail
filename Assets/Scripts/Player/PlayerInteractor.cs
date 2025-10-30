using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerInteractor : MonoBehaviour{
    // General References
    static public PlayerInteractor instance;
    public DialogueRunner dialogueRunner;
    private AudioSource audioSource;
    private PlayerStatus ps;
    
    // UI
    public GameObject interactPrompt;
    
    // Player State
    private List<GameObject> interactables = new List<GameObject>();

    // Interaction History
    private Tile lastTileUsed = null;
    private Action hijackCallback = null;

    // Audio
    public AudioClip phenomenonNearbySoundLoop;

    void Start(){
        instance = this;
        ps = PlayerStatus.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    void Update(){
        if(interactables.Count > 0 && !ps.IsTalking()){
            interactPrompt.SetActive(true);
        } else interactPrompt.SetActive(false);

        if(Input.GetKeyDown("e")){
            if(!ps.IsTalking()){
                AttemptInteraction();
            } 
        }

        if(Input.GetKeyDown(KeyCode.Tab) && hijackCallback!=null){
            hijackCallback();
        }
    }

    private void AttemptInteraction(){
        foreach(GameObject interact in interactables){
            NPC npc = interact.GetComponent<NPC>();
            ScenePortal portal = interact.GetComponent<ScenePortal>();
            WorldInteractable thing = interact.GetComponent<WorldInteractable>();

            if(npc != null){
                string node = npc.GetCurrentNode();
                if(node != null){
                    StartConversation(node);
                    PlayerInteractionData.Instance.RegisterNPCTalk(npc.GetCharacterName());
                    return;
                }
            } else if(portal) {
                string node = portal.AttemptTravel();
                if(node != null){
                    StartConversation(node);
                    return;
                }
            } else if(thing){
                thing.Interact();
            }
        }
    }

    public void StartConversation(string node){
        dialogueRunner.StartDialogue(node);
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("NPC") || other.gameObject.CompareTag("Portal") || other.gameObject.CompareTag("WorldInteractable")){
            interactables.Add(other.gameObject);
        } else if (other.gameObject.CompareTag("Phenomenon")){
            SetCurrentPhenomenon(other.gameObject.GetComponent<AzulejoPhenomenon>());
            audioSource.clip = phenomenonNearbySoundLoop;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("NPC") || other.gameObject.CompareTag("Portal") || other.gameObject.CompareTag("WorldInteractable")){
            if(interactables.Contains(other.gameObject)) interactables.Remove(other.gameObject);
        } else if (other.gameObject.CompareTag("Phenomenon")){
            SetCurrentPhenomenon(null);
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

        private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("NPC") || other.gameObject.CompareTag("Portal") || other.gameObject.CompareTag("WorldInteractable")){
            interactables.Add(other.gameObject);
        } else if (other.gameObject.CompareTag("Phenomenon")){
            SetCurrentPhenomenon(other.gameObject.GetComponent<AzulejoPhenomenon>());
            audioSource.clip = phenomenonNearbySoundLoop;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("NPC") || other.gameObject.CompareTag("Portal") || other.gameObject.CompareTag("WorldInteractable")){
            if(interactables.Contains(other.gameObject)) interactables.Remove(other.gameObject);
        } else if (other.gameObject.CompareTag("Phenomenon")){
            SetCurrentPhenomenon(null);
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    public void RemoveInteractor(GameObject obj){
        if(interactables.Contains(obj)) interactables.Remove(obj);
    }

    public void UpdateLastTileUsed(Tile tile){
        lastTileUsed = tile;
    }

    public Tile GetLastTileUsed(){
        return lastTileUsed;
    }

    public void HijackInteractor(Action releaseCallback){
        hijackCallback = releaseCallback;
    }

    public void EndHijack(){
        hijackCallback = null;
    }

    private void SetCurrentPhenomenon(AzulejoPhenomenon _phenomenon){
        PlayerUIManager.instance.SetPhenomenon(_phenomenon);
    }

    public AudioSource GetAudioSource(){
        return audioSource;
    }
}
