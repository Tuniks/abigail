using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class CollageManager : MonoBehaviour
{
    // Start is called before the first frame update
        // Start is called before the first frame update
        [YarnCommand]
        public void StartCollageAzulejoDay1(){
            SceneManager.LoadScene("Dani_Collage_Azulejo");
        }

        [YarnCommand]
        public void GoToPartyCollage(){
            SceneManager.LoadScene("MomsHousePartyStart");
        }
    
}
