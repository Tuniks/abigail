using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebInteraction : MonoBehaviour
{
    public bool CurtainInteractionHappened = false; //keeps track of if the interaction has triggered 
    public GameObject BeforeInteraction; //Sprite for before interaction
    public GameObject AfterInteraction; //Sprite for after interaction
    public AudioClip Sound;
    public AudioSource audioSource;
    
    [Header("Interaction Tiles")] 
    public Tile[] tile = null;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    

    public Transform player; // Reference to the player lovation
    public float interactionRadius = 2f; // Distance required for interaction
    
    void Update()
    {
        if (BeforeInteraction != null && player != null) 
        {
            float distance = Vector2.Distance(BeforeInteraction.transform.position, player.position);//calculating the distance between player and before interaction object

            if (!CurtainInteractionHappened && distance <= interactionRadius) // the curtain interaction hasn't happened and the distance is within the interactino radisu then set after interaction sprite and chnage the bool
            {
                AfterInteraction.SetActive(true);
                BeforeInteraction.SetActive(false);
                CurtainInteractionHappened = true;
                audioSource.PlayOneShot(Sound, 0.7F);
                
                if (tile != null && tile.Length > 0)
                {
                    PlayerInventory.Instance.AddTilesToCollection(tile);
                }
            }
        }
    }
}