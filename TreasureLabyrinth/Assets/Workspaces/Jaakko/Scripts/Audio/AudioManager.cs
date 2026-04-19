using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I {get;private set;}
    [Header("Sources")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioSource m_sfxSource;

    [Header("Mixer")]
    [SerializeField] private AudioMixer m_mainMixer;

    private void Awake()
    {
        if (I != null) 
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(this);

        UIEvents.OnMusicVolumeChanged += MusicVolumeChanged;
        UIEvents.OnSfxVolumeChanged += SfxVolumeChanged;
    }
    private void OnDestroy()
    {
        if (I != this) 
        {
            return;
        }
        UIEvents.OnMusicVolumeChanged -= MusicVolumeChanged;
        UIEvents.OnSfxVolumeChanged -= SfxVolumeChanged;
    }
    private void MusicVolumeChanged(float value) 
    {
        m_mainMixer.SetFloat("MusicVolume", value);   
    }
    private void SfxVolumeChanged(float value) 
    {
        m_mainMixer.SetFloat("SfxVolume", value);
    }
}
