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
        if (audioSource && playerScoreClip) audioSource.PlayOneShot(playerScoreClip);
        if (playerScoreText) StartCoroutine(AnimateBounce(playerScoreText.transform));

        if (playerScore >= winningScore) {
            gameEnded = true;
            EndgameManager.instance.TriggerEndgame(true);
        }
    }

    public void AddScoreToEnemy(int points) {
        if (gameEnded) return;

        enemyScore += points;
        UpdateEnemyUI();
        if (audioSource && enemyScoreClip) audioSource.PlayOneShot(enemyScoreClip);
        if (enemyScoreText) StartCoroutine(AnimateBounce(enemyScoreText.transform));

        if (enemyScore >= winningScore) {
            gameEnded = true;
            EndgameManager.instance.TriggerEndgame(false);
        }
    }

    private void UpdatePlayerUI() {
        if (playerScoreText)
            playerScoreText.text = Mathf.FloorToInt(playerScore).ToString();
    }

    private void UpdateEnemyUI() {
        if (enemyScoreText)
            enemyScoreText.text = Mathf.FloorToInt(enemyScore).ToString();
    }

    private IEnumerator AnimateBounce(Transform target) {
        Vector3 original = target.localScale;
        Vector3 enlarged = original * bounceScale;
        float half = bounceDuration * 0.5f;

        float t = 0f;
        while (t < half) {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(original, enlarged, t / half);
            yield return null;
        }

        t = 0f;
        while (t < half) {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(enlarged, original, t / half);
            yield return null;
        }
    }
}
