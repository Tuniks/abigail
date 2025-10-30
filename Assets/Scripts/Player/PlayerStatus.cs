using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerStatus : MonoBehaviour{
    public static PlayerStatus Instance = null;

    public DialogueRunner dialogueRunner;
    private ThirdPersonSimpleController threepc = null;
    
    // Status
    private bool isInventoryOpen = false;
    private bool isMenuOpen = false;
    private bool isAzulejoConvoOpen = false;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else{
            Destroy(this.gameObject);
        }
    }

    private void Start(){
        threepc = GetComponent<ThirdPersonSimpleController>();
    }
    
    // ====== SETTERS =====
    public void OnMenuOpen(){
        isMenuOpen = true;

        if(threepc) threepc.SetCursorLocked(false);
    }

    public void OnMenuClose(){
        isMenuOpen = false;

        if(threepc) threepc.SetCursorLocked(true);
    }

    public void OnInventoryOpen(){
        isInventoryOpen = true;

        if(threepc) threepc.SetCursorLocked(false);
    }

    public void OnInventoryClose(){
        isInventoryOpen = false;

        if(threepc) threepc.SetCursorLocked(true);
    }

    public void OnStartAzulejoConversation(){
        isAzulejoConvoOpen = true;
        OnInventoryOpen();
    }

    public void OnEndAzulejoConversation(){
        isAzulejoConvoOpen = false;
        OnInventoryClose();
    }

    // ==== GETTERS =====
    public bool IsInventoryOpen(){
        return isInventoryOpen;
    }

    public bool IsMenuOpen(){
        return isMenuOpen;
    }
    
    public bool CanMove(){
        return !isMenuOpen && !isInventoryOpen && !IsTalking();
    }

    public bool CanOpenMenu(){
        return !isMenuOpen && !isInventoryOpen && !IsTalking();;
    }

    public bool IsTalking(){
        return dialogueRunner.IsDialogueRunning || isAzulejoConvoOpen;
    }


}
