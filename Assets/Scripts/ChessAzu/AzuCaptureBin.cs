using System.Collections.Generic;
using UnityEngine;

public class AzuCaptureBin : MonoBehaviour
{
    [Header("Placement")]
    [Tooltip("Where captured pieces will be parented. Defaults to this transform if null.")]
    public Transform anchor;
    [Tooltip("Vertical distance between stacked pieces (world units).")]
    public float verticalSpacing = 1.0f;
    [Tooltip("Optional X offset step per capture (for a subtle stagger).")]
    public float horizontalJitter = 0.0f;

    [Header("Behavior")]
    [Tooltip("Disable interaction scripts on captured pieces.")]
    public bool disablePieceScripts = true;

    private readonly List<ChessAzuPiece> captured = new List<ChessAzuPiece>();

    void Awake()
    {
        if (anchor == null) anchor = transform;
    }

    public void AddCaptured(ChessAzuPiece piece)
    {
        if (piece == null) return;

        // Optional: disable interaction
        if (disablePieceScripts) piece.enabled = false;

        // Parent and position in stack
        piece.transform.SetParent(anchor, worldPositionStays: false);

        int i = captured.Count;
        Vector3 pos = new Vector3(i * horizontalJitter, -i * verticalSpacing, 0f);
        piece.transform.localPosition = pos;
        piece.transform.localRotation = Quaternion.identity;
        piece.transform.localScale = Vector3.one;

        captured.Add(piece);
    }

    public int Count => captured.Count;
    public IReadOnlyList<ChessAzuPiece> Captured => captured;
}
