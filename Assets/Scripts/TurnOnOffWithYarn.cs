using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TurnOnOffWithYarn : MonoBehaviour{
    public GameObject ObjectToActivate;
    public GameObject ObjectToDeactivate;

    [YarnCommand]
    public void ExitPossible()
    {
        ObjectToActivate.SetActive(true);
        ObjectToDeactivate.SetActive(false);
    }
    
}
