using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GoodbyeOz : MonoBehaviour
{
    public GameObject PoofHolder;
    public GameObject Oz;

    //public AudioSource PoofSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    [YarnCommand]
    public void GoodbyebyeOz()
    {
        Oz.SetActive(false);
        PoofHolder.SetActive(true);
        //AudioSource.PlayOneShot(PoofSound, 1);
    }
    // Update is called once per frame
    

}
