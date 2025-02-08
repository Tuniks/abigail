using System.Collections;
using UnityEngine;

public class ClayGameManager : MonoBehaviour {
    public static ClayGameManager instance;

    public enum GameState {
        PlayerTurn,
        Revealing,
        Ended
    }
    public GameState currentState;

    public PlayerHand playerHand;
    public GameObject assignedObject;
    public float revealDelay = 2f;

    private int tilesPlayed = 0;
    public int maxTilesToPlay = 5;

    // Renamed reference for the enemy hand
    public ClayEnemyHand clayEnemyHand;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        currentState = GameState.PlayerTurn;
        if(assignedObject != null)
            assignedObject.SetActive(false);
    }

    // Called when a tile is placed in Clay mode.
    public void OnTilePlaced() {
        if(currentState != GameState.PlayerTurn)
            return;

        currentState = GameState.Revealing;
        if(assignedObject != null)
            assignedObject.SetActive(true);

        tilesPlayed++;
        StartCoroutine(ReenablePlayer());
    }

    IEnumerator ReenablePlayer() {
        yield return new WaitForSeconds(revealDelay);

        if(assignedObject != null)
            assignedObject.SetActive(false);

        if(clayEnemyHand != null)
            clayEnemyHand.PlayNextTile();

        if(tilesPlayed >= maxTilesToPlay) {
            currentState = GameState.Ended;
            Debug.Log("Game Over. Tiles played: " + tilesPlayed);
        } else {
            currentState = GameState.PlayerTurn;
        }
    }
}