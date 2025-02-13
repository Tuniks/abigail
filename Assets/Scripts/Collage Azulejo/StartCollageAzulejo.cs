using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCollageAzulejo : MonoBehaviour
{
    public GameObject Introtext;
    public GameObject CycleManager;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Introtext.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Introtext.SetActive(false);
            CycleManager.SetActive(true);
        }
    }
}
