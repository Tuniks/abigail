using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour{
    public float fadeSpeed = 1f;
    public Image bg;
    public Image button;
    public GameObject poem;
    
    private Color fadingColor;
    private bool isFading = false;
    private bool hasFaded = false;
    private bool canStart = false;
    
    void Awake(){
        fadingColor = bg.color;
    }

    void Update(){
        if(isFading){
            fadingColor.a -= Time.deltaTime * fadeSpeed;
            bg.color = fadingColor;
            button.color = fadingColor;
            if(fadingColor.a <= 0){
                isFading = false;
                hasFaded = true;
                bg.gameObject.SetActive(false);
                poem.SetActive(true);
                StartCoroutine(EnableStart());
            }
        }

        if(canStart && Input.anyKey){
            SceneManager.LoadScene("MomsHouse");
        }
    }

    public void OnClickStart(){
        if(hasFaded || isFading) return;

        isFading = true;
    }

    private IEnumerator EnableStart(){
        yield return new WaitForSeconds(2);
        canStart = true;
    }

}
