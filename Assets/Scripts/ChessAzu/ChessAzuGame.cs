using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-150)] // after ChessAzuManager (-200)
public class ChessAzuGame : MonoBehaviour
{
    public enum Side { Player, Enemy }

    [Header("Board")]
    public ChessAzuManager board;           // assign your existing ChessAzuManager

    [Header("Turn State")]
    public Side currentTurn = Side.Player;

    [Header("Capture Bins")]
    public AzuCaptureBin playerCaptureBin;  // right side (player trophies)
    public AzuCaptureBin enemyCaptureBin;   // left side  (enemy trophies)

    [Header("Enemy Automation")]
    public bool enemyAutoPlay = true;

    [Header("Juice / Audio")]
    public AudioClip captureSfx;
    public AudioSource sfxSource; // optional; if null, uses PlayClipAtPoint


    // Occupancy: grid cell -> piece
    private readonly Dictionary<Vector2Int, ChessAzuPiece> occ = new Dictionary<Vector2Int, ChessAzuPiece>();

    void Awake()
    {
        if (board == null) board = FindFirstObjectByType<ChessAzuManager>();
        RebuildOccupancy();
    }

    // -------- Occupancy --------
    public void RebuildOccupancy()
    {
        occ.Clear();
        var pieces = FindObjectsOfType<ChessAzuPiece>();
        foreach (var p in pieces)
        {
            if (!p.enabled || !p.gameObject.activeInHierarchy) continue; // captured pieces disabled
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
        return (currentTurn == Side.Player && p.isPlayerTile) ||
               (currentTurn == Side.Enemy && !p.isPlayerTile);
    }

    // -------- Legal moves --------
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

    // -------- Apply move/capture + turns --------
    public bool TryApplyMove(ChessAzuPiece piece, Vector2Int targetCell)
    {
        if (piece == null) return false;
        if (!IsPieceOnCurrentSide(piece)) return false;

        MoveOption? chosen = null;
        var options = GetLegalMoves(piece);
        for (int i = 0; i < options.Count; i++)
            if (options[i].cell == targetCell) { chosen = options[i]; break; }
        if (chosen == null) return false;

        // Capture first (if any)
        // Capture first (if any)
        if (chosen.Value.isCapture && chosen.Value.captured != null)
        {
            var cap = chosen.Value.captured;

            // prevent the captured piece from acting again / being counted in occupancy
            cap.enabled = false;

            // play the pop animation (in place), then send to the appropriate bin
            var juice = cap.GetComponent<AzuPieceJuice>();

            System.Action mid = () =>
            {
                // move to the correct bin at the "midpoint" of the pop (after overshoot)
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

            if (juice != null)
                juice.PlayCapturePop(captureSfx, sfxSource, onMidpoint: mid, onFinish: null);
            else
            {
                // no juice component? still do the bin move and sfx
                if (captureSfx != null)
                {
                    if (sfxSource != null) sfxSource.PlayOneShot(captureSfx);
                    else AudioSource.PlayClipAtPoint(captureSfx, cap.transform.position);
                }
                mid();
            }
        }


        // Move the mover
        piece.TryMoveTo(targetCell.x, targetCell.y);

        // Update board state and flip turn (with optional enemy auto)
        RebuildOccupancy();
        FlipTurn();
        return true;
    }

    public void FlipTurn()
    {
        currentTurn = (currentTurn == Side.Player) ? Side.Enemy : Side.Player;
        if (enemyAutoPlay && currentTurn == Side.Enemy)
            DoEnemyAutoMove();
    }

    // -------- Simple Enemy AI (prefer capture, else first move) --------
    private void DoEnemyAutoMove()
    {
        var all = FindObjectsOfType<ChessAzuPiece>();
        var cands = new List<(ChessAzuPiece piece, List<MoveOption> moves)>();

        foreach (var p in all)
        {
            if (!p.enabled || !p.gameObject.activeInHierarchy) continue;
            if (p.isPlayerTile) continue; // enemy only
            var moves = GetLegalMoves(p);
            if (moves.Count > 0) cands.Add((p, moves));
        }

        if (cands.Count == 0)
        {
            currentTurn = Side.Player; // no enemy moves; avoid soft lock
            return;
        }

        // Greedy capture
        foreach (var c in cands)
        {
            var cap = c.moves.Find(m => m.isCapture);
            if (cap.captured != null)
            {
                TryApplyMove(c.piece, cap.cell);
                return;
            }
        }

        // Otherwise first legal move
        TryApplyMove(cands[0].piece, cands[0].moves[0].cell);
    }
}
