using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;

public class FeltPhenomenonManager : MonoBehaviour{
    [Header("Tanya")]
    public GameObject tanyaNPC;

    [Header("Cockroach")]
    public GameObject cockroachInteraction;
    public float cockroachDelay = 1f;

    [Header("Sunflower")]
    public GameObject sunflowerInteraction;

    [Header("Pizza")]
    public GameObject[] pizzaObjects;

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

    // Sunflower Phenomenon
    public void ExecuteSunflowerPhenomenon(){
        sunflowerInteraction.SetActive(true);
    }

    // Pizza Phenomenon
    public void ExecutePizzaPhenomenon(){
        GameObject pizza = pizzaObjects[Random.Range(0, pizzaObjects.Length)];
        Instantiate(pizza, PlayerInteractor.instance.transform.position, Quaternion.identity);
    }
}
