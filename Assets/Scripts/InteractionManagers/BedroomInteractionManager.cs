using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomInteractionManager : MonoBehaviour
{
    public bool CurtainInteractionHappened = false; //keeps track of if the interaction has triggered 
    public GameObject BeforeInteraction; //Sprite for before interaction
    public GameObject AfterInteraction; //Sprite for after interaction
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Transform player; // Reference to the player lovation
    public float interactionRadius = 2f; // Distance required for interaction
    
    void Update()
    {
        if (BeforeInteraction != null && player != null) 
        {
            float distance = Vector2.Distance(BeforeInteraction.transform.position, player.position);//calculating the distance between player and before interaction object

            if (Input.GetKeyDown(KeyCode.E) && !CurtainInteractionHappened && distance <= interactionRadius) //if the player pressess E, the curtain interaction hasn't happened and the distance is within the interactino radisu then set after interaction sprite and chnage the bool
            {
                AfterInteraction.SetActive(true);
                BeforeInteraction.SetActive(false);
                CurtainInteractionHappened = true;
            }
        }
    }
}
