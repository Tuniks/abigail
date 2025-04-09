using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float revealDelay = 2f;  // Time the assigned object is shown

    private int tilesPlayed = 0;
    public int maxTilesToPlay = 5;

    public ClayEnemyHand clayEnemyHand;

    // Instead of UI objects, these are now sprite GameObjects with SpriteRenderer and Animator components.
    public GameObject playerVictorySprite;
    public GameObject enemyVictorySprite;

    // Assign the active arenas in the Inspector
    public Arena[] arenas;

    // New enemy thinking UI element (assign in the Inspector)
    public GameObject enemyThinkingUI;

    public string nextScene = "CLAY_area_post";

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
        if(playerVictorySprite != null)
            playerVictorySprite.SetActive(false);
        if(enemyVictorySprite != null)
            enemyVictorySprite.SetActive(false);
        if(enemyThinkingUI != null)
            enemyThinkingUI.SetActive(false);
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
        // Wait for the reveal period.
        yield return new WaitForSeconds(revealDelay);

        if(assignedObject != null)
            assignedObject.SetActive(false);

        // Enable enemy thinking sprite before the enemy plays their tile.
        if(enemyThinkingUI != null)
            enemyThinkingUI.SetActive(true);

        // Wait for a random "thinking" delay between 4 and 7 seconds.
        float thinkingDelay = Random.Range(1f, 2f);
        yield return new WaitForSeconds(thinkingDelay);

        if(enemyThinkingUI != null)
            enemyThinkingUI.SetActive(false);

        if(clayEnemyHand != null)
            clayEnemyHand.PlayNextTile();

        if(tilesPlayed >= maxTilesToPlay) {
            currentState = GameState.Ended;
            EvaluateWinner();
            StartCoroutine(OnGameEnd());
        } else {
            currentState = GameState.PlayerTurn;
        }
    }

    void EvaluateWinner() {
        int playerWins = 0;
        int enemyWins = 0;

        // Each arena returns: -1 for player win, 1 for enemy win, 0 for tie.
        foreach(Arena arena in arenas) {
            int result = arena.GetWinner();
            if(result < 0)
                playerWins++;
            else if(result > 0)
                enemyWins++;
        }

        // Enable the appropriate victory sprite based on the majority.
        if(playerWins > enemyWins) {
            if(playerVictorySprite != null)
                playerVictorySprite.SetActive(true);
            if(enemyVictorySprite != null)
                enemyVictorySprite.SetActive(false);
        } else if(enemyWins > playerWins) {
            if(enemyVictorySprite != null)
                enemyVictorySprite.SetActive(true);
            if(playerVictorySprite != null)
                playerVictorySprite.SetActive(false);
        } else {
            // In case of a tie, you may choose one or a neutral option.
            if(enemyVictorySprite != null)
                enemyVictorySprite.SetActive(true);
            if(playerVictorySprite != null)
                playerVictorySprite.SetActive(false);
        }
    }

    IEnumerator OnGameEnd(){
        yield return new WaitForSeconds(4);
        if(SceneFader.Instance == null){
            SceneManager.LoadScene(nextScene);

        } else SceneFader.Instance.ChangeScene(nextScene);
    }
}
