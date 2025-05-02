using System.Collections;
using UnityEngine;
using TMPro;

public class TargetManager : MonoBehaviour {
    public static TargetManager instance;

    [Header("Score Display")]
    public TMP_Text playerScoreText;
    public TMP_Text enemyScoreText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip playerScoreClip;
    public AudioClip enemyScoreClip;

    [Header("Bounce Animation")]
    public float bounceScale = 1.2f;
    public float bounceDuration = 0.2f;

    [Header("Win Settings")]
    public int winningScore = 10;
    public GameObject playerWinSpritePrefab;
    public GameObject enemyWinSpritePrefab;
    public GameObject objectToDisable;
    public Canvas endgameUICanvas;

    private float playerScore = 0f;
    private float enemyScore = 0f;
    private bool gameEnded = false;

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddScoreToPlayer(int points) {
        if (gameEnded) return;

        playerScore += points;
        UpdatePlayerUI();

        if (audioSource != null && playerScoreClip != null)
            audioSource.PlayOneShot(playerScoreClip);

        if (playerScoreText != null)
            StartCoroutine(AnimateBounce(playerScoreText.transform));

        if (playerScore >= winningScore)
            TriggerEndgame(true);
    }

    public void AddScoreToEnemy(int points) {
        if (gameEnded) return;

        enemyScore += points;
        UpdateEnemyUI();

        if (audioSource != null && enemyScoreClip != null)
            audioSource.PlayOneShot(enemyScoreClip);

        if (enemyScoreText != null)
            StartCoroutine(AnimateBounce(enemyScoreText.transform));

        if (enemyScore >= winningScore)
            TriggerEndgame(false);
    }

    private void UpdatePlayerUI() {
        if (playerScoreText != null)
            playerScoreText.text = Mathf.FloorToInt(playerScore).ToString();
    }

    private void UpdateEnemyUI() {
        if (enemyScoreText != null)
            enemyScoreText.text = Mathf.FloorToInt(enemyScore).ToString();
    }

    private IEnumerator AnimateBounce(Transform target) {
        Vector3 original = target.localScale;
        Vector3 enlarged = original * bounceScale;
        float half = bounceDuration * 0.5f;
        float t = 0f;

        // Scale up
        while (t < half) {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(original, enlarged, t / half);
            yield return null;
        }

        // Scale down
        t = 0f;
        while (t < half) {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(enlarged, original, t / half);
            yield return null;
        }
    }

    private void TriggerEndgame(bool playerWon) {
        gameEnded = true;

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        Vector3 spawnPos = playerWon && playerScoreText != null
            ? playerScoreText.transform.position
            : enemyScoreText.transform.position;

        GameObject prefab = playerWon ? playerWinSpritePrefab : enemyWinSpritePrefab;
        if (prefab != null)
            StartCoroutine(SpawnWinSprite(prefab, spawnPos));

        StartCoroutine(WaitForClickThenShowUI());
    }

    private IEnumerator SpawnWinSprite(GameObject prefab, Vector3 position) {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        Transform trans = obj.transform;

        Vector3 start = Vector3.one * 0.5f;
        Vector3 end = Vector3.one * 1.2f;
        float duration = 0.4f;
        float t = 0f;

        trans.localScale = start;

        while (t < duration) {
            t += Time.deltaTime;
            trans.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
    }

    private IEnumerator WaitForClickThenShowUI() {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E));

        if (endgameUICanvas != null)
            endgameUICanvas.enabled = true;
    }
}
