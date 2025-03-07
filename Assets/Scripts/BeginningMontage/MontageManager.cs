using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class MontageManager : MonoBehaviour
{
    // Start is called before the first frame update

    [YarnCommand]
    public void DaniMovetoCalendar()
    {
        currentState = GameState.Calendar2;
    }
    
    [YarnCommand]
    public void JoannaMovetoCalendar()
    {
        currentState = GameState.Calendar3;
    }
    
    [YarnCommand]
    public void KateMovetoCalendar()
    {
        currentState = GameState.Calendar4;
    }

    public GameObject Calendar1;
    public GameObject Calendar2;
    public GameObject Calendar3;
    public GameObject Calendar4;
    public GameObject DaniandChase;
    public GameObject JoannaandChase;
    public GameObject KateandGabe;
    private bool dialoguerunning = false;
    public GameObject Cam1;
    public GameObject Cam2;
    public GameObject Cam3;
    
    private GameState currentState = GameState.Calendar1;
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
    private enum GameState
    {
        Calendar1,
        DaniandChase,
        Calendar2,
        JoannaandChase,
        Calendar3,
        KateandGabe,
        Calendar4,
        Leave
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case GameState.Calendar1:
                HandleCalendar1();
                break;
            case GameState.DaniandChase:
                HandleDaniandChase();
                break;
            case GameState.Calendar2:
                HandleCalendar2();
                break;
            case GameState.JoannaandChase:
                HandleJoannaandChase();
                break;
            case GameState.Calendar3:
                HandleCalendar3();
                break;
            case GameState.KateandGabe:
                HandleKateandGabe();
                break;
            case GameState.Calendar4:
                HandleCalendar4();
                break;
            case GameState.Leave:
                HandleLeave();
                break;
        }
    }

    private void HandleCalendar1()
    {
        Calendar1.SetActive(true);

        if (Input.GetMouseButtonDown(0))
        {
            currentState = GameState.DaniandChase;
        }
        
    }

    private void HandleDaniandChase()
    {
        Calendar1.SetActive(false);
        if (dialoguerunning == false)
        {
            dialogueRunner.StartDialogue("Flashback");
            dialoguerunning = true;
        }
        else
        {
            
        }
        
    }
    
    private void HandleCalendar2()
    {
        DaniandChase.SetActive(false);
        JoannaandChase.SetActive(true);
        Cam2.SetActive(true);
        dialogueRunner.Stop();
        Calendar2.SetActive(true);
        dialoguerunning = false;
        
        if (Input.GetMouseButtonDown(0))
        {
            Calendar2.SetActive(false);
            currentState = GameState.JoannaandChase;
        }
        
    }
    
    private void HandleJoannaandChase()
    {
        //Cam2.SetActive(true);
        Cam1.SetActive(false);
        //JoannaandChase.SetActive(true);
        if (dialoguerunning == false)
        {
            dialogueRunner.StartDialogue("Flashback2");
            dialoguerunning = true;
        }
        else
        {
            
        }
        
    }
    
    private void HandleCalendar3()
    {
        JoannaandChase.SetActive(false);
        KateandGabe.SetActive(true);
        Cam3.SetActive(true);
        dialogueRunner.Stop();
        dialoguerunning = false;
        Calendar3.SetActive(true);
        
        if (Input.GetMouseButtonDown(0))
        {
            Calendar3.SetActive(false);
            currentState = GameState.KateandGabe;
        }
        
    }
    
    private void HandleKateandGabe()
    {
        //Cam3.SetActive(true);
        Cam2.SetActive(false);
        //KateandGabe.SetActive(true);
        if (dialoguerunning == false)
        {
            dialogueRunner.StartDialogue("Flashback3");
            dialoguerunning = true;
        }
        else
        {
            
        }
        
    }
    
    private void HandleCalendar4()
    {
        KateandGabe.SetActive(false);
        dialogueRunner.Stop();
        Calendar4.SetActive(true);
        
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            currentState = GameState.Leave;
        }
    }

    private void HandleLeave()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("START");
        }
    }
}
