using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Image m_mapImage;
    [SerializeField] private RectTransform playerIcon;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_atrifactIcon;
    [SerializeField] private GameObject m_enemyIcon;
    [SerializeField] private GameObject m_exitIcon;

    [SerializeField] private Material minimapMaterial;

    [SerializeField] private float m_pulseRadius = 0.2f;
    [SerializeField] private float m_pulseDuration = 3f;
    [SerializeField] private AnimationCurve m_pulseCurve;

    private int mapSizeX;
    private int mapSizeY;

    private Transform playerTransform;

    private List<GameObject> m_artifacts = new();
    private List<Discoverable> m_enemies = new();
    private List<Discoverable> m_discoverables = new();
    private Dictionary<Discoverable, GameObject> m_discoverableMap = new();
    private Dictionary<Discoverable, GameObject> m_enemyMap = new();

    private float pulseTimer = 10;
    private bool pulseStarted = false;

    private void Start()
    {
        InputEvents.OnInputAction += InputAction;
        UIEvents.OnEnemySpawned += EnemySpawned;
    }
    private void OnDestroy()
    {
        InputEvents.OnInputAction -= InputAction;
        UIEvents.OnEnemySpawned -= EnemySpawned;
    }
    private void InputAction(PlayerInputAction action) 
    {
        if (action == PlayerInputAction.Pulse) 
        {
            pulseStarted = true;
        }
    }
    public void MapChanged(Texture2D tex)
    {

        mapSizeX = tex.width;
        mapSizeY = tex.height;

        if (m_artifacts.Count > 0) 
        {
            foreach (var a in m_artifacts)
                Destroy(a);
        }

        if (m_enemies.Count > 0) 
        {
            foreach(var a in m_enemies)
                Destroy(a);
        }

        m_artifacts.Clear();
        m_enemies.Clear();
        m_discoverables.Clear();
        m_discoverableMap.Clear();
        m_enemyMap.Clear();

        List<Discoverable> players = DiscoverableManager
            .GetDiscoverablesOfType(DiscoverableType.Player);
        if (players == null || players.Count == 0)
        {
            Debug.LogWarning("Player NULL");
            return;
        }
        playerTransform = players[0].transform;

        List<Discoverable> artifacts = DiscoverableManager
            .GetDiscoverablesOfType(DiscoverableType.Artifact);
        if (artifacts == null || artifacts.Count == 0) 
        {
            Debug.LogWarning("Artifacts NULL");
            return;
        }
        int index = 0;
        foreach (Discoverable artifact in artifacts) 
        {
            index++;
            GameObject g = Instantiate(m_atrifactIcon, m_mapImage.transform);
            g.name = $"Artifact {index}";

            RectTransform rt = g.GetComponent<RectTransform>();
            rt.anchoredPosition = GetUIPos(artifact.transform.position);

            m_artifacts.Add(g);
            m_discoverableMap[artifact] = g;
            m_discoverables.Add(artifact);
        }

        List<Discoverable> exits = DiscoverableManager
            .GetDiscoverablesOfType(DiscoverableType.Exit);

        if (exits == null || exits.Count == 0) 
        {
            Debug.LogWarning("Exits NULL");
            return;
        }
        foreach (var e in exits) 
        {
            GameObject g = Instantiate(m_exitIcon, m_mapImage.transform);
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.anchoredPosition = GetUIPos(e.transform.position);

            m_discoverableMap[e] = g;
            m_discoverables.Add(e);            
        }
    }
    private void EnemySpawned(Discoverable enemy) 
    {
        GameObject g = Instantiate(m_enemyIcon, m_mapImage.transform);

        RectTransform rt = g.GetComponent<RectTransform>();
        rt.anchoredPosition = GetUIPos(enemy.transform.position);

        m_enemies.Add(enemy);
        m_discoverables.Add(enemy);
        m_discoverableMap[enemy] = g;
        m_enemyMap[enemy] = g;
    }
    Vector2 GetUIPos(Vector3 worldPos) 
    {
        Vector2 n = new Vector2(
            worldPos.x / mapSizeX,
            worldPos.y / mapSizeY
            );

        RectTransform rect = m_mapImage.rectTransform;

        Vector2 uiPos = new Vector2(
            n.x * rect.rect.width,
            n.y * rect.rect.height
        );

        uiPos -= rect.rect.size / 2f;

        return uiPos;
    }
    void Update()
    {
        if (playerTransform == null) return;

        UpdatePlayerIcon();

        if (pulseStarted) 
        {
            pulseTimer = 0f;
            pulseStarted = false;
        }

        pulseTimer += Time.deltaTime;
        var t = pulseTimer / m_pulseDuration;

        t = m_pulseCurve.Evaluate(t);

        float r = t * m_pulseRadius;
        minimapMaterial.SetFloat("_RadiusUV", r);

        UpdateEnemyIcons();
        CheckVisibleDiscoverables(r);
    }
    void CheckVisibleDiscoverables(float r) 
    {
        float magicNumber = 0.05f;
        if (r < magicNumber)
            r = magicNumber;

        for (int i = 0; i < m_discoverables.Count; i++) 
        {
            float dist = Vector2.Distance(GetPosition(playerTransform.position)
                , GetPosition(m_discoverables[i].transform.position));

            GameObject go = m_discoverableMap[m_discoverables[i]];
            DiscoverableDisplay d = go.GetComponent<DiscoverableDisplay>();
            if (d == null)
                return;


            if (m_discoverables[i].Discovered)
                go.SetActive(false);

            if (dist <= r + magicNumber) 
            {
                d.Toggle(true);    
            }
            else 
            {
                d.Toggle(false);
            }
        }
        
    }
    Vector2 GetPosition(Vector3 pos) 
    {
        return new Vector2
            (
                pos.x / mapSizeX,
                pos.y / mapSizeY
            );
    }
    void UpdateEnemyIcons() 
    {
        if (m_enemies == null || m_enemies.Count == 0)
            return;

        for (int i = 0; i < m_enemies.Count; i++) 
        {
            Discoverable e = m_enemies[i];
            if (e == null) continue;

            Vector2 p = GetUIPos(e.transform.position);
            m_enemyMap[e].GetComponent<RectTransform>().anchoredPosition = p;
        }
    }
    void UpdatePlayerIcon()
    {
        Vector2 p = GetUIPos(playerTransform.position);
        playerIcon.anchoredPosition = p;


        Vector2 s = new Vector2
            (
                playerTransform.position.x / mapSizeX,
                playerTransform.position.y / mapSizeY
            );
        minimapMaterial.SetVector("_PlayerUV", GetPosition(playerTransform.position));
    }
}