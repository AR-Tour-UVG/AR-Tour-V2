using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class AudioDirector : MonoBehaviour
{
    public static AudioDirector Instance { get; private set; }

    [Header("Fades")]
    [Range(0f, 5f)]
    [Tooltip("Default fade-in duration in seconds")]
    [SerializeField]
    float defaultFadeIn = 0.0f;

    [Range(0f, 5f)]
    [Tooltip("Default fade-out duration in seconds")]
    [SerializeField]
    float defaultFadeOut = 0.25f;

    AudioSource src;
    Coroutine routine;
    uint token;

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EnsureAudioInfrastructure();
        ApplyVolumeFromPrefs();

        Debug.Log("[AudioDirector] Ready");
    }

    // -------- infra helpers --------
    void EnsureAudioInfrastructure()
    {
        // 1) Listener: ensure at least one enabled in scene
        var listener = FindFirstObjectByType<AudioListener>();
        if (!listener)
        {
            gameObject.AddComponent<AudioListener>();
            Debug.Log("[AudioDirector] Added AudioListener to Audio GO");
        }
        else if (!listener.enabled)
        {
            listener.enabled = true;
        }

        // 2) Source: find or create on this GO
        src = GetComponent<AudioSource>();
        if (!src)
        {
            src = gameObject.AddComponent<AudioSource>();
            Debug.Log("[AudioDirector] Added AudioSource to Audio GO");
        }

        ConfigureNarrationSource(src);
    }

    static void ConfigureNarrationSource(AudioSource s)
    {
        s.playOnAwake = false;
        s.loop = false;
        s.spatialBlend = 0f; // 2D
        s.dopplerLevel = 0f; // no pitch warble
        s.rolloffMode = AudioRolloffMode.Linear;
        s.minDistance = 1f;
        s.maxDistance = 10f;
        s.volume = 1f; // overall loudness comes from AudioListener.volume
        s.bypassListenerEffects = false;
        s.bypassEffects = false;
        s.bypassReverbZones = true;
    }

    public static void ApplyVolumeFromPrefs()
    {
        AudioListener.volume = AppPrefs.LoadVolume() / 100f;
    }

    // -------- Public API (unchanged) --------
    public void Play(AudioClip clip, float fadeIn = -1f, float fadeOutPrev = -1f)
    {
        if (!clip)
            return;
        fadeIn = fadeIn < 0 ? defaultFadeIn : fadeIn;
        fadeOutPrev = fadeOutPrev < 0 ? defaultFadeOut : fadeOutPrev;
        token++;
        StartOrSwap(new[] { clip }, fadeIn, fadeOutPrev);
    }

    public void PlaySequence(
        IReadOnlyList<AudioClip> clips,
        float gapSeconds = 0.05f,
        float fadeIn = -1f,
        float fadeOutPrev = -1f
    )
    {
        if (clips == null || clips.Count == 0)
            return;
        fadeIn = fadeIn < 0 ? defaultFadeIn : fadeIn;
        fadeOutPrev = fadeOutPrev < 0 ? defaultFadeOut : fadeOutPrev;
        token++;
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(CoSequence(clips, gapSeconds, fadeIn, fadeOutPrev, token));
    }

    public void Stop(float fadeOut = -1f)
    {
        fadeOut = fadeOut < 0 ? defaultFadeOut : fadeOut;
        token++;
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(CoFadeOut(src, fadeOut));
    }

    // -------- Internals (your existing code) --------
    void StartOrSwap(IReadOnlyList<AudioClip> clips, float fadeIn, float fadeOutPrev)
    {
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(CoSwapTo(clips, fadeIn, fadeOutPrev, token));
    }

    IEnumerator CoSwapTo(IReadOnlyList<AudioClip> clips, float fadeIn, float fadeOutPrev, uint tk)
    {
        yield return CoFadeOut(src, fadeOutPrev);
        if (tk != token)
            yield break;

        for (int i = 0; i < clips.Count; i++)
        {
            var clip = clips[i];
            if (!clip)
                continue;
            src.clip = clip;
            src.volume = 0f;
            src.Play();
            yield return CoFadeTo(src, 1f, fadeIn);
            while (src.isPlaying && tk == token)
                yield return null;
            if (tk != token)
                yield break;
        }
    }

    IEnumerator CoSequence(
        IReadOnlyList<AudioClip> clips,
        float gap,
        float fadeIn,
        float fadeOutPrev,
        uint tk
    )
    {
        yield return CoFadeOut(src, fadeOutPrev);
        for (int i = 0; i < clips.Count; i++)
        {
            if (tk != token)
                yield break;
            var c = clips[i];
            if (!c)
                continue;
            src.clip = c;
            src.volume = 0f;
            src.Play();
            yield return CoFadeTo(src, 1f, fadeIn);
            while (src.isPlaying && tk == token)
                yield return null;
            if (i < clips.Count - 1 && gap > 0f && tk == token)
                yield return new WaitForSeconds(gap);
        }
        yield return CoFadeOut(src, defaultFadeOut);
    }

    static IEnumerator CoFadeOut(AudioSource s, float dur)
    {
        if (!s.isPlaying || dur <= 0f)
        {
            s.Stop();
            s.volume = 1f;
            yield break;
        }
        float start = s.volume,
            t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            s.volume = Mathf.Lerp(start, 0f, t / dur);
            yield return null;
        }
        s.Stop();
        s.volume = 1f;
    }

    static IEnumerator CoFadeTo(AudioSource s, float target, float dur)
    {
        if (dur <= 0f)
        {
            s.volume = target;
            yield break;
        }
        float start = s.volume,
            t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            s.volume = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        s.volume = target;
    }
}
