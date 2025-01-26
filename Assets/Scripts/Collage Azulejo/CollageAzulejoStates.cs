using UnityEngine;
using System.Collections.Generic;

public class CollageAzulejoStates : MonoBehaviour
{
    public enum GameState { NPC1TURN, PLAYERTURN, NPC2TURN, NPC3TURN, ROUNDVERDICT, REEVALUATION, REPLACE }

    public GameState currentState;

    public GameObject npc1Tile, npc2Tile, npc3Tile; // NPC Tiles
    public List<GameObject> emptySlots;            // Public list of valid slots
    public List<GameObject> playerTiles;           // List of player tiles for toggling Draggable script
    public GameObject EvaluationObjects;           // Public GameObject to be activated during ROUNDVERDICT
    public GameObject NO;                          // The NO GameObject to be clicked on
    public GameObject YourTurn;

    public float npcTurnDuration = 5f;             // Duration of each NPC turn (in seconds)
    private float npcTimer = 0f;                   // Timer to track NPC turn duration

    void Start()
    {
        SetState(GameState.NPC1TURN); // Initialize the first state

        // Add listener to NO GameObject (ensure it has Collider2D for 2D interaction)
        if (NO != null)
        {
            var collider2D = NO.GetComponent<Collider2D>();
            if (collider2D != null)
            {
                collider2D.isTrigger = true; // Ensure it's triggerable
            }
            else
            {
                Debug.LogWarning("NO GameObject does not have a Collider2D component.");
            }
        }
    }


    void Update()
    {
        // NPC turn timer logic
        if (currentState == GameState.NPC1TURN || currentState == GameState.NPC2TURN || currentState == GameState.NPC3TURN)
        {
            npcTimer += Time.deltaTime;

            // Transition to the next state when the timer expires
            if (npcTimer >= npcTurnDuration)
            {
                AdvanceState();
            }
        }
    }

    private void AdvanceState()
    {
        npcTimer = 0f; // Reset the timer

        switch (currentState)
        {
            case GameState.NPC1TURN:
                SetState(GameState.PLAYERTURN);
                break;
            case GameState.PLAYERTURN:
                SetState(GameState.NPC2TURN);
                break;
            case GameState.NPC2TURN:
                SetState(GameState.NPC3TURN);
                break;
            case GameState.NPC3TURN:
                SetState(GameState.ROUNDVERDICT);
                break;
            case GameState.ROUNDVERDICT:
                // This state will be triggered by the NO button click.
                break;
            case GameState.REEVALUATION:
                // Add logic for the REEVALUATION state if needed.
                Debug.Log("Reevaluation started.");
                break;
            case GameState.REPLACE:
                //
                break;
        }
    }


    public void SetState(GameState newState)
    {
        currentState = newState;
        npcTimer = 0f; // Reset the timer whenever the state changes
        UpdateState();
    }

    private void UpdateState()
    {
        // Deactivate all NPC tiles and disable player dragging
        foreach (GameObject tile in playerTiles)
        {
            tile.GetComponent<Draggable>().enabled = false;
        }

        switch (currentState)
        {
            case GameState.NPC1TURN:
                PlaceNpcTile(npc1Tile);
                break;

            case GameState.PLAYERTURN:
                // Enable player to drag tiles, but only to available slots
                YourTurn.SetActive(true);
                foreach (GameObject tile in playerTiles)
                {
                    tile.GetComponent<Draggable>().enabled = true;
                }
                break;

            case GameState.NPC2TURN:
                YourTurn.SetActive(false);
                PlaceNpcTile(npc2Tile);
                break;

            case GameState.NPC3TURN:
                PlaceNpcTile(npc3Tile);
                break;

            case GameState.ROUNDVERDICT:
                // Activate EvaluationObjects during the ROUNDVERDICT state
                if (EvaluationObjects != null)
                {
                    EvaluationObjects.SetActive(true);
                }
                Debug.Log("Round Verdict: Evaluate results.");
                break;
            case GameState.REEVALUATION:
                Debug.Log("Reevaluation state started.");
                // Enable the TileClickHandler on tiles that are in the empty slots or have been placed by the NPC/player
                foreach (GameObject tile in playerTiles)
                {
                    // Make sure the tile is active and in a valid position to be removed
                    tile.GetComponent<TileClickHandler>().enabled = true;  // Enable the TileClickHandler so the player can remove it
                }
                break;
            case GameState.REPLACE:
                foreach (GameObject tile in playerTiles)
                {
                    tile.GetComponent<Draggable>().enabled = true;
                }
                break;
        }
    }

    private void PlaceNpcTile(GameObject npcTile)
    {
        // Filter out slots that are already occupied by player tiles
        List<GameObject> availableSlots = new List<GameObject>(emptySlots);

        foreach (GameObject playerTile in playerTiles)
        {
            // Remove any empty slots that overlap with player tiles
            availableSlots.RemoveAll(slot => slot.transform.position == playerTile.transform.position);
        }

        // If there are available slots, place the NPC tile on a random one
        if (availableSlots.Count > 0)
        {
            GameObject randomSlot = availableSlots[Random.Range(0, availableSlots.Count)];
            npcTile.transform.position = randomSlot.transform.position;

            // Remove the occupied slot from emptySlots
            emptySlots.Remove(randomSlot);

            npcTile.SetActive(true);  // Activate the NPC tile at the chosen position
        }
        else
        {
            Debug.Log("No available slots for NPC tile placement.");
        }
    }

    // This method will be called when the player clicks on the NO GameObject
    private void OnMouseDown()
    {
        if (currentState == GameState.ROUNDVERDICT && EvaluationObjects != null)
        {
            // Deactivate all EvaluationObjects
            EvaluationObjects.SetActive(false);
            Debug.Log("Evaluation Objects deactivated.");
        }
    }
}
