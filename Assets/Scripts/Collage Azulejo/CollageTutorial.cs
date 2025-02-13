using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject WSExplain;

    public GameObject EExplain;

    public GameObject TutorialObject;
    // Start is called before the first frame update
    void Start()
    {
        WSExplain.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W))
        {
            WSExplain.SetActive(false);
            EExplain.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EExplain.SetActive(false);
            TutorialObject.SetActive(false);
        }
            
    }
}
