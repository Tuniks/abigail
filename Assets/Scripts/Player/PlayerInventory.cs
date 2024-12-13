using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory{
    public static PlayerInventory Instance;
    
    [Header("UI")]
    public PlayerUIManager ui; 
    
    void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    
    protected override void Start(){
        base.Start();
        ui.SetInventoryUI();
        if(Instance == null){
            Instance = this;
        } else Destroy(gameObject);
    }
}
