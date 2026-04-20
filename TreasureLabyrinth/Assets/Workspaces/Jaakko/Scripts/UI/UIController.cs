using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum UIPanelType 
{
    Pause,
    HUD,
    Menu,
    Credits,
    Win,
    Lose
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
                UpdateCurrent(1);
                break;
            case NavigationDirection.Down:
                UpdateCurrent(-1);
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
    public void SetCurrentSelected(GameObject go) 
    {
        EventSystem.current.SetSelectedGameObject(go);
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
    [SerializeField] private Button m_mainCredits;
    [SerializeField] private Button m_creditsBack;
    [SerializeField] private Button m_winEndButton;
    [SerializeField] private Button m_loseEndButton;
    [Header("Prefabs")]
    [SerializeField] private ArtifactDisplay m_artifactDisplayPrefab;

    [Header("Text")]
    [SerializeField] private TMP_Text m_artifactsText;
    [Header("Anchors")]
    [SerializeField] private Transform m_artifactAnchor;

    [SerializeField] private GameObject m_pausePanel;
    [SerializeField] private GameObject m_gamePanel;
    [SerializeField] private GameObject m_menuPanel;
    [SerializeField] private GameObject m_creditsPanel;

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
                m_pauseActive = value;

                if (value) 
                {                    
                    var selectables = new List<Selectable>();

                    selectables.Add(m_sfxSlider);
                    selectables.Add(m_musicSlider);
                    selectables.Add(m_quitButton);
                    selectables.Add(m_resumeButton);
                    
                    nav.UpdateSelectables(selectables);
                    Time.timeScale = 0;
                }
                else 
                {
                    TogglePanel(UIPanelType.HUD, true);
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
                    selectables.Add(m_mainCredits);

                    nav.UpdateSelectables(selectables);
                }                
                break;
            case UIPanelType.Credits:
                m_creditsPanel.SetActive(value);

                if (value) 
                {
                    var s = new List<Selectable>();

                    s.Add(m_creditsBack);
                    nav.UpdateSelectables(s);

                    nav.SetCurrentSelected(m_creditsBack.gameObject);
                }
                break;
            case UIPanelType.Win:
                m_winPanel.SetActive(value);
                if (value) 
                {
                    StartCoroutine(FadeImage(m_winPanel.GetComponent<Image>()));
                    var s = new List<Selectable>();
                    s.Add(m_winEndButton);

                    nav.UpdateSelectables(s);
                    nav.SetCurrentSelected(m_winEndButton.gameObject);
                }
                break;
            case UIPanelType.Lose:
                m_losePanel.SetActive(value);                
                if (value) 
                {
                    StartCoroutine(FadeImage(m_losePanel.GetComponent<Image>()));
                    var s = new List<Selectable>();

                    s.Add(m_loseEndButton);
                    nav.UpdateSelectables(s);
                    nav.SetCurrentSelected(m_loseEndButton.gameObject);
                }
                break;
        }
    }
    private IEnumerator FadeImage(Image i) 
    {

        Color c = i.color;
        c.a = 0f;
        i.color = c;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime;
            c.a = Mathf.Clamp01(c.a);

            i.color = c;

            yield return null;
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
        m_sfxSlider.value = 1f;

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
        m_mainCredits.onClick.AddListener(() =>
        {
            TogglePanel(UIPanelType.Credits, true);
        });
        m_creditsBack.onClick.AddListener(() =>
        {
            TogglePanel(UIPanelType.Credits, false);
            TogglePanel(UIPanelType.Menu, true);
        });
        m_winEndButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
        m_loseEndButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });


        UIEvents.OnArtifactAdded += ArtifactAdded;
        SceneManager.sceneLoaded += SceneLoaded;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        Time.timeScale = 1;
        m_gameOver = false;

        m_artifactsCollected = 0;
        m_artifactsText.text = $"{m_artifactsCollected} / 4 COLLECTED";

        if (m_artifactDisplays.Count > 0) 
        {
            foreach(var a in m_artifactDisplays) 
            {
                Destroy(a.gameObject);
            }
            m_artifactDisplays.Clear();
        }
        if (scene.name == "MenuScene") 
        {
            TogglePanel(UIPanelType.Win, false);
            TogglePanel(UIPanelType.Lose, false);
            TogglePanel(UIPanelType.HUD, false);
            TogglePanel(UIPanelType.Pause, false);
            TogglePanel(UIPanelType.Credits, false);

            TogglePanel(UIPanelType.Menu, true);            
        }
        else 
        {
            TogglePanel(UIPanelType.Menu, false);
            TogglePanel(UIPanelType.Pause, false);
            TogglePanel(UIPanelType.Credits, false);

            TogglePanel(UIPanelType.HUD, true);

            Map m = FindAnyObjectByType<Map>();
            if (m)
                m.OnMapChanged += MapChanged;
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

        Map m = FindAnyObjectByType<Map>();
        if (m)
            m.OnMapChanged -= MapChanged;

        m_musicSlider.onValueChanged.RemoveAllListeners();
        m_sfxSlider.onValueChanged.RemoveAllListeners();

        m_resumeButton.onClick.RemoveAllListeners();

        InputEvents.OnUIInputAction -= UIInput;

        UIEvents.OnArtifactAdded -= ArtifactAdded;
    }
    private bool m_pauseActive;
    private void UIInput(UIInputAction action) 
    {
        switch (action) 
        {
            case UIInputAction.Pause:
                TogglePanel(UIPanelType.HUD, false);
                TogglePanel(UIPanelType.Pause, !m_pauseActive);
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
    private int m_artifactsCollected = 0;

    private List<ArtifactDisplay> m_artifactDisplays = new();
    private void ArtifactAdded(ArtifactData data) 
    {
        ArtifactDisplay a = Instantiate(m_artifactDisplayPrefab, m_artifactAnchor);
        a.Bind(data);

        m_artifactDisplays.Add(a);
        m_artifactsCollected++;
        m_artifactsText.text = $"{m_artifactsCollected} / 4 COLLECTED";
    }
    [SerializeField] private GameObject m_winPanel;
    [SerializeField] private GameObject m_losePanel;

    private bool m_gameOver = false;
    public void GameEnded(bool won) 
    {
        if (m_gameOver) return;
        UIEvents.GameOver();

        m_gameOver = true;
        Time.timeScale = 0;

        m_sfxSlider.value = 0f;

        TogglePanel(UIPanelType.HUD, false);
        TogglePanel(UIPanelType.Pause, false);

        if (won) 
        {
            TogglePanel(UIPanelType.Win, true);
        }
        else 
        {
            TogglePanel(UIPanelType.Lose, true);
        }
    }
}
