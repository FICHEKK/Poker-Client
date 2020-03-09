using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public sealed class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundAudioClipPair[] soundAudioClipPairs;
    
    private static readonly Dictionary<Sound, AudioClip[]> SoundToClip = new Dictionary<Sound, AudioClip[]>();
    private AudioSource audioSource;
    
    public static AudioManager Instance { get; private set; }
    
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
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        foreach (var pair in soundAudioClipPairs)
        {
            SoundToClip.Add(pair.sound, pair.audioClips);
        }
    }

    public void Play(Sound sound)
    {
        if (SoundToClip.ContainsKey(sound))
        {
            var audioClips = SoundToClip[sound];
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }
        else
        { 
            Debug.LogWarning("Sound '" + sound + "' does not exist in the dictionary.");
        }
    }

    [Serializable]
    private class SoundAudioClipPair
    {
        public Sound sound;
        public AudioClip[] audioClips;
    }
}
