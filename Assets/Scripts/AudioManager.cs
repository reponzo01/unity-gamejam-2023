using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [SerializeField] private AudioSource sfxBirds;
    [SerializeField] private AudioSource sfxExclamation;
    [SerializeField] private AudioSource sfxRumble;
    [SerializeField] private AudioSource sfxMagicTransform;
    [SerializeField] private AudioSource sfxWoosh;
    [SerializeField] private AudioSource sfxExplosion;
    [SerializeField] private AudioSource sfxPowerup;
    [SerializeField] private AudioSource musicMainMenu;
    [SerializeField] private AudioSource musicPanels3To5;
    [SerializeField] private AudioSource musicPanels6To8;
    [SerializeField] private AudioSource musicPanelFinal;
    [SerializeField] private AudioSource musicGameplay;
    [SerializeField] private AudioSource musicZenGameplay;
    [SerializeField] private AudioClip[] clipsGameplay;
    [SerializeField] private AudioClip[] clipsExplosion;

    private AudioSource[] _allAudioSources;
    private bool _isZenMusicEnabled = true;

    public static AudioManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Audio Manager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _allAudioSources = FindObjectsOfType(typeof(AudioSource), true) as AudioSource[];
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("IsZenMusicEnabled"))
        {
            _isZenMusicEnabled = PlayerPrefs.GetInt("IsZenMusicEnabled", 0) == 1 ? true : false;
        }
        CanvasManager.Instance.UpdateToggleZenMusicButtonText(_isZenMusicEnabled);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopAllAudio()
    {
        foreach (var audio in _allAudioSources)
        {
            if (audio.isPlaying) audio.Stop();
        }
    }

    public void PlayMusicMainMenu()
    {
        StopAllAudio();
        if (!musicMainMenu.isPlaying) musicMainMenu.Play();
    }

    public void PlayMusicPanels3To5()
    {
        StopAllAudio();
        if (!musicPanels3To5.isPlaying) musicPanels3To5.Play();
    }

    public void PlayMusicPanels6To8()
    {
        StopAllAudio();
        if (!musicPanels6To8.isPlaying) musicPanels6To8.Play();
    }

    public void PlayMusicPanelFinal()
    {
        if (sfxRumble.isPlaying) sfxRumble.Stop();
        if (!musicPanelFinal.isPlaying) musicPanelFinal.Play();
    }

    public void PlayZenGameplayMusicIfEnabled()
    {
        if (!_isZenMusicEnabled) return;
        if (!musicZenGameplay.isPlaying)
        {
            StopAllAudio();
            musicZenGameplay.Play();
        }
    }

    public void PlayRandomGameplayMusic()
    {
        StopAllAudio();
        var clipNumber = Random.Range(0, clipsGameplay.Length);
        musicGameplay.clip = clipsGameplay[clipNumber];
        musicGameplay.Play();
    }

    public void PlaySFXBirds()
    {
        if (!sfxBirds.isPlaying) sfxBirds.Play();
    }

    public void PlaySFXExclamation()
    {
        if (!sfxExclamation.isPlaying) sfxExclamation.Play();
    }

    public void PlaySFXRumble()
    {
        if (!sfxRumble.isPlaying) sfxRumble.Play();
    }

    public void PlaySFXMagicTransform()
    {
        if (!sfxMagicTransform.isPlaying) sfxMagicTransform.Play();
    }

    public void PlaySFXWoosh()
    {
        if (sfxWoosh.isPlaying) sfxWoosh.Stop();
        sfxWoosh.Play();
    }

    public void PlaySFXPowerup()
    {
        if (sfxPowerup.isPlaying) sfxPowerup.Stop();
        sfxPowerup.Play();
    }

    public void PlayRandomExplosionSFX()
    {
        if (sfxExplosion.isPlaying) sfxExplosion.Stop();
        var clipNumber = Random.Range(0, clipsExplosion.Length);
        sfxExplosion.clip = clipsExplosion[clipNumber];
        sfxExplosion.Play();
    }

    public void ToggleZenMusic()
    {
        if (musicZenGameplay.isPlaying) musicZenGameplay.Stop();
        else musicZenGameplay.Play();
        CanvasManager.Instance.UpdateToggleZenMusicButtonText(musicZenGameplay.isPlaying);
        PlayerPrefs.SetInt("IsZenMusicEnabled", musicZenGameplay.isPlaying ? 1 : 0);
    }
}
