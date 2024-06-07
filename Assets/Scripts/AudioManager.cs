using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public Sound[] sounds;
    public float fadeDuration = 1.0f; // Duração para fade in/out

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            if (s.clip == null)
            {
                Debug.LogWarning("Sound: " + s.name + " does not have a clip assigned.");
                continue;
            }

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.savedTime = 0f; // Inicializa savedTime
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " does not have an AudioSource component.");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " does not have an AudioSource component.");
            return;
        }
        s.source.Stop();
        s.savedTime = 0f; // Reseta o tempo de reprodução salvo
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " does not have an AudioSource component.");
            return;
        }
        s.savedTime = s.source.time; // Salva o tempo de reprodução atual
        s.source.Pause();
    }

    public void Continue(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " does not have an AudioSource component.");
            return;
        }
        if (s.savedTime > 0)
        {
            s.source.time = s.savedTime; // Define o tempo de reprodução salvo
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " has not been paused or has not started.");
        }
    }

    public void FadeIn(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " does not have an AudioSource component.");
            return;
        }
        StartCoroutine(FadeInCoroutine(s));
    }

    public void FadeOut(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " does not have an AudioSource component.");
            return;
        }
        StartCoroutine(FadeOutCoroutine(s));
    }

    public void SwitchSound(string oldSound, string newSound)
    {
        StartCoroutine(SwitchSoundCoroutine(oldSound, newSound));
    }

    private IEnumerator FadeInCoroutine(Sound s)
    {
        s.source.volume = 0;
        s.source.Play();
        
        while (s.source.volume < s.volume)
        {
            s.source.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        s.source.volume = s.volume;
    }

    private IEnumerator FadeOutCoroutine(Sound s)
    {
        float startVolume = s.source.volume;

        while (s.source.volume > 0)
        {
            s.source.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        s.source.Stop();
        s.source.volume = startVolume;
    }

    private IEnumerator SwitchSoundCoroutine(string oldSound, string newSound)
    {
        Sound oldS = Array.Find(sounds, sound => sound.name == oldSound);
        Sound newS = Array.Find(sounds, sound => sound.name == newSound);

        Coroutine fadeOutCoroutine = null;
        Coroutine fadeInCoroutine = null;

        if (oldS != null && oldS.source != null)
        {
            fadeOutCoroutine = StartCoroutine(FadeOutCoroutine(oldS));
        }

        if (newS != null && newS.source != null)
        {
            fadeInCoroutine = StartCoroutine(FadeInCoroutine(newS));
        }

        // Espera os fades terminarem antes de continuar
        if (fadeOutCoroutine != null)
        {
            yield return fadeOutCoroutine;
        }
        
        if (fadeInCoroutine != null)
        {
            yield return fadeInCoroutine;
        }
    }
}
