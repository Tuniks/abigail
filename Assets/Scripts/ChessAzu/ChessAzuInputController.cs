using System.Collections.Generic;
using UnityEngine;

public class ChessAzuInputController : MonoBehaviour
{
    [Header("References")]
    public ChessAzuManager board;    // grid/cells
    public ChessAzuGame game;        // rules/turns/captures
    public Camera worldCamera;

    [Header("Selection Tints (override occupancy while selected)")]
    public Color moveTint         = new Color(0.2f, 0.9f, 0.6f, 1f);
    public Color captureTint      = new Color(1.0f, 0.25f, 0.25f, 1f);

    [Header("Hover Tints (PLAYER piece under cursor)")]
    public Color hoverMoveTintPlayer    = new Color(0.2f, 0.9f, 0.6f, 0.55f);
    public Color hoverCaptureTintPlayer = new Color(1.0f, 0.25f, 0.25f, 0.55f);

    [Header("Hover Tints (ENEMY piece under cursor)")]
    public Color hoverMoveTintEnemy    = new Color(0.3f, 0.6f, 1.0f, 0.55f);
    public Color hoverCaptureTintEnemy = new Color(1.0f, 0.4f, 0.0f, 0.55f);

    private ChessAzuPiece selectedPiece;
    private readonly HashSet<Vector2Int> moveCells    = new();
    private readonly HashSet<Vector2Int> captureCells = new();

    // Hover state
    private ChessAzuPiece hoverPiece;
    private Vector2Int hoverCell = new Vector2Int(-999, -999);
    private readonly HashSet<Vector2Int> hoverTinted = new();

    void Awake()
    {
        if (board == null) board = FindFirstObjectByType<ChessAzuManager>();
        if (game  == null) game  = FindFirstObjectByType<ChessAzuGame>();
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
            if (worldCamera == null)
                foreach (var cam in Camera.allCameras) if (cam.enabled) { worldCamera = cam; break; }
        }
    }

    void OnDisable()
    {
        ClearSelectionTints();
        ClearHoverTints();
        selectedPiece = null;
        // repaint base occupancy when controller goes away
        game?.ApplyOccupancyTints();
    }

    void Update()
    {
            if (board == null || !board.IsGridReady() || game == null) return;
            if (game.IsGameOver) return; // <- stop interaction after win/lose



        // Hover preview (suppressed when selecting)
        if (selectedPiece == null) UpdateHover();
        else ClearHoverTints();

        // Left click = select or move
        if (Input.GetMouseButtonDown(0)) HandleLeftClick();

        // Right click = clear selection + hover, restore base
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
            ClearHoverTints();
            game.ApplyOccupancyTints();
        }
    }

    // ---------- Hover ----------
    private void UpdateHover()
    {
        Vector3 world = GetWorldMouse();
        if (!board.TryWorldToCell(world, out int cx, out int cy))
        {
            ClearHoverTints();
            game.ApplyOccupancyTints();
            return;
        }

        var cell = new Vector2Int(cx, cy);
        if (cell == hoverCell) return;
        hoverCell = cell;

        var p = game.GetPieceAt(cell);
        if (p == null)
        {
            ClearHoverTints();
            game.ApplyOccupancyTints();
            return;
        }
        if (p == hoverPiece) return;

        hoverPiece = p;
        ClearHoverTints();

        // Start from base occupancy, then overlay hover tints on legal cells
        game.ApplyOccupancyTints();

        var opts = game.GetLegalMoves(p);
        bool isPlayer = p.isPlayerTile;
        var hoverMove    = isPlayer ? hoverMoveTintPlayer    : hoverMoveTintEnemy;
        var hoverCapture = isPlayer ? hoverCaptureTintPlayer : hoverCaptureTintEnemy;

        foreach (var opt in opts)
        {
            var tint = opt.isCapture ? hoverCapture : hoverMove;
            board.SetCellTint(opt.cell.x, opt.cell.y, tint);
            hoverTinted.Add(opt.cell);
        }
    }

    private void ClearHoverTints()
    {
        foreach (var c in hoverTinted)
            board.ClearCellTint(c.x, c.y);
        hoverTinted.Clear();
        hoverPiece = null;
        hoverCell = new Vector2Int(-999, -999);
    }

    // ---------- Clicks ----------
    private void HandleLeftClick()
    {
        Vector3 world = GetWorldMouse();
        if (!board.TryWorldToCell(world, out int cx, out int cy))
        {
            Deselect();
            game.ApplyOccupancyTints();
            return;
        }
        var cell = new Vector2Int(cx, cy);

        // Move if clicking a selected highlight
        if (selectedPiece != null && (moveCells.Contains(cell) || captureCells.Contains(cell)))
        {
            var mover = selectedPiece;
            mover.GetComponent<AzuPieceJuice>()?.StopShake();

            Deselect();          // clears selection tints
            ClearHoverTints();   // safety
            game.ApplyOccupancyTints();

            game.TryApplyMove(mover, cell); // this will also repaint occupancy
            return;
        }

        // Select only current side
        var clicked = game.GetPieceAt(cell);
        if (clicked != null && game.IsPieceOnCurrentSide(clicked))
        {
            SelectPiece(clicked);
            return;
        }

        Deselect();
        game.ApplyOccupancyTints();
    }

    private void SelectPiece(ChessAzuPiece piece)
    {
        if (piece == null || !game.IsPieceOnCurrentSide(piece)) { Deselect(); game.ApplyOccupancyTints(); return; }

        ClearSelectionTints();
        ClearHoverTints();

        selectedPiece = piece;
        piece.GetComponent<AzuPieceJuice>()?.PlaySelectShake();

        // Start from base occupancy, then overlay selection options
        game.ApplyOccupancyTints();

        var options = game.GetLegalMoves(piece);
        foreach (var opt in options)
        {
            var tint = opt.isCapture ? captureTint : moveTint;
            board.SetCellTint(opt.cell.x, opt.cell.y, tint);

            if (opt.isCapture) captureCells.Add(opt.cell);
            else               moveCells.Add(opt.cell);
        }
    }

    private void Deselect()
    {
        selectedPiece = null;
        ClearSelectionTints();
    }

    private void ClearSelectionTints()
    {
        foreach (var c in moveCells)    board.ClearCellTint(c.x, c.y);
        foreach (var c in captureCells) board.ClearCellTint(c.x, c.y);
        moveCells.Clear();
        captureCells.Clear();
    }

    // ---------- Helpers ----------
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
}
