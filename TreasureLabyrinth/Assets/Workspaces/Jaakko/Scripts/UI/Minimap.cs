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

    private int mapSizeX;
    private int mapSizeY;

    private Transform playerTransform;

    private List<GameObject> m_artifacts = new();
    private List<GameObject> m_enemies = new();

    public void MapChanged(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f));

        if (m_artifacts.Count > 0) 
        {
            foreach (var a in m_artifacts)
                Destroy(a);
        }
        m_artifacts.Clear();

        if (m_enemies.Count > 0) 
        {
            foreach(var a in m_enemies)
                Destroy(a);
        }
        m_enemies.Clear();

        m_mapImage.sprite = sprite;
        mapSizeX = tex.width;
        mapSizeY = tex.height;

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
        foreach (Discoverable artifact in artifacts) 
        {
            GameObject g = Instantiate(m_atrifactIcon, m_mapImage.transform);            
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.anchoredPosition = GetUIPos(artifact.transform.position);

            m_artifacts.Add(g);
        }

        List<Discoverable> enemies = DiscoverableManager
            .GetDiscoverablesOfType(DiscoverableType.Enemy);
        if (enemies == null || enemies.Count == 0) 
        {
            Debug.LogWarning("Enemies NULL");
            return;
        }
        foreach (Discoverable enemy in enemies) 
        {
            // spawn shit
        }
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
    }

    void UpdatePlayerIcon()
    {
        playerIcon.anchoredPosition = GetUIPos(playerTransform.position);
    }
}