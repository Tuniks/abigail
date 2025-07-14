using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


public class MomsHousePortalManager : MonoBehaviour
{
    public GameObject Blockeddoor;
    public GameObject Feltportal;
    public Areas destination;
    
    [YarnCommand]
    public void CanLeave(){
        Blockeddoor.SetActive(false);
        Feltportal.SetActive(true);
    }
    
    [YarnCommand]
    public void Travel(){
        SceneController.Instance.Travel(destination);
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
