using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour{
    public float fadeDuration = .5f;
    public float pauseDuration = 5f;

    public void Show(){
        Fader fader = GetComponent<Fader>();
        if(fader != null){
            gameObject.SetActive(true);
            StartCoroutine(fader.FadeInAndOut(fadeDuration, pauseDuration));
        }
    }
}
