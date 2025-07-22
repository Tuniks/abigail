using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioManager2D : MonoBehaviour
{
    [Header("Who is moving through the triggers?")]
    [SerializeField] private GameObject watchedObject;

    [System.Serializable]
    public class TriggerAudioPair
    {
        public Collider2D trigger;              // 2D trigger volume
        public AudioSource audioSource;         // AudioSource to control
        [Range(0f, 1f)] public float maxVolume = 1f;   // <- cap for THIS clip
        public float fadeInDuration  = 1f;
        public float fadeOutDuration = 1f;
    }

    [Header("Trigger ↔ Audio mappings")]
    [SerializeField] private List<TriggerAudioPair> pairs = new();

    // internals
    private readonly Dictionary<AudioSource, Coroutine> _fadeCo = new();
    private readonly HashSet<Collider2D> _inside = new();

    private void Reset() => watchedObject = gameObject;

    private void Awake()
    {
        if (!watchedObject) watchedObject = gameObject;

        var rb2d = watchedObject.GetComponent<Rigidbody2D>();
        if (!rb2d)
        {
            rb2d = watchedObject.AddComponent<Rigidbody2D>();
            rb2d.isKinematic = true;
            rb2d.gravityScale = 0f;
        }

        var col2d = watchedObject.GetComponent<Collider2D>();
        if (!col2d)
        {
            Debug.LogWarning("[TriggerAudioManager2D] WatchedObject needs a Collider2D.");
        }

        foreach (var p in pairs)
        {
            if (p.audioSource)
            {
                p.audioSource.volume = 0f;
                p.audioSource.loop   = true;
            }
        }
    }

    private void OnEnable()
    {
        var relay = watchedObject.GetComponent<TriggerRelay2D>();
        if (!relay) relay = watchedObject.AddComponent<TriggerRelay2D>();
        relay.Entered += HandleEnter;
        relay.Exited  += HandleExit;
    }

    private void OnDisable()
    {
        var relay = watchedObject.GetComponent<TriggerRelay2D>();
        if (relay)
        {
            relay.Entered -= HandleEnter;
            relay.Exited  -= HandleExit;
        }
    }

    private void HandleEnter(Collider2D trigger)
    {
        var pair = pairs.Find(p => p.trigger == trigger);
        if (pair == null) return;

        _inside.Add(trigger);
        Fade(pair.audioSource, pair.maxVolume, pair.fadeInDuration);
    }

    private void HandleExit(Collider2D trigger)
    {
        var pair = pairs.Find(p => p.trigger == trigger);
        if (pair == null) return;

        _inside.Remove(trigger);
        Fade(pair.audioSource, 0f, pair.fadeOutDuration);
    }

    private void Fade(AudioSource src, float target, float duration)
    {
        if (!src) return;

        if (_fadeCo.TryGetValue(src, out var running) && running != null)
            StopCoroutine(running);

        if (target > 0f && !src.isPlaying) src.Play();

        _fadeCo[src] = StartCoroutine(FadeRoutine(src, target, duration));
    }

    private IEnumerator FadeRoutine(AudioSource src, float target, float time)
    {
        float start = src.volume;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }

        src.volume = target;

        if (Mathf.Approximately(target, 0f))
            src.Stop();

        _fadeCo[src] = null;
    }

    // -------- Relay -----------
    private class TriggerRelay2D : MonoBehaviour
    {
        public event System.Action<Collider2D> Entered;
        public event System.Action<Collider2D> Exited;

        private void OnTriggerEnter2D(Collider2D other) => Entered?.Invoke(other);
        private void OnTriggerExit2D(Collider2D other)  => Exited?.Invoke(other);
    }
}
