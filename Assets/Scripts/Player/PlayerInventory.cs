using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory{
    [Header("UI")]
    public PlayerUIManager ui; 
    
    protected override void Start(){
        base.Start();
        ui.SetInventoryUI();
    }

    void Update(){
        
    }
}
