using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;

public class FeltPhenomenonManager : MonoBehaviour{
    [Header("Tanya")]
    public GameObject tanyaNPC;
    public AudioClip tanyaTriggerSound;

    [Header("Cockroach")]
    public GameObject cockroachInteraction;
    public float cockroachDelay = 1f;
    public AudioClip cockroachTriggerSound;

    [Header("Sunflower")]
    public GameObject sunflowerInteraction;
    public AudioClip sunflowerTriggerSound;

    [Header("Pizza")]
    public GameObject[] pizzaObjects;
    public AudioClip pizzaTriggerSounds;

    // Tanya Phenomenon
    public void ExecuteTanyaPhenomenon(){
        tanyaNPC.SetActive(true);
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(tanyaTriggerSound);
    }

    // Cockroach Phenomenon
    public void ExecuteCockroachPhenomenon(){
        StartCoroutine(ExecuteCockroachDelay());
    }

    private IEnumerator ExecuteCockroachDelay(){
        yield return new WaitForSeconds(cockroachDelay);
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(cockroachTriggerSound);
        cockroachInteraction.SetActive(true);
    }

    // Sunflower Phenomenon
    public void ExecuteSunflowerPhenomenon(){
        sunflowerInteraction.SetActive(true);
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(sunflowerTriggerSound);
    }

    // Pizza Phenomenon
    public void ExecutePizzaPhenomenon(){
        GameObject pizza = pizzaObjects[Random.Range(0, pizzaObjects.Length)];
        Instantiate(pizza, PlayerInteractor.instance.transform.position, Quaternion.identity);
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(pizzaTriggerSounds);
    }
}
