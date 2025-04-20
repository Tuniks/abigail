using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class FeltInteractions : AreaInteractions{
    [Header("Ryans Interaction")]
    public PlayableDirector ryanAnimator;
    public TilePickUp ryanFlyTile;
    
    // Ryans
    [YarnCommand]
    public void RyansThrowFlyTile(){
        Tile flyTile = PlayerInteractor.instance.GetLastTileUsed();
        PlayerInventory.Instance.RemoveTileFromCollection(flyTile);
        ryanFlyTile.SetTile(flyTile);
        ryanAnimator.Play();
    }
}
