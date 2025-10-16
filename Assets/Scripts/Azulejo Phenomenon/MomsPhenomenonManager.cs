using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MomsPhenomenonManager : MonoBehaviour{
    [Header("Toucan")]
    public GameObject toucanNPC;
    public AudioClip toucanTriggerSound;
    public DialogueRunner dialogueRunner;

    public void ExecuteToucanPhenomenon(){

    toucanNPC.SetActive(true);
    PlayerInteractor.instance.GetAudioSource().PlayOneShot(toucanTriggerSound);

    if (!dialogueRunner.IsDialogueRunning){
        dialogueRunner.StartDialogue("Tuca");
    } else {
        Debug.LogWarning("Dialogue is already running.");
    }
}


}
