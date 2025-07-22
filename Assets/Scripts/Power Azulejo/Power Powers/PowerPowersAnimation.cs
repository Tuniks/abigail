using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerPowersAnimation : MonoBehaviour{
    public float startScale = 1f;
    public float endScale = 5f;
    
    public float duration = 1f;
    
    public void SetSprite(Sprite spr){
        if(spr == null) return;
        GetComponent<Image>().sprite = spr;
    }

    public void StartAnimation(){
        transform.localScale = new Vector3(startScale, startScale, startScale);
        gameObject.SetActive(true);
        StartCoroutine(Animate());
    }

    private IEnumerator Animate(){
        float lerp = 0;
        float scale;

        while((lerp/duration) <= 1){
            scale = Mathf.Lerp(startScale, endScale, lerp/duration);
            transform.localScale = new Vector3(scale, scale, scale);
            lerp += Time.deltaTime;
            yield return null;
        }

        EndAnimation();
    }

    private void EndAnimation(){
        gameObject.SetActive(false);
    }
}
