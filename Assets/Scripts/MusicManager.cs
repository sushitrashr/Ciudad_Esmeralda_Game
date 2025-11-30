using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("🎵 Música principal")]
    public AudioSource audioSource;
    public AudioClip musicaFondo;

    [Range(0f, 1f)]
    public float volume = 0.7f;

    [Header("🎚️ Efectos Fade")]
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Evitar duplicados entre escenas
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // inicia silencioso para el fade-in
    }

    void Start()
    {
        if (musicaFondo != null)
        {
            audioSource.clip = musicaFondo;
            audioSource.Play();
            StartCoroutine(FadeIn(audioSource, fadeInTime, volume));
        }
    }

    // 🔊 Fade In
    IEnumerator FadeIn(AudioSource source, float duration, float targetVolume)
    {
        float startVolume = 0f;
        source.volume = 0f;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    // 🔇 Fade Out
    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    // 🎶 Cambiar música con transición suave
    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(CambiarMusicaSuave(nuevaMusica));
    }

    IEnumerator CambiarMusicaSuave(AudioClip nuevaMusica)
    {
        // Fade out de la actual
        yield return FadeOut(audioSource, fadeOutTime);

        // Reproducir nueva
        audioSource.clip = nuevaMusica;
        audioSource.Play();

        // Fade in de la nueva
        yield return FadeIn(audioSource, fadeInTime, volume);
    }

    // 🔈 Control manual del volumen global
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    // 🔇 Apagar música con fade
    public void StopMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut(audioSource, fadeOutTime));
    }
}
