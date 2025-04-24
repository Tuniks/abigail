using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AzulejoPhenomenon : MonoBehaviour{
    public GameObject face;
    public UnityEvent callback;

    public void TriggerPhenomenon(){
        callback.Invoke();
    }

    public TileComponent GetFace(){
        return face.GetComponent<TileComponent>();
    }
}
