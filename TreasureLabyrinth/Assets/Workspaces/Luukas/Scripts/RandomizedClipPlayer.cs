using UnityEngine;

public class PlayRandomizedClip : MonoBehaviour
{
    AudioClip[] clips;
    public AudioSource source;
    public int modifier;
    public AudioClip[] clips0;
    public AudioClip[] clips1;
    public AudioClip[] clips2;
    public AudioClip[] clips3;
    public AudioClip[] clips4;
    public AudioClip[] clips5;



    public void Play()
    {

        if (modifier == 0)
        {
            clips = clips0;
        }

        if (modifier == 1)
        {
            clips = clips1;
        }

        if (modifier == 2)
        {
            clips = clips2;
        }

        if (modifier == 3)
        {
            clips = clips3;
        }

        if (modifier == 4)
        {
            clips = clips4;
        }

        if (modifier == 5)
        {
            clips = clips5;
        }

        if (clips.Length == 0)
        {
            Debug.LogError("Add audio clips in inspector!");
            return;
        }

        var clip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(clip);
    }


}
