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
    public bool Discovered { get; private set; }
    public void SetDiscovered(bool value) 
    {
        Discovered = value;
    }
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
