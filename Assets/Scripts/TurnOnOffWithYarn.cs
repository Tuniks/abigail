using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TurnOnOffWithYarn : MonoBehaviour{
    public GameObject ObjectToActivate;
    public GameObject ObjectToDeactivate;

    public Tile[] tilesInChest;
    public PlayerInventory playerInventory;

    [YarnCommand]
    public void ExitPossible(){
        playerInventory.AddTilesToCollection(tilesInChest);
        ObjectToActivate.SetActive(true);
        ObjectToDeactivate.SetActive(false);
    }
    
}
