using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum UIPanelType 
{
    Pause,
    HUD,
    Menu
}
public class UINavigation 
{
    public enum NavigationDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    private List<Selectable> m_selectables = new();
    private Selectable m_current = null;
    public void UpdateSelectables(List<Selectable> s) 
    {
        m_selectables.Clear();

        if (s == null) 
        {
            return;
        }        
        m_selectables = s;
    }
    public void Navigate(NavigationDirection dir) 
    {
        Debug.Log(dir);

        if (m_current != null) 
        {
            if (m_current is Slider slider) 
            {
                switch (dir)
                {
                    case NavigationDirection.Left:
                        slider.value -= slider.wholeNumbers ? 3 : slider.maxValue * 0.2f;
                        return;

                    case NavigationDirection.Right:
                        slider.value += slider.wholeNumbers ? 3 : slider.maxValue * 0.2f;
                        return;
                }
            }
        }
        switch (dir) 
        {
            case NavigationDirection.Up:
                UpdateCurrent(-1);
                break;
            case NavigationDirection.Down:
                UpdateCurrent(1);
                break;
        }
    }
    private void UpdateCurrent(int step)
    {
        if (m_selectables.Count == 0)
            return;

        int i;

        if (m_current == null)
        {
            i = 0;
        }
        else
        {
            i = m_selectables.IndexOf(m_current);
            if (i == -1)
                i = 0;
        }

        i = (i + step + m_selectables.Count) % m_selectables.Count;
        
        m_current = m_selectables[i];
        EventSystem.current.SetSelectedGameObject(m_current.gameObject);
    }

}
[DefaultExecutionOrder(-100)]
public class UIController : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;
    [Header("Buttons")]
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private Button m_mainStart;
    [SerializeField] private Button m_mainQuit;
    [Header("Prefabs")]
    [SerializeField] private ArtifactDisplay m_artifactDisplayPrefab;
    
    [Header("Anchors")]
    [SerializeField] private Transform m_artifactAnchor;

    [SerializeField] private GameObject m_pausePanel;
    [SerializeField] private GameObject m_gamePanel;
    [SerializeField] private GameObject m_menuPanel;

    public static UIController I { get; private set; }

    private Minimap m_minimap;

    private UINavigation nav;

    public void TogglePanel(UIPanelType type, bool value) 
    {
        if (!value) 
        {
            nav.UpdateSelectables(null); 
        }
        switch (type) 
        {
            case UIPanelType.Pause:
                m_pausePanel.SetActive(value);
                if (value) 
                {
                    var selectables = new List<Selectable>();

                    selectables.Add(m_resumeButton);
                    selectables.Add(m_quitButton);
                    selectables.Add(m_musicSlider);
                    selectables.Add(m_sfxSlider);

                    nav.UpdateSelectables(selectables);
                    Time.timeScale = 0;
                }
                else 
                {
                    Time.timeScale = 1;
                }
                    break;
            case UIPanelType.HUD:
                m_gamePanel.SetActive(value);
                nav.UpdateSelectables(null);
                break;
            case UIPanelType.Menu:
                m_menuPanel.SetActive(value);
                if (value) 
                {
                    var selectables = new List<Selectable>();

                    selectables.Add(m_mainStart);
                    selectables.Add(m_mainQuit);

                    nav.UpdateSelectables(selectables);
                }                
                break;
        }
    }
    private Canvas m_canvas;
    private void Awake()
    {
        if (I != null) 
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(this);

        m_canvas = GetComponentInChildren<Canvas>();        
        nav = new UINavigation();
        m_minimap = GetComponent<Minimap>();

        Map.OnMapChanged += MapChanged;

        m_musicSlider.onValueChanged.AddListener(UIEvents.MusicVolumeChanged);
        m_sfxSlider.onValueChanged.AddListener(UIEvents.SfxVolumeChanged);

        InputEvents.OnUIInputAction += UIInput;

        m_resumeButton.onClick.AddListener(() =>
        {
            TogglePanel(UIPanelType.Pause, false);
        });
        m_quitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
        m_mainStart.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameplayScene");
        });
        m_mainQuit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        TogglePanel(UIPanelType.Pause, false);



        UIEvents.OnArtifactAdded += ArtifactAdded;
        SceneManager.sceneLoaded += SceneLoaded;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        Time.timeScale = 1;

        if (scene.name == "MenuScene") 
        {
            TogglePanel(UIPanelType.HUD, false);
            TogglePanel(UIPanelType.Pause, false);
            TogglePanel(UIPanelType.Menu, true);
        }
        else 
        {
            TogglePanel(UIPanelType.Menu, false);
            TogglePanel(UIPanelType.Pause, false);
            TogglePanel(UIPanelType.HUD, true);
        }

        m_canvas.worldCamera = Camera.main;
    }
    private void OnDestroy()
    {
        if (I != this) 
        {
            return;
        }
        I = null;

        Map.OnMapChanged -= MapChanged;

        m_musicSlider.onValueChanged.RemoveAllListeners();
        m_sfxSlider.onValueChanged.RemoveAllListeners();

        m_resumeButton.onClick.RemoveAllListeners();

        InputEvents.OnUIInputAction -= UIInput;

        UIEvents.OnArtifactAdded -= ArtifactAdded;
    }  
    private void UIInput(UIInputAction action) 
    {
        switch (action) 
        {
            case UIInputAction.Pause:
                TogglePanel(UIPanelType.Pause, true);
                break;
            case UIInputAction.NavUp:
                nav.Navigate(UINavigation.NavigationDirection.Up);
                break;
            case UIInputAction.NavDown:
                nav.Navigate(UINavigation.NavigationDirection.Down);
                break;
            case UIInputAction.NavLeft:
                nav.Navigate(UINavigation.NavigationDirection.Left);
                break;
            case UIInputAction.NavRight:
                nav.Navigate(UINavigation.NavigationDirection.Right);
                break;
        }

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
    [SerializeField] private GameObject m_winPanel;
    [SerializeField] private GameObject m_losePanel;
    public void GameEnded(bool won) 
    {
        Time.timeScale = 0;

        if (won) 
        {
            m_winPanel.SetActive(true);
        }
        else 
        {
            m_losePanel.SetActive(true);
        }
    }
}
