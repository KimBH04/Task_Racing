using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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

    public void PlayAudioFadeIn(AudioSource source, string audioName, float fadeTime, float volume = 1f, bool loop = false)
    {
        if (!source.isPlaying)
        {
            source.clip = sfxClipsDict[audioName];
            source.loop = loop;
            source.Play();

            source.DOKill();
            source.DOFade(sfxVolume * volume, fadeTime);
        }
    }

    public void StopAudioFadeOut(AudioSource source, float fadeTime)
    {
        if (source.isPlaying)
        {
            source.DOFade(0f, fadeTime).OnComplete(source.Stop);
            source.loop = false;
        }
    }
}
