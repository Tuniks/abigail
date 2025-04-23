using UnityEngine;
using UnityEngine.UI;


public class TileActionMenu : MonoBehaviour {
    public static TileActionMenu instance;

    public GameObject menuUIPrefab;
    private GameObject currentMenuUI;
    private int currentIndex = 0;
    private Transform[] icons = new Transform[3];

    private float selectedScale = 1.4f;
    private float normalScale = 1f;
    private float iconOffsetY = 1.5f;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        if (currentMenuUI == null) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentIndex = (currentIndex + 1) % 3;
            UpdateIconSelection();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentIndex = (currentIndex + 2) % 3;
            UpdateIconSelection();
        } else if (Input.GetKeyDown(KeyCode.E)) {
            switch (currentIndex) {
                case 0:
                    PowerAzuManager.instance.EnterLaunchMode();
                    HideMenu();
                    break;
                case 1:
                    if (PowerAzuManager.instance.activeTile != null && PowerAzuManager.instance.activeTile.tilePower != null) {
                        PowerAzuManager.instance.activeTile.tilePower.Activate(PowerAzuManager.instance.activeTile);
                    }
                    HideMenu();
                    break;
                case 2:
                    PowerAzuManager.instance.CancelActiveTile();
                    break;
            }
        }

        if (PowerAzuManager.instance.activeTile != null && currentMenuUI != null) {
            Vector3 targetPos = PowerAzuManager.instance.activeTile.transform.position + Vector3.up * iconOffsetY;
            currentMenuUI.transform.position = targetPos;
        }
    }

    public void ShowMenu(Tile tile) {
        HideMenu();

        if (menuUIPrefab == null || tile == null) return;

        Vector3 spawnPos = tile.transform.position + Vector3.up * iconOffsetY;
        currentMenuUI = Instantiate(menuUIPrefab, spawnPos, Quaternion.identity);

        for (int i = 0; i < 3; i++) {
            icons[i] = currentMenuUI.transform.Find("Icon" + i);
        }
        
        if (icons[1] != null) {
            Image iconImage = icons[1].GetComponent<Image>();
            if (iconImage != null && tile.tilePower != null && tile.tilePower.Icon != null) {
                iconImage.sprite = tile.tilePower.Icon;
                iconImage.enabled = true;
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
        for (int i = 0; i < icons.Length; i++) {
            if (icons[i] != null) {
                icons[i].localScale = (i == currentIndex) ? Vector3.one * selectedScale : Vector3.one * normalScale;
            }
        }
    }
}