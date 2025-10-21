using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AzulejoPhenomenon : MonoBehaviour{
    public GameObject face;
    public UnityEvent callback;
    public string phenomenonTriggerName;

    public void TriggerPhenomenon(){
        PlayerInteractionData.Instance.RegisterTrigger(phenomenonTriggerName);
        callback.Invoke();
    }

    public TileComponent GetFace(){
        return face.GetComponent<TileComponent>();
    }

    public bool IsAMatch(Tile tile){
        if(tile == null) return false;
        if(GetFace().title == tile.GetName()){
            return true;
        }

        return false;
    }
}
