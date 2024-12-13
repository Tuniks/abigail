using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


public class PartyTransition : MonoBehaviour
{
    [YarnCommand]
    public void StartPartyAzulejo(){
        SceneManager.LoadScene("MomsHouseWParty_Azulejo");
    }
}
