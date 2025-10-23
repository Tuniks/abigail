using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AzuMovementDatabase", menuName = "Azulejo/Azu Movement Database", order = 1)]
public class AzuMovementDatabase : ScriptableObject
{
    [Tooltip("Profiles available in this ruleset.")]
    public List<AzuMovementProfile> profiles = new List<AzuMovementProfile>();

    /// <summary>
    /// Case-insensitive lookup. Trims whitespace and strips trailing \"(Clone)\".
    /// </summary>
    public AzuMovementProfile FindByPieceName(string goName)
    {
        if (string.IsNullOrWhiteSpace(goName) || profiles == null) return null;

        string key = NormalizeName(goName);
        foreach (var p in profiles)
        {
            if (p == null) continue;
            if (NormalizeName(p.pieceNameKey) == key)
                return p;
        }
        return null;
    }

    public static string NormalizeName(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        s = s.Trim();
        // Remove Unity's (Clone)
        if (s.EndsWith("(Clone)")) s = s.Substring(0, s.Length - "(Clone)".Length).TrimEnd();
        return s.ToLowerInvariant();
    }
}
