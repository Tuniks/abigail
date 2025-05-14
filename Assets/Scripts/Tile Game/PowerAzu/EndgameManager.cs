using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameManager : MonoBehaviour {
    public static EndgameManager instance;

    public GameObject playerWinSpritePrefab;
    public GameObject enemyWinSpritePrefab;
    public Canvas endgameUICanvas;
    public Transform playerScorePosition;
    public Transform enemyScorePosition;
    public string nextSceneName;

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void TriggerEndgame(bool playerWon) {
        Vector3 spawnPos = playerWon ? playerScorePosition.position : enemyScorePosition.position;
        GameObject prefab = playerWon ? playerWinSpritePrefab : enemyWinSpritePrefab;

        if (prefab) StartCoroutine(SpawnWinSprite(prefab, spawnPos));
        StartCoroutine(WaitForPressToContinue());
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

    private IEnumerator WaitForPressToContinue() {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        if (endgameUICanvas) endgameUICanvas.enabled = true;
        if (!string.IsNullOrEmpty(nextSceneName)) SceneManager.LoadScene(nextSceneName);
    }
}