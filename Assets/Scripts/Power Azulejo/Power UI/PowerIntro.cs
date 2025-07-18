using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIntro : MonoBehaviour{
    public GameObject speechBubble;

    public GameObject[] playerTileSlots;
    public GameObject[] enemyTileSlots;

    public float tileScale = 185f;

    private int current = 0;

    public void Initialize(List<Tile> ptiles, List<Tile> etiles){
        current = 0;
        SetState(current);
        SetTiles(playerTileSlots, ptiles);
        SetTiles(enemyTileSlots, etiles);
    }

    private void Update(){
        if(Input.GetMouseButtonDown(0)){
            SetState(current+1);
        } else if (Input.GetMouseButtonDown(1)){
            SetState(current-1);
        }
    }

    private void SetState(int _state){
        current = Mathf.Clamp(_state, 0, 2);
        
        if(current == 0){
            speechBubble.SetActive(false);
        } else if(current == 1){
            speechBubble.SetActive(true);
        } else {
            EndIntro();
        }
    }

    private void SetTiles(GameObject[] slots, List<Tile> tiles){
        for(int i = 0; i < tiles.Count; i++){
            GameObject newtile = Instantiate(tiles[i].gameObject);
            newtile.transform.SetParent(slots[i].transform);
            newtile.transform.localPosition = Vector3.zero;
            newtile.transform.localRotation = Quaternion.identity;
            newtile.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
            newtile.SetActive(true);
        }
    }

    private void EndIntro(){
        PowerManager.Instance.TriggerIntroEnd();
    }
}
