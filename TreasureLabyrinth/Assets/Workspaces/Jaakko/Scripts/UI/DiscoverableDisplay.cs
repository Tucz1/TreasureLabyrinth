using UnityEngine;

public class DiscoverableDisplay : MonoBehaviour
{
    bool show;
    public void Toggle(bool value) 
    {
        show = value; 
    }
    private void Update()
    {
        Vector3 target = show ? Vector3.one : Vector3.zero;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            target,
            Time.deltaTime * 5f
        );
    }
}
