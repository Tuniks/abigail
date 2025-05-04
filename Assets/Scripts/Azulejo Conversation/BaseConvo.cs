using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public abstract class BaseConvo : MonoBehaviour{
    protected abstract void StartConvo();
    
    public abstract void QuitConvo();

    public abstract void OnTileSelected(Tile tile, ConvoSlot slot);

    public abstract bool IsActive();

    public abstract Vector3 GetSlotPosition();

    public abstract float GetSlotScale();

    [YarnCommand]
    public void StartAzulejoConversation(){
        StartConvo();
    }
}
