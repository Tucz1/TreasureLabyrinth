using UnityEngine;
public enum DiscoverableType 
{
    Artifact,
    Enemy,
    Player
}
public class Discoverable : MonoBehaviour
{
    [SerializeField] private DiscoverableType type;
    public DiscoverableType Type => type;
    private void Awake()
    {
        DiscoverableManager.Add(this);
    }
    private void OnDestroy()
    {
        DiscoverableManager.Remove(this);
    }
}
