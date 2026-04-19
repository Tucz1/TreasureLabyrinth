using UnityEngine;

public class PositionPlayer : MonoBehaviour
{
    
    void Start()
    {
        var player = FindAnyObjectByType<GridMovement>();
        player.transform.position = transform.position;
    }

}
