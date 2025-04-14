using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class IndicatorToggle : MonoBehaviour
{
    public GameObject Indicator;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [YarnCommand]
    public void ShowIndicator()
    {
        Indicator.SetActive(true);
    }
    
    [YarnCommand]
    public void HideIndicator()
    {
        Indicator.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
