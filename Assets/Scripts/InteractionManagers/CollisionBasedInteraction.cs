using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CollisionBasedInteraction : MonoBehaviour
{
    public bool InteractionHappened = false;

    public GameObject ObjecttoMove;
    public GameObject ObjecttoCollideWith;

    public AudioClip Sound;
    public AudioSource audioSource;
    
    public DialogueRunner dialogueRunner;
    public string nodeToStart = "StartNode"; // Name of the Yarn node you want to start

    [Header("Interaction Tiles")] 
    public Tile[] tile = null;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (!InteractionHappened && ObjecttoMove.GetComponent<Collider2D>().bounds.Intersects(ObjecttoCollideWith.GetComponent<Collider2D>().bounds))
        {
            InteractionHappened = true;

            if (dialogueRunner != null && dialogueRunner.IsDialogueRunning == false)
            {
                audioSource.PlayOneShot(Sound, 0.7F);
                dialogueRunner.StartDialogue(nodeToStart);
                
                if (tile != null && tile.Length > 0)
                {
                    PlayerInventory.Instance.AddTilesToCollection(tile);
                }
            }
        }
    }
}