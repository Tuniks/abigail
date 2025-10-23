using UnityEngine;

[DefaultExecutionOrder(-200)] // Ensure manager builds before pieces
public class ChessAzuManager : MonoBehaviour
{
    [Header("Grid Settings (2D)")]
    [Min(1)] public int columns = 5;
    [Min(1)] public int rows = 5;

    [Tooltip("World size of each cell on X/Y.")]
    [Min(0.01f)] public float cellSize = 1f;

    [Tooltip("Extra spacing (world units) between cells (X,Y).")]
    public Vector2 spacing = new Vector2(0.05f, 0.05f);

    [Tooltip("Optional override for where the grid starts. If null, uses this GameObject.")]
    public Transform gridOrigin;

    [Header("Prefabs")]
    [Tooltip("A sprite-based cell prefab (SpriteRenderer). Pivot centered.")]
    public GameObject cellPrefab;

    [Header("2D Rendering")]
    public string sortingLayerName = "Default";
    public int sortingOrderBase = 0;

    [Header("Behavior")]
    [Tooltip("Build the grid automatically in Awake().")]
    public bool buildOnAwake = true;

    // Runtime
    private Transform gridParent;
    private GameObject[,] cells;

    // Convenience
    public float StepX => cellSize + spacing.x;
    public float StepY => cellSize + spacing.y;
    public Vector3 Origin => (gridOrigin ? gridOrigin : transform).position;

    public bool IsGridReady() => cells != null;

    /// <summary>
    /// Returns the cell GameObject at (x,y) if it exists, or null.
    /// </summary>
    public GameObject GetCell(int x, int y)
    {
        if (cells == null) return null;
        if (x < 0 || x >= columns || y < 0 || y >= rows) return null;
        return cells[x, y];
    }


    public Vector3 GetCellWorldPosition(int x, int y)
    {
        x = Mathf.Clamp(x, 0, columns - 1);
        y = Mathf.Clamp(y, 0, rows - 1);
        return new Vector3(Origin.x + x * StepX, Origin.y + y * StepY, 0f);
    }

    public bool TryWorldToCell(Vector3 worldPos, out int x, out int y)
    {
        Vector2 local = (Vector2)(worldPos - Origin);
        x = Mathf.FloorToInt((local.x + StepX * 0.5f) / StepX);
        y = Mathf.FloorToInt((local.y + StepY * 0.5f) / StepY);
        bool inside = x >= 0 && x < columns && y >= 0 && y < rows;
        if (!inside) { x = -1; y = -1; }
        return inside;
    }

    private void Awake()
    {
        if (buildOnAwake)
            RebuildGrid();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);
        cellSize = Mathf.Max(0.01f, cellSize);
        spacing.x = Mathf.Max(0f, spacing.x);
        spacing.y = Mathf.Max(0f, spacing.y);
    }
#endif

    [ContextMenu("Rebuild Grid")]
    public void RebuildGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogWarning("[ChessAzuManager] No cellPrefab assigned.");
            return;
        }

        ClearGrid();

        gridParent = new GameObject($"ChessAzu_Grid_{columns}x{rows}").transform;
        gridParent.SetParent(transform, worldPositionStays: false);

        cells = new GameObject[columns, rows];

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
            {
                Vector3 pos = GetCellWorldPosition(x, y);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, gridParent);

                // Scale to fit cellSize on X/Y (keep Z)
                Vector3 s = cell.transform.localScale;
                cell.transform.localScale = new Vector3(cellSize, cellSize, s.z);

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
    }

    private void OnDrawGizmosSelected()
    {
        float szX = StepX, szY = StepY;
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.35f);
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
            {
                Vector3 c = GetCellWorldPosition(x, y);
                Gizmos.DrawWireCube(c, new Vector3(cellSize, cellSize, 0.01f));
            }
    }
}
