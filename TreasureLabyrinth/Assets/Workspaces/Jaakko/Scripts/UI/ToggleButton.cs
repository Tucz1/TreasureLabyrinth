using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [Header("UIElements")]
    [SerializeField] private Button m_button;
    [SerializeField] private Toggle m_toggle;
    [Header("Settings")]
    [SerializeField] private bool m_defaultValue; 

    private bool m_value;

    public event Action<bool> OnValueChanged;

    private void Awake()
    {
        m_button.onClick.AddListener(() =>
        {
            SetValue(!m_value);
        });

        SetValue(m_defaultValue);
    }
    private void OnDestroy()
    {
        m_button.onClick.RemoveAllListeners();
    }
    public void SetValue(bool value) 
    {
        m_value = value;
        m_toggle.isOn = value;
        OnValueChanged?.Invoke(m_value);
    }
}
