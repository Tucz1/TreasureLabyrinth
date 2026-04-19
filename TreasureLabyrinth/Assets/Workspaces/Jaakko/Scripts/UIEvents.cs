using System;
using UnityEditor;

public static class UIEvents 
{
    public static event Action<bool> OnWindowToggleChanged;
    public static event Action<bool> OnAutorunToggleChanged;
    public static event Action<bool> OnCrtFilerToggleChanged;

    public static event Action<float> OnMusicVolumeChanged;
    public static event Action<float> OnSfxVolumeChanged;

    public static event Action<ArtifactData> OnArtifactAdded;

    public static event Action<float> OnBrightnessValueChanged;

    public static void WindowToggleChanged(bool value) 
    {
        OnWindowToggleChanged?.Invoke(value);
    }
    public static void AutorunToggleChanged(bool value) 
    {
        OnAutorunToggleChanged?.Invoke(value);
    }
    public static void CrtFilterToggleChanged(bool value) 
    {
        OnCrtFilerToggleChanged?.Invoke(value);
    }

    public static void MusicVolumeChanged(float value) 
    {
        OnMusicVolumeChanged?.Invoke(value);    
    }
    public static void SfxVolumeChanged(float value) 
    {
        OnSfxVolumeChanged?.Invoke(value);
    }
    public static void BrighnessValueChanged(float value) 
    {
        OnBrightnessValueChanged?.Invoke(value);
    }
    public static void ArtifactAdded(ArtifactData data) 
    {
        OnArtifactAdded?.Invoke(data);
    }

}