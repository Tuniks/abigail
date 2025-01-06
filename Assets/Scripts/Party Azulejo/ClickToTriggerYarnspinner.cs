using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ClickToTriggerYarnspinner : MonoBehaviour
{
    public string nodeName; // Name of the Yarn node to play
    private DialogueRunner dialogueRunner;

    void Start()
    {
        // Find the DialogueRunner in the scene
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (dialogueRunner == null)
        {
            Debug.LogError("No DialogueRunner found in the scene!");
        }
    }

    void OnMouseDown()
    {
        // Check if dialogue is already running
        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(nodeName);
        }
    }
}
