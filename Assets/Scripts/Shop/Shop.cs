using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour{
    public GameObject shopScreen;
    
    public void ShowShop(){
        shopScreen.SetActive(true);
    }

    public void HideShop(){
        shopScreen.SetActive(false);
    }
}
