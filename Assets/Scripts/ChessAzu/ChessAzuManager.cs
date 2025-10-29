using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)] // Build board before pieces snap
public class ChessAzuManager : MonoBehaviour
{
    // ---------- Grid Settings ----------
    [Header("Grid Settings (2D layout, 3D cells)")]
    [Min(1)] public int columns = 5;
    [Min(1)] public int rows = 5;

    [Tooltip("Footprint of a single cell in world units (X/Y).")]
    [Min(0.01f)] public float cellSize = 1f;

    [Tooltip("Extra spacing added between cells (X,Y).")]
    public Vector2 spacing = new Vector2(0.05f, 0.05f);

    [Tooltip("Optional origin transform; if null, uses this object's position.")]
    public Transform gridOrigin;

    // ---------- Prefabs / Rendering ----------
    [Header("Prefabs")]
    [Tooltip("Assign a 3D Cube (or any Renderer) prefab used for each cell.")]
    public GameObject cellPrefab;

    [Header("Sprite Sorting (if your cell prefab also has a SpriteRenderer)")]
    public string sortingLayerName = "Default";
    public int sortingOrderBase = 0;

    [Header("Z Offsets (Camera at z = -10 looking +Z)")]
    [Tooltip("World Z for the cube cells (farther from camera = larger Z).")]
    public float cellZ = 0f;

    [Tooltip("Default world Z for the PIECES root (more negative than cellZ so sprites render on top).")]
    public float pieceZ = -2f;

    [Header("Piece Placement")]
    [Tooltip("If true, place pieces on the physical top face of the cube (adds vertical lift).")]
    public bool placePiecesOnTopOfCubes = true;

    [Tooltip("Extra vertical lift above the cube top (world units).")]
    public float pieceYOffset = 0.01f;

    [Tooltip("Additional Z nudge for pieces relative to cellZ (negative = closer to camera in 2D Ortho).")]
    public float pieceZOffset = -0.05f;

    [Header("Behavior")]
    [Tooltip("Build the grid automatically on Awake().")]
    public bool buildOnAwake = true;

    // ---------- Runtime ----------
    private Transform gridParent;
    private Transform piecesRoot; // parent for all pieces (keeps Z consistent & in front)
    private GameObject[,] cells;

    // Tint system (material-safe via MaterialPropertyBlock)
    private readonly Dictionary<Renderer, Color> _originalColor = new();
    private readonly Dictionary<Renderer, MaterialPropertyBlock> _blocks = new();

    // Common shader color property IDs
    private static readonly int _BaseColorId = Shader.PropertyToID("_BaseColor"); // URP/HDRP
    private static readonly int _ColorId     = Shader.PropertyToID("_Color");     // Built-in

    // ---------- Convenience ----------
    public float StepX => cellSize + spacing.x;
    public float StepY => cellSize + spacing.y;
    public Vector3 Origin => (gridOrigin ? gridOrigin.position : transform.position);

    public bool IsGridReady() => cells != null;

    // =====================================================================
    // Public API
    // =====================================================================

    /// <summary>Returns the cell GameObject at (x,y) or null if out of range/not built.</summary>
    public GameObject GetCell(int x, int y)
    {
        if (cells == null) return null;
        if (x < 0 || x >= columns || y < 0 || y >= rows) return null;
        return cells[x, y];
    }

    /// <summary>World position for a cell center (use for cubes).</summary>
    public Vector3 GetCellWorldPosition(int x, int y)
    {
        x = Mathf.Clamp(x, 0, columns - 1);
        y = Mathf.Clamp(y, 0, rows - 1);
        return new Vector3(Origin.x + x * StepX, Origin.y + y * StepY, cellZ);
    }

    /// <summary>World position where a PIECE should sit (above cubes, with optional top-face lift).</summary>
    public Vector3 GetPieceWorldPosition(int x, int y)
    {
        // Start from the cube's center position
        Vector3 basePos = GetCellWorldPosition(x, y);

        // 1) Lift to the cube's top if requested (Unity cube height is along Y; we scale Y to cellSize)
        if (placePiecesOnTopOfCubes)
        {
            float cubeHalfHeight = cellSize * 0.5f;
            basePos.y += cubeHalfHeight + pieceYOffset;
        }

        // 2) Nudge toward camera on Z so sprites are guaranteed to render above the cube
        basePos.z = cellZ + pieceZOffset;

        // If we have a PiecesRoot, ensure we use its Z (keeps everything consistent)
        if (piecesRoot != null)
            basePos.z = piecesRoot.position.z;

        return basePos;
    }

    /// <summary>Attempts to convert a world position to a cell index on the 2D board plane.</summary>
    public bool TryWorldToCell(Vector3 worldPos, out int x, out int y)
    {
        // Project onto board plane using X/Y only
        Vector2 local = (Vector2)(worldPos - new Vector3(Origin.x, Origin.y, 0f));
        x = Mathf.FloorToInt((local.x + StepX * 0.5f) / StepX);
        y = Mathf.FloorToInt((local.y + StepY * 0.5f) / StepY);
        bool inside = x >= 0 && x < columns && y >= 0 && y < rows;
        if (!inside) { x = -1; y = -1; }
        return inside;
    }

    /// <summary>Parent transform where all PIECES should live (keeps consistent Z).</summary>
    public Transform GetPiecesRoot() => piecesRoot;

    // =====================================================================
    // Lifecycle
    // =====================================================================

    private void Awake()
    {
        if (buildOnAwake)
            RebuildGrid();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        columns  = Mathf.Max(1, columns);
        rows     = Mathf.Max(1, rows);
        cellSize = Mathf.Max(0.01f, cellSize);
        spacing.x = Mathf.Max(0f, spacing.x);
        spacing.y = Mathf.Max(0f, spacing.y);
    }
#endif

    // =====================================================================
    // Grid Build / Clear
    // =====================================================================

    [ContextMenu("Rebuild Grid")]
    public void RebuildGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogWarning("[ChessAzuManager] No cellPrefab assigned.");
            return;
        }

        ClearGrid();

        // Parent for cubes
        gridParent = new GameObject($"ChessAzu_Grid_{columns}x{rows}").transform;
        gridParent.SetParent(transform, worldPositionStays: false);
        gridParent.position = new Vector3(Origin.x, Origin.y, 0f);

        // Parent for pieces (keeps all pieces at consistent Z in front of cubes)
        if (piecesRoot == null)
        {
            piecesRoot = new GameObject("ChessAzu_PiecesRoot").transform;
            piecesRoot.SetParent(transform, worldPositionStays: false);
        }
        // PiecesRoot uses pieceZ by default, else you can move it in-scene for layering
        piecesRoot.position = new Vector3(Origin.x, Origin.y, pieceZ);

        cells = new GameObject[columns, rows];

        for (int y = 0; y < rows; y++)
        for (int x = 0; x < columns; x++)
        {
            Vector3 pos = GetCellWorldPosition(x, y);
            GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, gridParent);

            // Fit cube footprint to cellSize on X/Y (keep local Z scale as-is)
            var t = cell.transform;
            var s = t.localScale;
            t.localScale = new Vector3(cellSize, cellSize, s.z);

            // If your prefab also includes a SpriteRenderer, keep sorting consistent (optional)
            var sr = cell.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
                sr.sortingOrder = sortingOrderBase;
            }

            cell.name = $"Cell_{x}_{y}";
            cells[x, y] = cell;
        }
    }

    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        if (gridParent != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(gridParent.gameObject);
            else Destroy(gridParent.gameObject);
#else
            Destroy(gridParent.gameObject);
#endif
        }
        gridParent = null;
        cells = null;

        // Only destroy PiecesRoot if empty (you likely keep pieces around)
        if (piecesRoot != null && piecesRoot.childCount == 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(piecesRoot.gameObject);
            else Destroy(piecesRoot.gameObject);
#else
            Destroy(piecesRoot.gameObject);
#endif
            piecesRoot = null;
        }

        _originalColor.Clear();
        _blocks.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.35f);
        for (int y = 0; y < rows; y++)
        for (int x = 0; x < columns; x++)
        {
            Vector3 c = GetCellWorldPosition(x, y);
            Gizmos.DrawWireCube(c, new Vector3(cellSize, cellSize, 0.01f));
        }
    }

    // =====================================================================
    // Cube Tinting (for highlights/hover/occupancy/ripples)
    // =====================================================================

    /// <summary>Applies a tint to the cube cell's material using a MaterialPropertyBlock.</summary>
    public void SetCellTint(int x, int y, Color color)
    {
        var go = GetCell(x, y);
        if (go == null) return;

        var rend = go.GetComponentInChildren<Renderer>();
        if (rend == null) return;

        if (!_blocks.TryGetValue(rend, out var block))
        {
            block = new MaterialPropertyBlock();
            _blocks[rend] = block;
        }

        // Cache original color once
        if (!_originalColor.ContainsKey(rend))
        {
            Color baseCol = Color.white;
            var mat = rend.sharedMaterial;
            if (mat != null)
            {
                if (mat.HasProperty(_BaseColorId)) baseCol = mat.GetColor(_BaseColorId);
                else if (mat.HasProperty(_ColorId)) baseCol = mat.GetColor(_ColorId);
            }
            _originalColor[rend] = baseCol;
        }

        // Apply into block (prefer _BaseColor if the shader supports it)
        var shared = rend.sharedMaterial;
        if (shared != null && shared.HasProperty(_BaseColorId))
            block.SetColor(_BaseColorId, color);
        else
            block.SetColor(_ColorId, color);

        rend.SetPropertyBlock(block);
    }

    /// <summary>Restores the cell’s original material color (clears the tint).</summary>
    public void ClearCellTint(int x, int y)
    {
        var go = GetCell(x, y);
        if (go == null) return;

        var rend = go.GetComponentInChildren<Renderer>();
        if (rend == null) return;

        if (!_blocks.TryGetValue(rend, out var block))
        {
            block = new MaterialPropertyBlock();
            _blocks[rend] = block;
        }

        if (_originalColor.TryGetValue(rend, out var original))
        {
            var shared = rend.sharedMaterial;
            if (shared != null && shared.HasProperty(_BaseColorId))
                block.SetColor(_BaseColorId, original);
            else
                block.SetColor(_ColorId, original);

            rend.SetPropertyBlock(block);
        }
        else
        {
            // No record; remove any block
            rend.SetPropertyBlock(null);
        }
    }

    /// <summary>Returns the base (untinted) material color of a cell, safely.</summary>
    public Color GetCellBaseColor(int x, int y)
    {
        var go = GetCell(x, y);
        if (go == null) return Color.white;

        var rend = go.GetComponentInChildren<Renderer>();
        if (rend == null || rend.sharedMaterial == null) return Color.white;

        var mat = rend.sharedMaterial;
        if (mat.HasProperty(_BaseColorId)) return mat.GetColor(_BaseColorId);
        if (mat.HasProperty(_ColorId))     return mat.GetColor(_ColorId);
        return Color.white;
    }

    /// <summary>Blend-tint a cell: baseColor → targetColor by 'strength' (0..1).</summary>
    public void SetCellTintBlend(int x, int y, Color targetColor, float strength)
    {
        strength = Mathf.Clamp01(strength);
        var baseCol = GetCellBaseColor(x, y);
        var blended = Color.Lerp(baseCol, targetColor, strength);
        SetCellTint(x, y, blended);
    }
}
