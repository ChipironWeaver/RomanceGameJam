using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _iceCubeSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _waterSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _dropSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _blingSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _uiClickSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _chattingSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _music = new List<AudioClip>();
    
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    
    [SerializeField] private bool _playMusicOnAwake;
    
    void OnEnable()
    {
        Singleton();
    }
    
    public void PlayMusic(int index)
    {
        _musicAudioSource.clip = _music[index];
        _musicAudioSource.Play();
    }

    public void PlaySfx(SfxType type)
    {
        switch (type)
        {
            case SfxType.IceCube:
                _sfxAudioSource.PlayOneShot(_iceCubeSfx[Random.Range(0, _iceCubeSfx.Count)]);
                break;
            case SfxType.Bling:
                _sfxAudioSource.PlayOneShot(_blingSfx[Random.Range(0, _blingSfx.Count)]);
                break;
            case SfxType.Water:
                _sfxAudioSource.PlayOneShot(_waterSfx[Random.Range(0, _waterSfx.Count)]);
                break;
            case SfxType.Drop:
                _sfxAudioSource.PlayOneShot(_dropSfx[Random.Range(0, _dropSfx.Count)]);
                break;
            case SfxType.UiClick:
                _sfxAudioSource.PlayOneShot(_uiClickSfx[Random.Range(0, _uiClickSfx.Count)]);
                break;
            case SfxType.Chatting:
                _sfxAudioSource.PlayOneShot(_chattingSfx[Random.Range(0, _chattingSfx.Count)]);
                break;
        }
    }

    public void PlayUiClick()
    {
        PlaySfx(SfxType.UiClick);
    }
    
    public static AudioManager Instance{ get; private set; }
    void Singleton()
    {
        if (Instance !=null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}

[Serializable]
public enum SfxType
{
    None,
    IceCube,
    Bling,
    Water,
    Drop,
    UiClick,
    Chatting
}