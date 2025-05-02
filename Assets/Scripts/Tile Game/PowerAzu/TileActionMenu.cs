using UnityEngine;

public class TileActionMenu : MonoBehaviour {
    public static TileActionMenu instance;

    public GameObject menuUIPrefab;
    public AudioClip cycleSFX;
    public AudioClip selectSFX;
    public AudioSource audioSource;

    private GameObject currentMenuUI;
    private Transform[] icons = new Transform[3];
    private int currentIndex = 0;

    private float selectedScale = 1.4f;
    private float normalScale = 1f;
    public float iconOffsetY = 1.5f;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        if (currentMenuUI == null) return;

        // Keyboard input
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            CycleIndex(1);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            CycleIndex(-1);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            SelectIcon(currentIndex);
        }

        // Mouse hover detection
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null && currentMenuUI != null) {
            for (int i = 0; i < 3; i++) {
                if (hit.collider.gameObject == icons[i].gameObject) {
                    if (i != currentIndex) {
                        currentIndex = i;
                        UpdateIconSelection();
                        PlaySound(cycleSFX);
                    }

                    if (Input.GetMouseButtonDown(0)) {
                        SelectIcon(currentIndex);
                    }
                }
            }
        }

        // Update menu position relative to tile and screen
        if (PowerAzuManager.instance.activeTile != null && currentMenuUI != null) {
            Vector3 worldPos = PowerAzuManager.instance.activeTile.transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            float screenHalf = Screen.height / 2f;
            float offset = (screenPos.y > screenHalf) ? -iconOffsetY : iconOffsetY;

            currentMenuUI.transform.position = worldPos + Vector3.up * offset;
        }
    }

    private void CycleIndex(int direction) {
        currentIndex = (currentIndex + direction + 3) % 3;
        UpdateIconSelection();
        PlaySound(cycleSFX);
    }

    public void ShowMenu(Tile tile) {
        HideMenu();
        if (menuUIPrefab == null || tile == null) return;

        Vector3 worldPos = tile.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        float screenHalf = Screen.height / 2f;
        float offset = (screenPos.y > screenHalf) ? -iconOffsetY : iconOffsetY;

        currentMenuUI = Instantiate(menuUIPrefab, worldPos + Vector3.up * offset, Quaternion.identity);

        for (int i = 0; i < 3; i++) {
            icons[i] = currentMenuUI.transform.Find("Icon" + i);
        }

        if (icons[1] != null) {
            SpriteRenderer sr = icons[1].GetComponent<SpriteRenderer>();
            if (sr != null && tile.tilePower != null && tile.tilePower.Icon != null) {
                sr.sprite = tile.tilePower.Icon;
                sr.enabled = true;
            }
        }

        currentIndex = 0;
        UpdateIconSelection();
    }

    public void HideMenu() {
        if (currentMenuUI != null) {
            Destroy(currentMenuUI);
            currentMenuUI = null;
        }
    }

    private void UpdateIconSelection() {
        for (int i = 0; i < 3; i++) {
            if (icons[i] != null) {
                icons[i].localScale = (i == currentIndex) ? Vector3.one * selectedScale : Vector3.one * normalScale;
            }
        }
    }

    private void SelectIcon(int index) {
        PlaySound(selectSFX);
        switch (index) {
            case 0:
                PowerAzuManager.instance.EnterLaunchMode();
                HideMenu();
                break;
            case 1:
                if (PowerAzuManager.instance.activeTile?.tilePower != null) {
                    PowerAzuManager.instance.activeTile.tilePower.Activate(PowerAzuManager.instance.activeTile);
                }
                HideMenu();
                break;
            case 2:
                PowerAzuManager.instance.CancelActiveTile();
                break;
        }
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}
