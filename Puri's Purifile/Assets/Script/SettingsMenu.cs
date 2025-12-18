using UnityEngine;
using UnityEngine.Audio; // Required for Mixer
using UnityEngine.UI;    // Required for Sliders

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;

public void SetMasterVolume(float volume)
{
    if (volume <= -39f) 
    {
        mainMixer.SetFloat("MasterVol", -80f); // -80 is total silence in Unity
    }
    else 
    {
        mainMixer.SetFloat("MasterVol", volume);
    }
}

    public void SetMusicVolume(float volume)
    {
        // "MusicVol" must match the name in Exposed Parameters exactly
        mainMixer.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        // "SFXVol" must match the name in Exposed Parameters exactly
        mainMixer.SetFloat("SFXVol", volume);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}