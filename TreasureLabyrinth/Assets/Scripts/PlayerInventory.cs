using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    GridMovement player;
    public int artifactsHeld;

    AudioSource myAudio;
    public AudioClip unlock;

    void Start()
    {
        player = FindAnyObjectByType<GridMovement>();
        myAudio = GetComponent<AudioSource>();
        player.InteractWithArtifact += AddArtifact;
    }

    void AddArtifact()
    {
        artifactsHeld++;
        Debug.Log($"Artifacts held: {artifactsHeld}");

        if(artifactsHeld >= 4)
        {
            OpenTrapDoor();
        }
    }

    void OpenTrapDoor()
    {
        myAudio.PlayOneShot(unlock);
    }
}
