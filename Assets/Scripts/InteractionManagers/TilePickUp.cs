using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePickUp : WorldInteractable{
    public Tile tile;

    public void SetTile(Tile _tile){
        GameObject go = Instantiate(_tile.gameObject, transform);
        go.SetActive(false);
        tile = go.GetComponent<Tile>();
    }

    public override void Interact(){
        if(tile == null) return;

        PlayerInventory.Instance.AddTilesToCollection(new Tile[]{tile});
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy(){
        yield return new WaitForEndOfFrame();
        PlayerInteractor.instance.RemoveInteractor(gameObject);
        Destroy(gameObject);
    }
}
