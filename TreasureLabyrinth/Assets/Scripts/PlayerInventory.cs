using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    GridMovement player;
    public int artifactsHeld;

    void Start()
    {
        player = FindAnyObjectByType<GridMovement>();
        player.InteractWithArtifact += AddArtifact;
    }

    void AddArtifact()
    {
        artifactsHeld++;
    }
}
