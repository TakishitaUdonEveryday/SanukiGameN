using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggHatchingText : MonoBehaviour
{
    private Text m_text = null;
    private ObjEgg m_objEgg = null;

    private Vector3 m_lastEggPos = Vector3.zero;    // 卵の座標 
    private Vector2 m_canvasSize = Vector2.zero;
    private RectTransform m_rectTr = null;

    private int m_displayTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
        m_rectTr = GetComponent<RectTransform>();

        // Canvasサイズ 
        Canvas canvas = m_text.canvas;
        RectTransform canvasTr = canvas.GetComponent<RectTransform>();
        m_canvasSize = canvasTr.sizeDelta;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_objEgg != null)
        {
            // 卵に残り時間を問い合わせ 
            float time = 0.0f;
            bool isHatching = false;
            m_objEgg.GetRestHatchingTime(out time, out isHatching);

            // 孵化してないなら消滅 
            if ( !isHatching )
            {
                Destroy(this.gameObject);
                return;
			}
            // 表示時間
            int dTime = (int)time;
            if (dTime != m_displayTime)
            {
                m_displayTime = dTime;
                if (dTime == 0)
                {
                    m_text.text = "生まれたよ!!";
                    StartCoroutine(CoDestroy());
				} else {
                    m_text.text = "孵化中\n" + dTime;
				}
			}

            // 座標 
            m_lastEggPos = m_objEgg.transform.position;
        }

        // 表示座標を更新 
        updateDisplayPosition();
    }


    /// <summary>
    /// 表示座標を更新 
    /// </summary>
    private void updateDisplayPosition()
    {
        var scrPos = Camera.main.WorldToViewportPoint(m_lastEggPos);
        if (scrPos.x < 0.0f || 1.0f < scrPos.x
            || scrPos.y < 0.0f || 1.0f < scrPos.y
            || scrPos.z < 0.0f )
        {
            m_text.enabled = false;
		} else {
            m_text.enabled = true;

            m_rectTr.anchoredPosition = new Vector2(
                m_canvasSize.x*scrPos.x - m_canvasSize.x*0.5f,
                m_canvasSize.y*scrPos.y - m_canvasSize.y*0.5f
            );

        }
	}


    /// <summary>
    /// 少しのディレイののち消滅 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoDestroy()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
	}


    public void Setup(ObjEgg egg)
    {
        m_objEgg = egg;
	}
}
