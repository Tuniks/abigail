using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Areas{
    None,
    Moms,
    Felt,
    Collage,
    Clay
}

public class SceneController : MonoBehaviour{
    public static SceneController Instance;

    // Mom's house
    [Header("Mom's House")]
    public string momHouseScene = "";
    public Transform moms_bedroomPortal;
    public Transform moms_streetPortal;
    public GameObject[] momsState;

    // Felt
    [Header("Felt")]
    public string feltScene = "";
    public Transform felt_momsPortal;
    public Transform felt_clayPortal;
    public Transform felt_collagePortal;
    public GameObject[] feltStates;

    // Clay
    [Header("Clay")]
    public string clayScene = "";
    public Transform clay_feltPortal;
    public GameObject[] clayStates;
    
    // Collage
    [Header("Collage")]
    public string collageScene = "";
    public Transform collage_feltPortal;
    public GameObject[] collageStates;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void Travel(Areas origin, Areas destination){
        WorldState.Instance.SetLastArea(origin);

    }

    public void CustomTravel(Areas destionation, Vector3 customDestinationPosition){

    }

}
