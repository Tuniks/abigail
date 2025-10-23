using System.Collections.Generic;
using UnityEngine;

public class ChessAzuInputController : MonoBehaviour
{
    [Header("References")]
    public ChessAzuManager board;    // your existing grid builder
    public ChessAzuGame game;        // rules/turns/captures
    public Camera worldCamera;

    [Header("Highlight Overlay")]
    public GameObject highlightPrefab;
    public Color moveHighlightColor = new Color(0.2f, 0.9f, 0.6f, 0.6f);
    public Color captureHighlightColor = new Color(1.0f, 0.25f, 0.25f, 0.6f);
    public int highlightOrderOffset = 10;

    [Header("Hover Preview (lighter)")]
    public Color hoverMoveColor = new Color(0.2f, 0.9f, 0.6f, 0.28f);
    public Color hoverCaptureColor = new Color(1.0f, 0.25f, 0.25f, 0.28f);

    private ChessAzuPiece selectedPiece;
    private readonly List<GameObject> activeOverlays = new();
    private readonly HashSet<Vector2Int> moveCells = new();
    private readonly HashSet<Vector2Int> captureCells = new();

    // Hover state
    private ChessAzuPiece hoverPiece;
    private Vector2Int hoverCell = new Vector2Int(-999, -999);
    private readonly List<GameObject> hoverOverlays = new();

    void Awake()
    {
        if (board == null) board = FindFirstObjectByType<ChessAzuManager>();
        if (game == null) game = FindFirstObjectByType<ChessAzuGame>();
        if (worldCamera == null) worldCamera = Camera.main;
        if (worldCamera == null)
        {
            foreach (var cam in Camera.allCameras) if (cam.enabled) { worldCamera = cam; break; }
        }
    }

    void OnDisable()
    {
        ClearHighlights();
        ClearHover();
        selectedPiece = null;
        hoverPiece = null;
    }

    void Update()
    {
        if (board == null || !board.IsGridReady() || game == null) return;

        // --- Hover preview (suppressed when a piece is selected) ---
        if (selectedPiece == null)
            UpdateHoverPreview();
        else
            ClearHover(); // ensure no hover overlays while selected

        // --- Click handling ---
        if (Input.GetMouseButtonDown(0)) HandleLeftClick();
        else if (Input.GetMouseButtonDown(1)) Deselect();
    }

    // ---------------- Hover ----------------

    private void UpdateHoverPreview()
    {
        Vector3 world = GetWorldMouse();
        if (!board.TryWorldToCell(world, out int cx, out int cy))
        {
            ClearHover();
            return;
        }

        var cell = new Vector2Int(cx, cy);
        if (cell == hoverCell) return; // no change

        hoverCell = cell;
        var piece = game.GetPieceAt(cell);

        // If no piece under mouse, clear overlays
        if (piece == null)
        {
            ClearHover();
            return;
        }

        // If same piece as last frame, no rebuild
        if (piece == hoverPiece) return;

        // Rebuild hover overlays
        hoverPiece = piece;
        ClearHover();

        var opts = game.GetLegalMoves(piece);
        foreach (var opt in opts)
            CreateHoverOverlay(opt.cell, opt.isCapture ? hoverCaptureColor : hoverMoveColor);
    }

    private void ClearHover()
    {
        for (int i = 0; i < hoverOverlays.Count; i++)
            if (hoverOverlays[i] != null) Destroy(hoverOverlays[i]);
        hoverOverlays.Clear();
        hoverPiece = null;
        hoverCell = new Vector2Int(-999, -999);
    }

    // ---------------- Click handling ----------------

    private void HandleLeftClick()
    {
        Vector3 world = GetWorldMouse();
        if (!board.TryWorldToCell(world, out int cx, out int cy))
        {
            Deselect();
            return;
        }

        var cell = new Vector2Int(cx, cy);

        // If we have a selection and clicked a highlighted cell → apply via game
        if (selectedPiece != null && (moveCells.Contains(cell) || captureCells.Contains(cell)))
        {
            var mover = selectedPiece;
            Deselect(); // clear overlays & selection first (also clears hover)
            game.TryApplyMove(mover, cell);
            return;
        }

        // Select only a piece from the current side
        var clicked = game.GetPieceAt(cell);
        if (clicked != null && game.IsPieceOnCurrentSide(clicked))
        {
            SelectPiece(clicked);
            return;
        }

        Deselect();
    }

    // ---------------- Selection & highlights ----------------

    private void SelectPiece(ChessAzuPiece piece)
    {
        if (piece == null || !game.IsPieceOnCurrentSide(piece)) { Deselect(); return; }

        ClearHighlights();
        ClearHover(); // if you kept hover

        selectedPiece = piece;

        // 👇 NEW: little shake when selected
        piece.GetComponent<AzuPieceJuice>()?.PlaySelectShake();

        var options = game.GetLegalMoves(piece);
        foreach (var opt in options)
            CreateMainOverlay(opt.cell, opt.isCapture ? captureHighlightColor : moveHighlightColor);
    }

    private void Deselect()
    {
        selectedPiece = null;
        ClearHighlights();
        // do not clear hover here; we'll rebuild hover next Update based on cursor
    }

    // ---------------- Overlays ----------------

    private void ClearHighlights()
    {
        foreach (var go in activeOverlays) if (go != null) Destroy(go);
        activeOverlays.Clear();
        moveCells.Clear();
        captureCells.Clear();
    }

    private void CreateMainOverlay(Vector2Int cell, Color color)
    {
        var go = CreateOverlayOnCell(cell, color);
        if (go != null) activeOverlays.Add(go);

        // Track for click validation
        if (Approximately(color, captureHighlightColor)) captureCells.Add(cell);
        else moveCells.Add(cell);
    }

    private void CreateHoverOverlay(Vector2Int cell, Color color)
    {
        var go = CreateOverlayOnCell(cell, color);
        if (go != null) hoverOverlays.Add(go);
    }

    private GameObject CreateOverlayOnCell(Vector2Int cell, Color color)
    {
        var cellGO = board.GetCell(cell.x, cell.y);
        if (cellGO == null) return null;

        GameObject overlay;
        if (highlightPrefab != null)
        {
            overlay = Instantiate(highlightPrefab, cellGO.transform);
            var sr = overlay.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
        else
        {
            overlay = new GameObject("CellOverlay");
            overlay.transform.SetParent(cellGO.transform, false);
            var cellSR = cellGO.GetComponentInChildren<SpriteRenderer>();
            var sr = overlay.AddComponent<SpriteRenderer>();
            if (cellSR != null)
            {
                sr.sprite = cellSR.sprite;
                sr.drawMode = cellSR.drawMode;
                sr.size = cellSR.size;
                sr.sortingLayerID = cellSR.sortingLayerID;
                sr.sortingOrder = cellSR.sortingOrder + highlightOrderOffset;
            }
            else
            {
                sr.sortingOrder = highlightOrderOffset;
            }
            sr.color = color;
        }

        overlay.transform.localPosition = Vector3.zero;
        overlay.transform.localRotation = Quaternion.identity;
        overlay.transform.localScale = Vector3.one;
        return overlay;
    }

    // ---------------- Helpers ----------------

    private Vector3 GetWorldMouse()
    {
        if (worldCamera == null) return new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        if (worldCamera.orthographic)
        {
            var w = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            w.z = 0f;
            return w;
        }
        else
        {
            var ray = worldCamera.ScreenPointToRay(Input.mousePosition);
            float t = 0f;
            if (Mathf.Abs(ray.direction.z) > 1e-5f) t = -ray.origin.z / ray.direction.z;
            return ray.origin + ray.direction * Mathf.Max(0f, t);
        }
    }

    private static bool Approximately(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
    }
}
