using System;
using UnityEngine;
public enum UIInputAction 
{
    Pause,

}
public enum PlayerInputAction 
{
    Pulse,

}
public static class InputEvents 
{
    public static event Action<UIInputAction> OnUIInputAction;
    public static event Action<PlayerInputAction> OnInputAction;  

    public static void UIInputAction(UIInputAction action) 
    {
        OnUIInputAction?.Invoke(action);
    }
    public static void InputAction(PlayerInputAction action) 
    {
        OnInputAction?.Invoke(action);
    }
}

public class InputManager : MonoBehaviour
{
    public static InputManager I {  get; private set; }

    private InputSystem_Actions m_actions;

    public InputSystem_Actions Actions => m_actions;

    public Transform[] artifacts;
    public GameObject pulsePrefab;
    AudioSource myAudio;
    public AudioClip whatToPlay;


    private void Awake()
    {
        if (I != null) 
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(this);

        m_actions = new InputSystem_Actions();
        m_actions.Enable();

        // this is bad and leaks :)
        m_actions.UI.OpenMenu.performed += ctx =>
        {
            InputEvents.UIInputAction(UIInputAction.Pause);
        };
        m_actions.Player.Pulse.performed += ctx =>
        {
            InputEvents.InputAction(PlayerInputAction.Pulse);
        };
        myAudio = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Artifact");

            artifacts = new Transform[gameObjects.Length];
            for (int i = 0; i < gameObjects.Length; i++)
            {
                artifacts[i] = gameObjects[i].transform;
            }

            for (int i = 0; i < artifacts.Length; i++)
            {
                if (pulsePrefab == null) break;

                var pulse = Instantiate(pulsePrefab, artifacts[i]);
                Destroy(pulse, 4);
                Debug.Log(artifacts[i]);
            }
            
            InputEvents.InputAction(PlayerInputAction.Pulse);
            myAudio.PlayOneShot(whatToPlay);

        }
    }



    private void OnDestroy()
    {
        if (I != this) 
        {
            return;
        }
        m_actions.Disable();
        m_actions.Dispose();

        I = null;
    }
}
