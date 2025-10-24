// NarrationDirector.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AudioDirector : MonoBehaviour
{
    public static AudioDirector Instance { get; private set; }

    [Header("Fades")]
    [SerializeField, Min(0f)]
    float defaultFadeIn = 0.2f;

    [SerializeField, Min(0f)]
    float defaultFadeOut = 0.25f;

    AudioSource src;
    Coroutine routine;
    uint token; // cancels in-flight coroutines

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f; // 2D voiceover
        Debug.Log($"[AudioDirector] Initialized");
    }

    // ---- Public API ----
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

    // ---- Helpers ----
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

            // wait until clip ends or cancelled
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
