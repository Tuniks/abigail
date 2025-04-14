using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CollisionBasedInteractionLinda : MonoBehaviour
{
    public bool InteractionHappened = false;

    public GameObject ObjecttoMove;
    public GameObject ObjecttoCollideWith;

    Rigidbody2D rb;

    public AudioClip Sound;
    public AudioSource audioSource;

    public DialogueRunner dialogueRunner;
    public string nodeToStart = "StartNode"; 

    [Header("Interaction Tiles")] public Tile[] tile = null;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on this object.");
            enabled = false;
        }
    }

    void Update()
    {
        if (!InteractionHappened && ObjecttoMove.GetComponent<Collider2D>().bounds.Intersects(ObjecttoCollideWith.GetComponent<Collider2D>().bounds))
        {
            InteractionHappened = true;
            LockObject();

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
    
    void LockObject()
    {
        Rigidbody2D rb = ObjecttoMove.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    
}

