using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UISceneTransitionLoader : MonoBehaviour
{
    [SerializeField] private bool _autoFadeIn;
    [SerializeField] private Color _fadeColor =  Color.white;
    [SerializeField] private float _fadeDuration = 1f;

    private Image _image;
    
    private void Start()
    {
        _image = GetComponent<Image>();
        if  (_autoFadeIn) FadeIn();
    }

    public void FadeOut(string sceneName)
    {
        Color color = _fadeColor;
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.SetUpdate(true);
        fadeOutSequence.Append(_image.DOColor(color, _fadeDuration));
        fadeOutSequence.OnComplete(() =>
        {
            Time.timeScale = 1;
            if (sceneName != null) SceneManager.LoadScene(sceneName);
            
        }) ;
    }

    private void FadeIn()
    {
        _image.color = _fadeColor;
        Sequence fadeInSequence = DOTween.Sequence();
        print("im starting");
        fadeInSequence.Append(_image.DOColor(Color.clear, _fadeDuration));
        fadeInSequence.OnComplete(() =>
            {
                _image.color = Color.clear;
            }
        );
        fadeInSequence.SetUpdate(true);
        fadeInSequence.Play();
    }

    [Button]
    private void Test()
    {
        Time.timeScale = 1;
    }
}
