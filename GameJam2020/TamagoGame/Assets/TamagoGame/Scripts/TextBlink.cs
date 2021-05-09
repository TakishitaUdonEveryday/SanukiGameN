using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    [SerializeField] private float m_blinkCycle = 2.0f;
    [SerializeField] private float m_displayTime = 1.2f;

    private float m_time = 0.0f;
    private Graphic m_graphic = null;
    private bool m_isOn = false;

    // Start is called before the first frame update
    void Start()
    {
        m_graphic = GetComponent<Graphic>();
    }

    // Update is called once per frame
    void Update()
    {
        m_time += Time.deltaTime * GameMaster.Instance.UiTimeScale;
        float displayTime = m_time % m_blinkCycle;
        bool isOn = (displayTime < m_displayTime);
        if (isOn != m_isOn)
        {
            m_isOn = isOn;
            m_graphic.enabled = isOn;
        }
    }
}
