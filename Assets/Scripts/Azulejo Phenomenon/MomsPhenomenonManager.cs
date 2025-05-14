using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomsPhenomenonManager : MonoBehaviour{
    [Header("Toucan")]
    public GameObject toucanNPC;
    public AudioClip toucanTriggerSound;

    public void ExecuteToucanPhenomenon(){
        toucanNPC.SetActive(true);
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(toucanTriggerSound);
    }
}
