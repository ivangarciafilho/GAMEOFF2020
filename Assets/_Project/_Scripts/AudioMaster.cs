using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    public static AudioMaster instance;

    AudioSource[] sources;
    int sourceIndex = 0;

    private void Awake()
    {
        instance = this;

        sources = new AudioSource[6];

        for (int i = 0; i < sources.Length; i++)
        {
            AudioSource src = sources[i] = new GameObject("AudioSrc" + i).AddComponent<AudioSource>();

            src.playOnAwake = false;
            src.loop = false;
        }
    }

    public void PlaySfx(AudioClip clip, float volume = 0.8f)
    {
        if (sourceIndex >= sources.Length) sourceIndex = 0;

        AudioSource src = sources[sourceIndex++];

        src.volume = volume;
        src.clip = clip;
        src.Play();
    }
}
