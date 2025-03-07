using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaManager : MonoBehaviour{
    public abstract void UpdateSceneState(int state);

    protected void UpdateDialogueNode(GameObject target, string newNode){
        NPC npc = target.GetComponent<NPC>();
        if(npc == null) return;

        npc.SetNewDialogueNode(new string[1]{newNode});
    }
}
