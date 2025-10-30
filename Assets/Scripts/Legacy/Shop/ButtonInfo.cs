using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour
{
    public int ItemID;

    public TextMeshProUGUI PriceTxt;

    public GameObject ShopManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PriceTxt.text = "Price: $" + ShopManager.GetComponent<ShopManagerScript>().shopItems[2, ItemID].ToString();
    }
}
