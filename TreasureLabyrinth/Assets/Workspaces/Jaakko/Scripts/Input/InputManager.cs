using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum UIInputAction
{
    Pause,
    NavUp,
    NavDown,
    NavLeft,
    NavRight
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
    public static InputManager I { get; private set; }

    private InputSystem_Actions m_actions;

    public InputSystem_Actions Actions => m_actions;

    public Transform[] artifacts;
    public EnemyAI enemy;
    public GameObject pulsePrefab;
    AudioSource myAudio;
    public AudioClip whatToPlay;
    [Range(0f, 1f)][SerializeField] float pingVolume;


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
        m_actions.UI.NavigateUP.performed += ctx =>
        {
            InputEvents.UIInputAction(UIInputAction.NavUp);
        };
        m_actions.UI.NavigateDown.performed += ctx =>
        {
            InputEvents.UIInputAction(UIInputAction.NavDown);
        };
        m_actions.UI.NavigateRight.performed += ctx =>
        {
            InputEvents.UIInputAction(UIInputAction.NavRight);
        };
        m_actions.UI.NavigateLeft.performed += ctx =>
        {
            InputEvents.UIInputAction(UIInputAction.NavLeft);
        };
        myAudio = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += SceneLoaded;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
        {
            myAudio.volume = 0;
        }
        else
        {
            myAudio.volume = pingVolume;
        }
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

            Debug.Log(EnemyAI.AllEnemies.Count);
            foreach (var enemy in EnemyAI.AllEnemies) {
                enemy.playerPinged();
            }
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
