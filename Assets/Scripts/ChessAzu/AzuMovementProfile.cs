using UnityEngine;

[CreateAssetMenu(fileName = "NewAzuMovementProfile", menuName = "Azulejo/Azu Movement Profile", order = 0)]
public class AzuMovementProfile : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Name key this profile matches against (case-insensitive). " +
             "Your piece GameObject name should equal this (ignoring (Clone)).")]
    public string pieceNameKey = "Knight";

    [Header("Movement Mask (5x5, center is P and ignored)")]
    [SerializeField] private bool[] movementMask = new bool[25];

    public const int SIZE = 5;
    public const int CENTER_INDEX = 12; // 5*2 + 2

    public bool[] GetMask()
    {
        if (movementMask == null || movementMask.Length != 25)
            movementMask = new bool[25];
        return movementMask;
    }
}
