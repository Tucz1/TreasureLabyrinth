using TMPro;
using UnityEngine;
using UnityEngine.UI;
public struct ArtifactData 
{
    public Sprite image;
    public string name;
}
public class ArtifactDisplay : MonoBehaviour
{
    [SerializeField] private Image m_image;
    [SerializeField] private TMP_Text m_text;

    public void Bind(ArtifactData data) 
    {
        if (data.image == null) 
        {
            Debug.LogWarning($"Artifact Image == NULL");
            return;
        }
        if (string.IsNullOrEmpty(data.name)) 
        {
            Debug.LogWarning($"Artifact Name == NULL");
            return;
        }
        m_image.sprite = data.image;
        m_text.text = data.name.ToUpper();
    }
}
