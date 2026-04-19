using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class UIController : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] private ToggleButton m_windowToggle;
    [SerializeField] private ToggleButton m_autorunToggle;
    [SerializeField] private ToggleButton m_crtFilerToggle;
    [Header("Sliders")]
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;
    [SerializeField] private Slider m_brightnessSlider;
    [Header("Buttons")]
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Button m_quitButton;
    [Header("Prefabs")]
    [SerializeField] private ArtifactDisplay m_artifactDisplayPrefab;
    [Header("Anchors")]
    [SerializeField] private Transform m_artifactAnchor;

    public static UIController I { get; private set; }

    private Minimap m_minimap;
    public void Toggle(bool value) 
    {
        gameObject.SetActive(value);
    }

    private void Awake()
    {
        if (I != null) 
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(this);

        m_minimap = GetComponent<Minimap>();

        Map.OnMapChanged += MapChanged;

        m_windowToggle.OnValueChanged += UIEvents.WindowToggleChanged;
        m_autorunToggle.OnValueChanged += UIEvents.AutorunToggleChanged;
        m_crtFilerToggle.OnValueChanged += UIEvents.AutorunToggleChanged;

        m_musicSlider.onValueChanged.AddListener(UIEvents.MusicVolumeChanged);
        m_sfxSlider.onValueChanged.AddListener(UIEvents.SfxVolumeChanged);
        m_brightnessSlider.onValueChanged.AddListener(UIEvents.BrighnessValueChanged);

        m_resumeButton.onClick.AddListener(() =>
        {
            Toggle(false);
        });

        UIEvents.OnArtifactAdded += ArtifactAdded;
    }
    private void OnDestroy()
    {
        if (I != this) 
        {
            return;
        }
        I = null;

        Map.OnMapChanged -= MapChanged;

        m_windowToggle.OnValueChanged -= UIEvents.WindowToggleChanged;
        m_autorunToggle.OnValueChanged -= UIEvents.AutorunToggleChanged;
        m_crtFilerToggle.OnValueChanged -= UIEvents.AutorunToggleChanged;

        m_musicSlider.onValueChanged.RemoveAllListeners();
        m_sfxSlider.onValueChanged.RemoveAllListeners();
        m_brightnessSlider.onValueChanged.RemoveAllListeners();

        m_resumeButton.onClick.RemoveAllListeners();

        UIEvents.OnArtifactAdded -= ArtifactAdded;
    }  
    private void MapChanged(Texture2D tex) 
    {
        m_minimap.MapChanged(tex);
    }
    private void ArtifactAdded(ArtifactData data) 
    {
        ArtifactDisplay a = Instantiate(m_artifactDisplayPrefab, m_artifactAnchor);
        a.Bind(data);
    }
}
