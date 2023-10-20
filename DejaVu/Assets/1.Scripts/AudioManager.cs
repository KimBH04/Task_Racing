using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioSource bgmSource;
    [Range(0f, 1f)] public float bgmVolume = 0.3f;

    [Header("SFX")]
    [SerializeField] private AudioClip[] sfxClips;
    [Range(0f, 1f)] public float sfxVolume = 0.5f;

    private Dictionary<string, AudioClip> sfxClipsDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        foreach (AudioClip clip in sfxClips)
        {
            sfxClipsDict.Add(clip.name, clip);
        }
    }

    public void PlayAudio(AudioSource source, string audioName)
    {
        if (!source.isPlaying)
        {
            source.clip = sfxClipsDict[audioName];
            source.volume = sfxVolume;
            source.Play();
        }
    }

    public void StopAudio(AudioSource source)
    {
        source.Stop();
    }
}
