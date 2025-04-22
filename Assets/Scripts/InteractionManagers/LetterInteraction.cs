using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class LetterInteraction : MonoBehaviour
{
    public bool CurtainInteractionHappened = false; //keeps track of if the interaction has triggered 
    public GameObject BeforeInteraction; //Sprite for before interaction
    public GameObject AfterInteraction; //Sprite for after interaction
    public AudioClip Sound;
    public AudioSource audioSource;
    public Image LoveLetter;
    private DialogueRunner dialogueRunner;
    public Transform player; // Reference to the player lovation
    public float interactionRadius = 5f; // Distance required for interaction
    public NPC delilahNPC; // assign this in the Inspector
    
    
    private enum GameState
    {
        NOLETTER,
        SHOWLETTER,
        REACTION,
        AFTER
    }
    
    private GameState currentState = GameState.NOLETTER;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // In Start(), hide it
        LoveLetter.gameObject.SetActive(false);
        // Find the DialogueRunner in the scene
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (dialogueRunner == null)
        {
            Debug.LogError("No DialogueRunner found in the scene!");
        }
    }

    
    [YarnCommand]
    public void ResetInteraction()
    {
        currentState = GameState.AFTER;
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case GameState.NOLETTER:
                HandleNOLETTER();
                break;
            case GameState.SHOWLETTER:
                HandleSHOWLETTER();
                break;
            case GameState.REACTION:
                HandleREACTION();
                break;
            case GameState.AFTER:
                HandleAFTER();
                break;
        }
    }

    private void HandleNOLETTER()
    {
        float distance = Vector2.Distance(BeforeInteraction.transform.position, player.position);//calculating the distance between player and before interaction object

        if (Input.GetKeyDown(KeyCode.E) && !CurtainInteractionHappened && distance <= interactionRadius) //if the player pressess E, the curtain interaction hasn't happened and the distance is within the interactino radisu then set after interaction sprite and chnage the bool
        {
            AfterInteraction.SetActive(true);
            //BeforeInteraction.SetActive(false);
            LoveLetter.enabled = true;
            LoveLetter.gameObject.SetActive(true);
            CurtainInteractionHappened = true;
            audioSource.PlayOneShot(Sound, 0.7F);
            currentState = GameState.SHOWLETTER;

        }
    }

    private void HandleSHOWLETTER()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LoveLetter.gameObject.SetActive(false);
            dialogueRunner.StartDialogue("LetterDiscovery");
            currentState = GameState.REACTION;
        }
    }

    private void HandleREACTION()
    {
        
    }

    private void HandleAFTER()
    {
        CurtainInteractionHappened = false;
        currentState = GameState.NOLETTER; 
        if (delilahNPC != null)
        {
            delilahNPC.SetNewDialogueNode("DelilahReadLetter");
        }
    }
    
    
}
