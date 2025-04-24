using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeltPhenomenonManager : MonoBehaviour{
    [Header("Tanya")]
    public GameObject tanyaNPC;

    [Header("Cockroach")]
    public GameObject cockroachInteraction;
    public float cockroachDelay = 1f;

    [Header("Sunflower")]
    public GameObject sunflowerInteraction;

    // Tanya Phenomenon
    public void ExecuteTanyaPhenomenon(){
        tanyaNPC.SetActive(true);
    }

    // Cockroach Phenomenon
    public void ExecuteCockroachPhenomenon(){
        StartCoroutine(ExecuteCockroachDelay());
    }

    private IEnumerator ExecuteCockroachDelay(){
        yield return new WaitForSeconds(cockroachDelay);
        cockroachInteraction.SetActive(true);
    }

    // Sunflower Interaction
    public void ExectureSunflowerPhenomenon(){
        sunflowerInteraction.SetActive(true);
    }
}
