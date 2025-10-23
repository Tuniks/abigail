using UnityEngine;
using System;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "VoiceProfile", menuName = "Dialogue/Voice Profile", order = 0)]
public class VoiceProfile : ScriptableObject
{
    [Header("Blip Core")]
    [Tooltip("Ultra-short 'blip' clips. One will be chosen at random per character.")]
    public AudioClip[] blips;

    [Header("Levels & Pitch")]
    [Range(0f, 1f)] public float baseVolume = 0.6f;
    [Range(-24f, 24f)] public float basePitchSemitones = 0f;
    [Range(0f, 12f)] public float pitchJitterSemitones = 3f;

    [Tooltip("Extra pitch for UPPERCASE letters (in semitones).")]
    [Range(0f, 12f)] public float uppercasePitchBoost = 2f;
    [Tooltip("Extra volume for UPPERCASE letters (0-1 additive, clamped).")]
    [Range(0f, 1f)] public float uppercaseVolumeBoost = 0.1f;

    [Tooltip("Random volume variation per blip (0-1 = ±100%).")]
    [Range(0f, 1f)] public float volumeJitter = 0.08f;

    [Header("Cadence")]
    [Tooltip("Minimum time between letter blips (seconds). Prevents audio spam at high LPS).")]
    [Min(0f)] public float minInterval = 0.015f;

    [Serializable]
    public struct PunctuationPauses
    {
        [Min(0f)] public float period;     // .
        [Min(0f)] public float comma;      // ,
        [Min(0f)] public float question;   // ?
        [Min(0f)] public float exclam;     // !
        [Min(0f)] public float colon;      // :
        [Min(0f)] public float semicolon;  // ;
        [Min(0f)] public float ellipsis;   // … or "..."
    }

    [Header("Punctuation Pauses (seconds)")]
    public PunctuationPauses punctuationPauses = new PunctuationPauses
    {
        period = 0.045f,
        comma = 0.025f,
        question = 0.05f,
        exclam = 0.05f,
        colon = 0.03f,
        semicolon = 0.03f,
        ellipsis = 0.06f
    };

    [Header("Letter-Class Pitch Bias (semitones)")]
    [Tooltip("Applied when the revealed char is a vowel (a,e,i,o,u).")]
    public float vowelPitchBiasSemitones = 0.3f;
    [Tooltip("Applied when the revealed char is a consonant.")]
    public float consonantPitchBiasSemitones = -0.1f;

    [Header("Line Events")]
    [Tooltip("Played once when the line begins revealing.")]
    public AudioClip lineStartSFX;
    [Range(0f, 1f)] public float lineStartVolume = 0.7f;

    [Tooltip("Optional flourish when a line fully finishes revealing.")]
    public AudioClip lineEndFlourish;
    [Range(0f, 1f)] public float flourishVolume = 0.7f;

    public enum FastForwardMode { Off, Sparse }

    [Header("Fast-Forward")]
    [Tooltip("Sparse = when many letters appear at once (skip/advance), only play every Nth blip.")]
    public FastForwardMode fastForwardMode = FastForwardMode.Sparse;
    [Min(1)] public int sparseEveryNLetters = 4;

    // ---- Utilities ----
    public static float SemitonesToPitch(float st) => Mathf.Pow(2f, st / 12f);

    public static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        s = s.Trim()
             .Replace('\u00A0', ' ')
             .Replace('\u200B', ' ')
             .Replace('\u2009', ' ');
        s = Regex.Replace(s, "\\s+", " ");
        return s.ToLowerInvariant();
    }
}
