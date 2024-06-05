using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private void Awake()
    {
        InitializeSounds();
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
        }
    }

    private void OnDestroy()
    {
        Debug.Log("AudioManager is being destroyed.");
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
}
