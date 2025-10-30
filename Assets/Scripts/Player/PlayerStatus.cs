using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour{
    public static PlayerStatus Instance = null;
    private ThirdPersonSimpleController threepc = null;
    
    // Status
    private bool isInventoryOpen = false;
    private bool isMenuOpen = false;
    
    // Allowances
    private bool canMove = true;
    private bool canPoint = false;
    private bool canOpenMenu = true;

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

        canMove = false;
        canPoint = true;
        canOpenMenu = false;

        if(threepc) threepc.SetCursorLocked(true);
    }

    public void OnMenuClose(){
        isMenuOpen = false;

        canMove = true;
        canPoint = false;
        canOpenMenu = false;

        if(threepc) threepc.SetCursorLocked(false);
    }

    public void OnInventoryOpen(){
        isInventoryOpen = true;

        canMove = false;
        canPoint = true;
        canOpenMenu = false;

        if(threepc) threepc.SetCursorLocked(true);
    }

    public void OnInventoryClose(){
        isInventoryOpen = false;

        canMove = true;
        canPoint = false;
        canOpenMenu = false;

        if(threepc) threepc.SetCursorLocked(false);
    }

    public void OnStartDialogue(){
        
    }

    public void OnEndDialogue(){
        
    }

    // ==== GETTERS =====
    public bool IsInventoryOpen(){
        return isInventoryOpen;
    }

    public bool IsMenuOpen(){
        return isMenuOpen;
    }
    
    public bool CanMove(){
        return canMove;
    }

    public bool CanPoint(){
        return canPoint;
    }

    public bool CanOpenMenu(){
        return canOpenMenu;
    }


}
