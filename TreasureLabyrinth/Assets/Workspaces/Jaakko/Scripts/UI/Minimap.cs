using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Image m_mapImage;

    public void MapChanged(Texture2D tex) 
    {
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f));

        m_mapImage.sprite = sprite;


        List<Discoverable> player = DiscoverableManager
            .GetDiscoverablesOfType(DiscoverableType.Player);

        if (player == null) 
        {
            Debug.LogWarning("Player NULL");
            return;
        }
        Debug.Log(player[0].name);
            
    }
}
