using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeltPhenomenonManager : MonoBehaviour{
    [Header("Tanya")]
    public GameObject tanyaNPC;

    // Tanya Phenomenon
    public void ExecuteTanyaPhenomenon(){
        tanyaNPC.SetActive(true);
    }
}
