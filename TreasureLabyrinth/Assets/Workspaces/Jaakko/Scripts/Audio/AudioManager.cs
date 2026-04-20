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
        float dB = Mathf.Log10(value) * 20;
        m_mainMixer.SetFloat("MusicVolume", dB);
    }
    private void SfxVolumeChanged(float value) 
    {
        float dB = Mathf.Log10(value) * 20;
        m_mainMixer.SetFloat("SfxVolume", dB);


    }
}
