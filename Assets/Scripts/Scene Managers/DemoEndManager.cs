using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DemoEndManager : MonoBehaviour
{
    public Areas destination;
  
    [YarnCommand]
    public void GoToStart(){
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
