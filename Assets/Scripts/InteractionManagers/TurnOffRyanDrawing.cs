using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOffRyanDrawing : MonoBehaviour
{
    public Image RyanDrawing;
    
    public void HandleHIDEPLAN()
    {
        RyanDrawing.enabled = false;
        RyanDrawing.gameObject.SetActive(false);
    }
}
