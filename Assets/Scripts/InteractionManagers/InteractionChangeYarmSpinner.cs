using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InteractionChangeYarmSpinner : MonoBehaviour
{
    // Start is called before the first frame update
    //public bool CurtainInteractionHappened = false; //keeps track of if the interaction has triggered 
    public GameObject BeforeInteraction; //Sprite for before interaction
    public GameObject AfterInteraction; //Sprite for after interaction
    public AudioClip Sound;
    public AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [YarnCommand]
    public void TriggerChange()
    {
        AfterInteraction.SetActive(true);
        BeforeInteraction.SetActive(false);
        audioSource.PlayOneShot(Sound, 0.7F);
        //CurtainInteractionHappened = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
