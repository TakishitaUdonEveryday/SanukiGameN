using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UiTimeText : MonoBehaviour
{
    private Text m_text = null;
    int m_sec = 0;
    int m_min = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float gameTime = GameMaster.Instance.GameTime;
        int timeMin = (int)(gameTime/60.0f);
        int timeSec = (int)(gameTime % 60.0f);
        if ( m_sec != timeSec || m_min != timeMin )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("残り時間\n{0}分{1}秒", timeMin, timeSec.ToString("00"));
            m_text.text = sb.ToString();
            m_min = timeMin;
            m_sec = timeSec;
        }
    }
}
