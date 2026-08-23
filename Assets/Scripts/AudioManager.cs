using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _deathSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _enemyDeathSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _eggPickupSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _jumpSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _uiClickSfx = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _music = new List<AudioClip>();
    
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    
    [SerializeField] private bool _playMusicOnAwake;

    void Start()
    {
        if(_playMusicOnAwake)PlayMusic();
    }

    void OnEnable()
    {
        Singleton();
    }
    
    public void PlayMusic()
    {
        _musicAudioSource.clip = _music[Random.Range(0, _music.Count)];
        _musicAudioSource.Play();
    }

    public void PlaySfx(SfxType type)
    {
        switch (type)
        {
            case SfxType.Death:
                _sfxAudioSource.PlayOneShot(_deathSfx[Random.Range(0, _deathSfx.Count)]);
                break;
            case SfxType.Jump:
                _sfxAudioSource.PlayOneShot(_jumpSfx[Random.Range(0, _jumpSfx.Count)]);
                break;
            case SfxType.EnemyDeath:
                _sfxAudioSource.PlayOneShot(_enemyDeathSfx[Random.Range(0, _enemyDeathSfx.Count)]);
                break;
            case SfxType.EggPickup:
                _sfxAudioSource.PlayOneShot(_eggPickupSfx[Random.Range(0, _eggPickupSfx.Count)]);
                break;
            case SfxType.UiClick:
                _sfxAudioSource.PlayOneShot(_uiClickSfx[Random.Range(0, _uiClickSfx.Count)]);
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
    Death,
    EnemyDeath,
    EggPickup,
    Jump,
    UiClick
}