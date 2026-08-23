using System.Runtime.Serialization;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public SettingPreset currentSettingPreset;
    public SettingPreset defaultSettingPreset;

    [CurveRange(0,0,1,1,EColor.Violet)] public AnimationCurve volumeCurve;
    
    public AudioMixerGroup masterGroup;
    public Slider masterSlider;
    public AudioMixerGroup sfxGroup;
    public Slider sfxSlider;
    public AudioMixerGroup musicGroup;
    public Slider musicSlider;
    
    public DialogueController dialogueController;
    public Toggle autoTextToggle;
    
    public Toggle fullscreenToggle;
        
    public void OnEnable()
    {
        if(masterSlider) masterSlider.onValueChanged.AddListener((arg) =>
        {
            SetVolume(masterSlider.value, masterGroup);
            currentSettingPreset.masterVolume = masterSlider.value;
        });
        if(sfxSlider) sfxSlider.onValueChanged.AddListener((arg) =>
        {
            SetVolume(sfxSlider.value, sfxGroup);
            currentSettingPreset.sfxVolume = sfxSlider.value;
        });
        if(musicSlider) musicSlider.onValueChanged.AddListener((arg) =>
        {
            SetVolume(musicSlider.value, musicGroup);
            currentSettingPreset.musicVolume = musicSlider.value;
        });
        if(autoTextToggle) autoTextToggle.onValueChanged.AddListener(arg =>
        {
            currentSettingPreset.autoText = autoTextToggle.isOn;
            if(dialogueController) dialogueController.isOnAuto = currentSettingPreset.autoText;
        });
        if(fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(arg =>
        {
            currentSettingPreset.fullscreen = fullscreenToggle.isOn;
            Screen.SetResolution(Screen.width, Screen.height, currentSettingPreset.fullscreen);
        });
    }
    public void OnDisable()
    {
        SaveSystem.Save(currentSettingPreset,"currentSettingPreset");
        if(masterSlider) masterSlider.onValueChanged.RemoveListener((arg) =>
        {
            SetVolume(masterSlider.value, masterGroup);
            currentSettingPreset.masterVolume = masterSlider.value;
        });
        if(sfxSlider) sfxSlider.onValueChanged.RemoveListener((arg) =>
        {
            SetVolume(sfxSlider.value, sfxGroup);
            currentSettingPreset.sfxVolume = sfxSlider.value;
        });
        if(musicSlider) musicSlider.onValueChanged.RemoveListener((arg) =>
        {
            SetVolume(musicSlider.value, musicGroup);
            currentSettingPreset.musicVolume = musicSlider.value;
        });
        if(autoTextToggle) autoTextToggle.onValueChanged.RemoveListener(arg =>
        {
            currentSettingPreset.autoText = autoTextToggle.isOn;
            if(dialogueController) dialogueController.isOnAuto = currentSettingPreset.autoText;
        });
        if(fullscreenToggle) fullscreenToggle.onValueChanged.RemoveListener(arg =>
        {
            currentSettingPreset.fullscreen = fullscreenToggle.isOn;
            Screen.SetResolution(Screen.width, Screen.height, currentSettingPreset.fullscreen);
        });
    }

    public void SetAutoText(bool value)
    {
        currentSettingPreset.autoText = autoTextToggle.isOn;
        if(dialogueController)  dialogueController.isOnAuto = currentSettingPreset.autoText;
    }
    
    public void SetFullscreen(bool value)
    {
        currentSettingPreset.fullscreen = value;
        Screen.SetResolution(Screen.width, Screen.height, currentSettingPreset.fullscreen);
    }
    
    public void Start()
    {
        if (SaveSystem.FileExist("currentSettingPreset"))
        {
            currentSettingPreset = SaveSystem.Load<SettingPreset>("currentSettingPreset");
        }
        else
        {
            currentSettingPreset = defaultSettingPreset;
        }
        ApplyCurrentPreset();
    }
    public void QuitApp()
    {
        Application.Quit();
    }

    [Button]
    public void ApplyCurrentPreset()
    {
        SetVolume(currentSettingPreset.masterVolume,masterGroup);
        if(masterSlider) masterSlider.value = currentSettingPreset.masterVolume;
        SetVolume(currentSettingPreset.sfxVolume,sfxGroup);
        if(sfxSlider) sfxSlider.value = currentSettingPreset.sfxVolume;
        SetVolume(currentSettingPreset.musicVolume,musicGroup);
        if(musicSlider) musicSlider.value = currentSettingPreset.musicVolume;

        if(dialogueController) dialogueController.isOnAuto = currentSettingPreset.autoText;
        if(autoTextToggle)
        {
            autoTextToggle.isOn = currentSettingPreset.autoText;
            autoTextToggle.onValueChanged?.Invoke(autoTextToggle.isOn);
        }
        
        Screen.SetResolution(Screen.width, Screen.height, currentSettingPreset.fullscreen);
        if(fullscreenToggle)
        {
            fullscreenToggle.isOn = currentSettingPreset.fullscreen;
            fullscreenToggle.onValueChanged?.Invoke(fullscreenToggle.isOn);
        }
    }
    public void SetVolume(float volume, AudioMixerGroup group)
    {
        group.audioMixer.SetFloat(group.name , volumeCurve.Evaluate(volume) * 90 - 80);
    }
}