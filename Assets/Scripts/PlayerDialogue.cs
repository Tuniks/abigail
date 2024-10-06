using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerDialogue : MonoBehaviour{
    private CapsuleCollider col;

    public DialogueRunner dialogueRunner;

    public float radius = 4;
    public LayerMask mask;
    
    private bool isTalking = false;

    void Start(){
        col = GetComponent<CapsuleCollider>();
    }

    void Update(){
        if(Input.GetKeyDown("e")){
            if(!dialogueRunner.IsDialogueRunning){
                AttemptConversation();
            } 
        }
    }

    private void AttemptConversation(){
        Vector3 point1 = (transform.position + col.center) + new Vector3(0, col.height/2 - col.radius, 0);
        Vector3 point2 = (transform.position + col.center) + new Vector3(0, -col.height/2 + col.radius, 0); 
    
        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, transform.forward, 0, mask);
        foreach(RaycastHit hit in hits){
            if(hit.collider.gameObject.tag == "NPC"){
                NPC npc = hit.collider.gameObject.GetComponent<NPC>();
                if(npc != null){
                    string node = npc.GetCurrentNode();
                    if(node != null){
                        StartConversation(node);
                        return;
                    }
                }
            }
        }
    }

    private void StartConversation(string node){
        dialogueRunner.StartDialogue(node);
    }
}
