using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ChessAzuPiece : MonoBehaviour
{
    public enum StartSnapMode
    {
        None,
        FromInspector,     // use gridX/gridY exactly
        FromWorldNearest   // find nearest cell to current transform.position
    }

    [Header("Ownership")]
    [Tooltip("If true, this piece is selectable by the player.")]
    public bool isPlayerTile = false;

    [Header("References")]
    public ChessAzuManager manager;

    [Tooltip("Database used to resolve movement profile by this GameObject's name.")]
    public AzuMovementDatabase movementDatabase;

    [Tooltip("Optional: override the profile directly (takes priority over name lookup).")]
    public AzuMovementProfile explicitProfileOverride;

    [Header("Grid Position")]
    [SerializeField] private int gridX;
    [SerializeField] private int gridY;

    [Header("Start Placement")]
    [Tooltip("How to place this piece when the scene starts.")]
    public StartSnapMode startSnap = StartSnapMode.FromInspector;

    // Runtime cache
    private AzuMovementProfile activeProfile;
    private bool[] mask; // 25-length view into activeProfile

    const int RADIUS = 2;

    void Awake()
    {
        if (manager == null)
            manager = FindFirstObjectByType<ChessAzuManager>();

        ResolveProfile();
    }

    void Start()
    {
        if (manager == null || !manager.IsGridReady()) return;

        switch (startSnap)
        {
            case StartSnapMode.None:
                // Do nothing — leave transform wherever it is
                break;

            case StartSnapMode.FromInspector:
                // Honor the inspector indices exactly
                ClampIndicesToBoard();
                EnsurePiecesParent();
                transform.position = manager.GetPieceWorldPosition(gridX, gridY);
                break;

            case StartSnapMode.FromWorldNearest:
                SnapToNearestCell();
                break;
        }
    }

    // ---------- Profile resolution ----------
    public void ResolveProfile()
    {
        if (explicitProfileOverride != null)
            activeProfile = explicitProfileOverride;
        else if (movementDatabase != null)
            activeProfile = movementDatabase.FindByPieceName(gameObject.name);
        else
            activeProfile = null;

        mask = activeProfile != null ? activeProfile.GetMask() : null;

        if (activeProfile == null)
        {
            Debug.LogWarning($"[ChessAzuPiece] No movement profile found for '{gameObject.name}'. " +
                             $"Assign an explicit profile or set a database with a matching name key.");
        }
    }

    // ---------- Placement helpers ----------
    private void EnsurePiecesParent()
    {
        if (manager == null) return;
        var root = manager.GetPiecesRoot();
        if (root != null && transform.parent != root)
            transform.SetParent(root, worldPositionStays: true);
    }

    public void SnapToNearestCell()
    {
        if (manager == null) return;

        if (manager.TryWorldToCell(transform.position, out int cx, out int cy))
        {
            SetGridPosition(cx, cy, teleport: true);
        }
        else
        {
            // Outside board: clamp to nearest edge
            int nx = Mathf.Clamp(Mathf.RoundToInt((transform.position.x - manager.Origin.x) / manager.StepX), 0, manager.columns - 1);
            int ny = Mathf.Clamp(Mathf.RoundToInt((transform.position.y - manager.Origin.y) / manager.StepY), 0, manager.rows - 1);
            SetGridPosition(nx, ny, teleport: true);
        }
    }

    public void SetGridPosition(int x, int y, bool teleport = true)
    {
        if (manager == null) return;
        gridX = Mathf.Clamp(x, 0, manager.columns - 1);
        gridY = Mathf.Clamp(y, 0, manager.rows - 1);

        if (teleport)
        {
            EnsurePiecesParent();
            transform.position = manager.GetPieceWorldPosition(gridX, gridY);
        }
    }

    public Vector2Int GetGridPosition() => new Vector2Int(gridX, gridY);

    private void ClampIndicesToBoard()
    {
        if (manager == null) return;
        gridX = Mathf.Clamp(gridX, 0, manager.columns - 1);
        gridY = Mathf.Clamp(gridY, 0, manager.rows - 1);
    }

    // ---------- Movement ----------
    public bool TryMoveTo(int targetX, int targetY)
    {
        if (manager == null || !manager.IsGridReady() || mask == null) return false;
        if (!IsInsideBoard(targetX, targetY)) return false;
        if (!IsMoveAllowed(targetX, targetY)) return false;

        gridX = targetX;
        gridY = targetY;

        EnsurePiecesParent();
        transform.position = manager.GetPieceWorldPosition(gridX, gridY);
        return true;
    }

    public bool IsMoveAllowed(int targetX, int targetY)
    {
        if (mask == null) return false;

        int dx = targetX - gridX;
        int dy = targetY - gridY;
        if (dx == 0 && dy == 0) return false;
        if (Mathf.Abs(dx) > RADIUS || Mathf.Abs(dy) > RADIUS) return false;

        int idx = OffsetToIndex(dx, dy);
        if (idx == AzuMovementProfile.CENTER_INDEX) return false;
        return mask[idx];
    }

    public IEnumerable<Vector2Int> GetAllowedMoves()
    {
        if (manager == null || !manager.IsGridReady() || mask == null) yield break;

        for (int dy = -RADIUS; dy <= RADIUS; dy++)
        for (int dx = -RADIUS; dx <= RADIUS; dx++)
        {
            if (dx == 0 && dy == 0) continue;
            int idx = OffsetToIndex(dx, dy);
            if (idx == AzuMovementProfile.CENTER_INDEX || !mask[idx]) continue;

            int tx = gridX + dx;
            int ty = gridY + dy;
            if (IsInsideBoard(tx, ty))
                yield return new Vector2Int(tx, ty);
        }
    }

    // ---------- Helpers ----------
    private bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && y >= 0 && x < manager.columns && y < manager.rows;
    }

    /// Row-major mapping: (dx=-2,dy=+2) → 0 … (dx=+2,dy=-2) → 24
    private int OffsetToIndex(int dx, int dy)
    {
        int col = dx + RADIUS;     // 0..4
        int row = (RADIUS - dy);   // dy=+2 → row=0 (top)
        return row * 5 + col;      // 0..24
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (manager == null) return;

        // Visualize intended (gridX,gridY) cell when StartSnap=FromInspector
        if (startSnap == StartSnapMode.FromInspector && manager.IsGridReady())
        {
            var c = manager.GetCellWorldPosition(
                Mathf.Clamp(gridX, 0, Mathf.Max(0, manager.columns - 1)),
                Mathf.Clamp(gridY, 0, Mathf.Max(0, manager.rows - 1))
            );
            Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.4f);
            Gizmos.DrawWireCube(c, new Vector3(manager.cellSize, manager.cellSize, 0.01f));
        }

        // Draw current legal moves if we have a profile
        if (manager.IsGridReady() && mask != null)
        {
            Gizmos.color = new Color(0f, 0.8f, 1f, 0.35f);
            foreach (var cell in GetAllowedMoves())
            {
                Vector3 p = manager.GetCellWorldPosition(cell.x, cell.y);
                Gizmos.DrawCube(p, new Vector3(manager.cellSize, manager.cellSize, 0.02f));
            }
        }
    }
#endif
}
