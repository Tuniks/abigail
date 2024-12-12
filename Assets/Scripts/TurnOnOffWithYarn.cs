using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TurnOnOffWithYarn : MonoBehaviour
{
    public GameObject ObjectToActivate;
    public GameObject ObjectToDeactivate;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [YarnCommand]
    public void ExitPossible()
    {
        ObjectToActivate.SetActive(true);
        ObjectToDeactivate.SetActive(false);
    }
    
}
