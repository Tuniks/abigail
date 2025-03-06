using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class FlashbackPartyStarter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PartyStarterObject;

    [YarnCommand]
    public void PartyStarter(){
        PartyStarterObject.SetActive(true);
    }
}
