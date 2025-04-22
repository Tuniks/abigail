using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CollisionBasedInteraction : MonoBehaviour
{
    public GameObject ObjecttoMove; 
    public GameObject ObjecttoCollideWith1;
    public GameObject ObjecttoCollideWith2;

    public AudioClip Sound;
    public AudioSource audioSource;

    public DialogueRunner dialogueRunner;
    public string nodeToStart = "StartNode";

    [Header("Interaction Tiles")] 
    public Tile[] tile = null;

    private bool InteractionHappened = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (InteractionHappened) return;
        
        if (other.gameObject == ObjecttoCollideWith1 || other.gameObject == ObjecttoCollideWith2)
        {
            InteractionHappened = true;

            if (audioSource != null && Sound != null)
            {
                audioSource.PlayOneShot(Sound, 0.7f);
            }

            if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.StartDialogue(nodeToStart);
            }

            if (tile != null && tile.Length > 0 && PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddTilesToCollection(tile);
            }
        }
    }
}