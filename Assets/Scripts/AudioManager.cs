using System;
using System.Collections.Generic;
using DG.Tweening;
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

    [SerializeField] private int _autoMusicPlay = -1;
    private int _activeMusic = -1;
    [SerializeField] private float _musicTransitionTime = 0.5f;
    
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    
    [SerializeField] private bool _playMusicOnAwake;
    [SerializeField] private bool _continuePlayingMusicOnStop;

    private bool _startedPlaying;
    private void Update()
    {
        if (!_musicAudioSource.isPlaying && !_startedPlaying && _continuePlayingMusicOnStop)
        {
            _startedPlaying = true;
            int index = Random.Range(1, _music.Count);
            if (index == _activeMusic) index++;
            if (index >= _music.Count) index = 1;
            PlayMusic(index);
        }
    }

    void OnEnable()
    {
        Singleton();
    }

    void Start()
    {
        if (_autoMusicPlay >= 0 && _playMusicOnAwake && _autoMusicPlay < _music.Count)
        {
            PlayMusic(_autoMusicPlay);
        }
    }
    
    public void PlayMusic(int index)
    {
        if (index == _activeMusic) return;
        _activeMusic = index;
        if(_activeMusic == -1)
        {
            _musicAudioSource.clip = _music[index];
            _musicAudioSource.Play();
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_musicAudioSource.DOFade(0, _musicTransitionTime/2));
            sequence.AppendCallback(() =>
            {
                _musicAudioSource.clip = _music[index];
                _musicAudioSource.Play();
            });
            sequence.Append(_musicAudioSource.DOFade(1, _musicTransitionTime/2));
            sequence.OnComplete((() => _startedPlaying = false));
        }
    }

    public void PlaySfx(SfxType type)
    {
        _sfxAudioSource.pitch = Random.Range(0.95f, 1.05f);
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