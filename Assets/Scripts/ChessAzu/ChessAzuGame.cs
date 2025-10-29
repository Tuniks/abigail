using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-150)] // after ChessAzuManager (-200)
public class ChessAzuGame : MonoBehaviour
{
    public enum Side { Player, Enemy }

    [Header("Board")]
    public ChessAzuManager board;

    [Header("Turn State")]
    public Side currentTurn = Side.Player;

    [Header("Capture Bins")]
    public AzuCaptureBin playerCaptureBin;
    public AzuCaptureBin enemyCaptureBin;

    [Header("Enemy Automation")]
    public bool enemyAutoPlay = true;

    [Header("Occupancy Tints")]
    [Tooltip("Tint applied to cells containing a Player piece.")]
    public Color playerOccupancyTint = new Color(1.0f, 0.95f, 0.5f, 1f);  // soft yellow
    [Tooltip("Tint applied to cells containing an Enemy piece.")]
    public Color enemyOccupancyTint  = new Color(1.0f, 0.3f, 0.3f, 1f);   // default red

    [Header("End Game UI")]
    public GameObject winUI;   // shown when all enemy tiles captured
    public GameObject loseUI;  // shown when all player tiles captured

    [Header("Ripple Juice")]
    [Tooltip("Color of the ripple when the PLAYER captures / moves.")]
    public Color playerRippleColor = new Color(0.95f, 0.85f, 0.25f, 1f);
    [Tooltip("Color of the ripple when the ENEMY captures / moves.")]
    public Color enemyRippleColor  = new Color(1.0f, 0.35f, 0.2f, 1f);

    [Tooltip("How strongly to blend the ripple color at its peak (0..1).")]
    [Range(0f, 1f)] public float rippleMaxStrength = 0.6f;

    [Tooltip("How long each cell stays affected once the wave arrives (seconds).")]
    public float rippleCellDuration = 0.35f;

    [Tooltip("Delay per Manhattan step from the origin (seconds per cell).")]
    public float rippleCellDelay = 0.05f;

    [Tooltip("Global time scale for the ripple (1 = normal).")]
    public float rippleTimeScale = 1f;

    // grid cell -> piece
    private readonly Dictionary<Vector2Int, ChessAzuPiece> occ = new Dictionary<Vector2Int, ChessAzuPiece>();

    private bool gameOver = false;
    public bool IsGameOver => gameOver;

    private Coroutine rippleCo;

    void Awake()
    {
        if (board == null) board = FindFirstObjectByType<ChessAzuManager>();

        // Ensure UI starts hidden (if assigned)
        if (winUI  != null) winUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);

        RebuildOccupancy();
        ApplyOccupancyTints();
        EvaluateEndGame(); // in case scene starts terminal
    }

    // ---------- Occupancy ----------
    public void RebuildOccupancy()
    {
        occ.Clear();
        var pieces = FindObjectsOfType<ChessAzuPiece>();
        foreach (var p in pieces)
        {
            if (!p.enabled || !p.gameObject.activeInHierarchy) continue;
            occ[p.GetGridPosition()] = p;
        }
    }

    public ChessAzuPiece GetPieceAt(Vector2Int cell)
    {
        occ.TryGetValue(cell, out var p);
        return p;
    }

    public static bool IsFriendly(ChessAzuPiece a, ChessAzuPiece b)
    {
        return (a.isPlayerTile && b.isPlayerTile) || (!a.isPlayerTile && !b.isPlayerTile);
    }

    public bool IsPieceOnCurrentSide(ChessAzuPiece p)
    {
        if (gameOver) return false;
        return (currentTurn == Side.Player && p.isPlayerTile) ||
               (currentTurn == Side.Enemy  && !p.isPlayerTile);
    }

    // ---------- Legal moves ----------
    public struct MoveOption
    {
        public Vector2Int cell;
        public bool isCapture;
        public ChessAzuPiece captured; // null if not capture
    }

    public List<MoveOption> GetLegalMoves(ChessAzuPiece piece)
    {
        var list = new List<MoveOption>();
        if (piece == null || board == null || !board.IsGridReady()) return list;

        foreach (var m in piece.GetAllowedMoves())
        {
            var target = new Vector2Int(m.x, m.y);
            var other = GetPieceAt(target);

            if (other == null)
                list.Add(new MoveOption { cell = target, isCapture = false, captured = null });
            else if (!IsFriendly(piece, other))
                list.Add(new MoveOption { cell = target, isCapture = true, captured = other });
        }
        return list;
    }

    // ---------- Apply move/capture + turns ----------
    public bool TryApplyMove(ChessAzuPiece piece, Vector2Int targetCell)
    {
        if (gameOver) return false;
        if (piece == null) return false;
        if (!IsPieceOnCurrentSide(piece)) return false;

        MoveOption? chosen = null;
        var options = GetLegalMoves(piece);
        for (int i = 0; i < options.Count; i++)
            if (options[i].cell == targetCell) { chosen = options[i]; break; }
        if (chosen == null) return false;

        // Capture first (if any)
        if (chosen.Value.isCapture && chosen.Value.captured != null)
        {
            var cap = chosen.Value.captured;

            // prevent captured piece from acting again
            cap.enabled = false;

            // pop + then move to capture bin at midpoint
            var juice = cap.GetComponent<AzuPieceJuice>();
            System.Action mid = () =>
            {
                if (piece.isPlayerTile)
                {
                    if (playerCaptureBin != null) playerCaptureBin.AddCaptured(cap);
                    else cap.gameObject.SetActive(false);
                }
                else
                {
                    if (enemyCaptureBin != null) enemyCaptureBin.AddCaptured(cap);
                    else cap.gameObject.SetActive(false);
                }
            };

            if (juice != null) juice.PlayCapturePop(null, null, onMidpoint: mid, onFinish: null);
            else               mid();
        }

        // Move the mover
        piece.TryMoveTo(targetCell.x, targetCell.y);

        // Refresh state + repaint base occupancy
        RebuildOccupancy();
        ApplyOccupancyTints();

        // Trigger ripple from the capturing/acting piece's new square
        StartRipple(origin: targetCell, color: piece.isPlayerTile ? playerRippleColor : enemyRippleColor);

        // Check for win/lose (still allow ripple to play visually)
        if (EvaluateEndGame())
            return true;

        // Flip turn (+ simple enemy auto)
        FlipTurn();
        return true;
    }

    public void FlipTurn()
    {
        if (gameOver) return;

        currentTurn = (currentTurn == Side.Player) ? Side.Enemy : Side.Player;
        if (enemyAutoPlay && currentTurn == Side.Enemy)
            DoEnemyAutoMove();
    }

    // ---------- very simple enemy auto (prefer capture) ----------
    private void DoEnemyAutoMove()
    {
        if (gameOver) return;

        var all = FindObjectsOfType<ChessAzuPiece>();
        List<(ChessAzuPiece piece, List<MoveOption> moves)> cands = new();

        foreach (var p in all)
        {
            if (!p.enabled || !p.gameObject.activeInHierarchy) continue;
            if (p.isPlayerTile) continue; // enemy only
            var moves = GetLegalMoves(p);
            if (moves.Count > 0) cands.Add((p, moves));
        }

        if (cands.Count == 0)
        {
            EvaluateEndGame();
            currentTurn = Side.Player;
            return;
        }

        // Greedy capture if available
        foreach (var c in cands)
        {
            var cap = c.moves.Find(m => m.isCapture);
            if (cap.captured != null)
            {
                TryApplyMove(c.piece, cap.cell);
                return;
            }
        }

        // Else first legal move
        TryApplyMove(cands[0].piece, cands[0].moves[0].cell);
    }

    // ---------- Base (persistent) occupancy tints ----------
    public void ApplyOccupancyTints()
    {
        if (board == null || !board.IsGridReady()) return;

        // 1) Clear all cells
        for (int y = 0; y < board.rows; y++)
            for (int x = 0; x < board.columns; x++)
                board.ClearCellTint(x, y);

        // 2) Tint occupied cells
        foreach (var kvp in occ)
        {
            var cell = kvp.Key;
            var piece = kvp.Value;
            var tint  = piece.isPlayerTile ? playerOccupancyTint : enemyOccupancyTint;
            board.SetCellTint(cell.x, cell.y, tint);
        }
    }

    // ---------- End Game ----------
    /// <summary>Returns true if the game ended and UI was shown.</summary>
    private bool EvaluateEndGame()
    {
        if (gameOver) return true;

        int playerCount = 0;
        int enemyCount  = 0;

        foreach (var kvp in occ)
        {
            var p = kvp.Value;
            if (!p.enabled || !p.gameObject.activeInHierarchy) continue;
            if (p.isPlayerTile) playerCount++;
            else                enemyCount++;
        }

        if (playerCount == 0 && enemyCount == 0)
        {
            gameOver = true;
            ShowLose(false);
            ShowWin(false);
            return true;
        }
        else if (playerCount == 0)
        {
            gameOver = true;
            ShowLose(true);
            ShowWin(false);
            return true;
        }
        else if (enemyCount == 0)
        {
            gameOver = true;
            ShowWin(true);
            ShowLose(false);
            return true;
        }

        return false;
    }

    private void ShowWin(bool on)  { if (winUI  != null) winUI.SetActive(on); }
    private void ShowLose(bool on) { if (loseUI != null) loseUI.SetActive(on); }

    // =====================================================================
    // Ripple Juice
    // =====================================================================

    public void StartRipple(Vector2Int origin, Color color)
    {
        if (rippleCo != null) StopCoroutine(rippleCo);
        rippleCo = StartCoroutine(CoRipple(origin, color));
    }

    private IEnumerator CoRipple(Vector2Int origin, Color color)
    {
        if (board == null || !board.IsGridReady()) yield break;

        // Precompute Manhattan distances from origin for all cells
        int w = board.columns;
        int h = board.rows;

        int[,] dist = new int[w, h];
        int maxD = 0;
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            int d = Mathf.Abs(x - origin.x) + Mathf.Abs(y - origin.y);
            dist[x, y] = d;
            if (d > maxD) maxD = d;
        }

        // Animate over time: each cell starts at time = d * rippleCellDelay
        float totalDuration = (maxD * rippleCellDelay + rippleCellDuration) / Mathf.Max(0.0001f, rippleTimeScale);

        float t = 0f;
        while (t < totalDuration)
        {
            // Start by restoring base occupancy, then overlay the current frame of the ripple
            ApplyOccupancyTints();

            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float startTime = dist[x, y] * rippleCellDelay / Mathf.Max(0.0001f, rippleTimeScale);
                float localT = (t - startTime) / Mathf.Max(0.0001f, rippleCellDuration / rippleTimeScale);

                if (localT >= 0f && localT <= 1f)
                {
                    // Ease-out curve for intensity
                    float ease = 1f - Mathf.Pow(1f - localT, 3f);
                    float strength = rippleMaxStrength * (1f - ease); // strong at start, fades out

                    board.SetCellTintBlend(x, y, color, strength);
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Final restore
        ApplyOccupancyTints();
        rippleCo = null;
    }
}
