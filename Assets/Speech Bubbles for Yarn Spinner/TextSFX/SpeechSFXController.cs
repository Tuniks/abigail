using System;
using UnityEngine;
using TMPro;

public class SpeechSFXController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The TMP text that Yarn is revealing (the bubble body text).")]
    [SerializeField] private TMP_Text dialogueTMP;
    [Tooltip("Optional: the TMP nameplate used by BasicBubbleContent (to resolve speaker).")]
    [SerializeField] private TMP_Text speakerNameTMP;

    [Header("Voices")]
    [Tooltip("Default voice if no speaker match is found (can be null).")]
    [SerializeField] private VoiceProfile defaultVoice;
    [Serializable] public class NameVoicePair { public string speakerName; public VoiceProfile voice; }
    [Tooltip("Map character display names to VoiceProfiles. Names should match your nameplate exactly.")]
    [SerializeField] private NameVoicePair[] speakerVoices = Array.Empty<NameVoicePair>();

    [Header("Audio Routing")]
    [SerializeField, Min(1)] private int audioSourcePoolSize = 6;
    [Tooltip("Optional mixer group (recommended).")]
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup mixerGroup;

    // Internal state
    private AudioSource[] pool;
    private int poolIndex = 0;
    private int lastVisible = 0;
    private double nextPlayableTime = 0;   // dsp time gate
    private bool flourishPlayedForThisLine = false;
    private bool lineStartPlayedForThisLine = false;

    // Cached text info to avoid allocations
    private TMP_TextInfo textInfo;

    void Awake()
    {
        // ---- Robust runtime lookup (includes inactive children) ----
        if (dialogueTMP == null) dialogueTMP = GetComponentInChildren<TMP_Text>(true);
        if (dialogueTMP == null)
        {
            Debug.LogError($"{nameof(SpeechSFXController)} couldn't find a Dialogue TMP in its children. Assign it explicitly in the Inspector.", this);
        }

        if (speakerNameTMP == null)
        {
            TMP_Text best = null;
            var tmps = GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in tmps)
            {
                if (t == dialogueTMP) continue;
                var n = t.gameObject.name.ToLowerInvariant();
                if (n.Contains("name") || n.Contains("speaker") || n.Contains("plate") || n.Contains("title"))
                {
                    best = t; break;
                }
            }
            if (best == null)
            {
                foreach (var t in tmps) { if (t != dialogueTMP) { best = t; break; } }
            }
            speakerNameTMP = best;
#if UNITY_EDITOR
            if (speakerNameTMP == null) {
                Debug.LogWarning($"{nameof(SpeechSFXController)}: No nameplate TMP found; per-speaker voices will only work if you assign one or set a defaultVoice.", this);
            }
#endif
        }

        // Build AudioSource pool
        pool = new AudioSource[audioSourcePoolSize];
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            src.outputAudioMixerGroup = mixerGroup;
            pool[i] = src;
        }

        ResetLineState();
    }

    void OnEnable()
    {
        ResetLineState();
    }

    void Update()
    {
        if (dialogueTMP == null) return;

        dialogueTMP.ForceMeshUpdate(false, false);
        textInfo = dialogueTMP.textInfo;

        int visible = Mathf.Min(dialogueTMP.maxVisibleCharacters, textInfo.characterCount);

        // Line start detected: first character becomes visible
        if (!lineStartPlayedForThisLine && visible > 0)
        {
            var v = GetCurrentVoice();
            PlayLineStartIfAny(v);
            lineStartPlayedForThisLine = true;
        }

        // Line end detected: all characters visible
        if (visible == textInfo.characterCount && textInfo.characterCount > 0 && !flourishPlayedForThisLine)
        {
            PlayFlourishIfAny(GetCurrentVoice());
            flourishPlayedForThisLine = true;
        }

        // New line auto-detect (visible resets)
        if (visible < lastVisible)
        {
            ResetLineState();
        }
        lastVisible = visible;
    }

    /// <summary>
    /// Hook this to BasicBubbleContent.OnTextAppeared (UnityEvent) via Inspector.
    /// </summary>
    public void OnCharacterTyped()
    {
        try
        {
            if (dialogueTMP == null) return;

            dialogueTMP.ForceMeshUpdate(false, false);
            textInfo = dialogueTMP.textInfo;

            int visible = Mathf.Min(dialogueTMP.maxVisibleCharacters, textInfo.characterCount);
            if (visible <= 0 || visible > textInfo.characterCount) return;

            // Index of the *newly revealed* character
            int charIndex = visible - 1;
            if (charIndex < 0 || charIndex >= textInfo.characterCount) return;

            var charInfo = textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) return;

            char c = charInfo.character;

            // Fast-forward / skip heuristic: >1 new chars revealed since last frame
            bool manyRevealedThisFrame = (visible - lastVisible) > 1;

            var voice = GetCurrentVoice();
            if (voice == null || voice.blips == null || voice.blips.Length == 0) return;

            // Fast-forward sparse mode: only play every Nth letter when many appear
            if (manyRevealedThisFrame &&
                voice.fastForwardMode == VoiceProfile.FastForwardMode.Sparse &&
                voice.sparseEveryNLetters > 1)
            {
                if ((visible % voice.sparseEveryNLetters) != 0)
                {
                    // Skip this blip
                    return;
                }
            }

            if (!IsSpeakable(c)) return;

            double now = AudioSettings.dspTime;
            double earliest = nextPlayableTime;

            // Punctuation handling: per-char extra pause
            float extraPause = GetPunctuationPauseSeconds(voice, c);

            // Cadence gate
            double interval = voice.minInterval + extraPause;
            if (now < earliest) return;

            // Choose clip
            var clip = voice.blips[UnityEngine.Random.Range(0, voice.blips.Length)];

            // Pitch compute: base + jitter + letter-class bias + uppercase
            float semis = voice.basePitchSemitones
                          + UnityEngine.Random.Range(-voice.pitchJitterSemitones, voice.pitchJitterSemitones);

            // Letter-class bias
            if (char.IsLetter(c))
            {
                if (IsVowel(c)) semis += voice.vowelPitchBiasSemitones;
                else semis += voice.consonantPitchBiasSemitones;
            }

            if (IsUppercase(c))
            {
                semis += voice.uppercasePitchBoost;
            }

            float pitch = VoiceProfile.SemitonesToPitch(semis);

            // Volume compute: base * (1 ± jitter) + uppercase boost
            float vol = voice.baseVolume;
            if (voice.volumeJitter > 0f)
            {
                float jitter = UnityEngine.Random.Range(-voice.volumeJitter, voice.volumeJitter);
                vol *= Mathf.Clamp01(1f + jitter);
            }
            if (IsUppercase(c))
            {
                vol = Mathf.Clamp01(vol + voice.uppercaseVolumeBoost);
            }

            // Play via pool
            var src = NextSource();
            src.clip = clip;
            src.volume = vol;
            src.pitch = pitch;

            double startTime = Math.Max(now, earliest);
            src.PlayScheduled(startTime);

            nextPlayableTime = startTime + clip.length + interval;
        }
        catch (Exception e)
        {
            // Don't kill the typewriter; log and continue.
            Debug.LogWarning($"SpeechSFXController.OnCharacterTyped error: {e.Message}", this);
        }
    }

    public void ResetLineState()
    {
        lastVisible = 0;
        flourishPlayedForThisLine = false;
        lineStartPlayedForThisLine = false;
        nextPlayableTime = AudioSettings.dspTime;
    }

    // -------- Helpers --------
    private AudioSource NextSource()
    {
        poolIndex = (poolIndex + 1) % pool.Length;
        return pool[poolIndex];
    }

    private VoiceProfile GetCurrentVoice()
    {
        if (speakerNameTMP == null || string.IsNullOrEmpty(speakerNameTMP.text)) return defaultVoice;
        string key = VoiceProfile.Normalize(speakerNameTMP.text);
        for (int i = 0; i < speakerVoices.Length; i++)
        {
            var entry = speakerVoices[i];
            if (entry == null || entry.voice == null) continue;
            if (VoiceProfile.Normalize(entry.speakerName) == key)
            {
                return entry.voice;
            }
        }
        return defaultVoice;
    }

    private static bool IsSpeakable(char c)
    {
        if (char.IsWhiteSpace(c)) return false;
        if (char.IsControl(c)) return false;
        return c != '\u200B' && c != '\u200C' && c != '\u200D';
    }

    private static bool IsUppercase(char c) => char.IsLetter(c) && char.IsUpper(c);

    private static bool IsVowel(char c)
    {
        char l = char.ToLowerInvariant(c);
        return l == 'a' || l == 'e' || l == 'i' || l == 'o' || l == 'u';
    }

    private static bool IsEllipsis(char c) => c == '…';

    private static float GetPunctuationPauseSeconds(VoiceProfile v, char c)
    {
        switch (c)
        {
            case '.': return v.punctuationPauses.period;
            case ',': return v.punctuationPauses.comma;
            case '?': return v.punctuationPauses.question;
            case '!': return v.punctuationPauses.exclam;
            case ':': return v.punctuationPauses.colon;
            case ';': return v.punctuationPauses.semicolon;
            default:
                if (IsEllipsis(c)) return v.punctuationPauses.ellipsis;
                // crude support for "..." typed as three chars: give the last '.' the ellipsis pause
                if (c == '.' && v.punctuationPauses.ellipsis > 0f)
                {
                    return v.punctuationPauses.period;
                }
                return 0f;
        }
    }

    private void PlayFlourishIfAny(VoiceProfile voice)
    {
        if (voice == null || voice.lineEndFlourish == null) return;
        var src = NextSource();
        src.clip = voice.lineEndFlourish;
        src.volume = voice.flourishVolume;
        src.pitch = 1f;
        src.Play();
    }

    private void PlayLineStartIfAny(VoiceProfile voice)
    {
        if (voice == null || voice.lineStartSFX == null) return;
        var src = NextSource();
        src.clip = voice.lineStartSFX;
        src.volume = voice.lineStartVolume;
        src.pitch = 1f;
        src.Play();
    }
}
