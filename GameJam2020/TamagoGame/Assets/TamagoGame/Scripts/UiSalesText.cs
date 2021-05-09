using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSalesText : MonoBehaviour
{
    private Text m_text = null;

    // Start is called before the first frame update
    void Awake()
    {
        m_text = GetComponent<Text>();
    }

    public void SetScore(int score)
    {
        m_text.text = "売上\n" + score + " 円";
    }

}
