using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CommonSystem
{

	public class VirtualPad : MonoBehaviour
	{
		[SerializeField] private RectTransform m_stickTr = null;

		private static VirtualPad ms_instance = null;

		enum TouchPhase
		{
			Begin,
			Playing,
		};
		private TouchPhase m_touchPhase = TouchPhase.Begin;

		[SerializeField] private Text m_debugText = null;

		private float m_screenToCanvasScale = 1.0f;     // 画面解像度→Canvasへのスケール 
		private Image m_padImage = null;
		private Vector2 m_canvasSize = Vector2.zero;
		private TouchData m_currTouchData = null;

		private void Awake()
		{
			ms_instance = this;
		}

		static public VirtualPad	Instance
		{
			get { return ms_instance; }
		}


		// Start is called before the first frame update
		void Start()
		{
			m_padImage = GetComponent<Image>();
			Canvas canvas = m_padImage.canvas;
			m_canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
			m_screenToCanvasScale = m_canvasSize.x / (float)Screen.width;
		}

		// Update is called once per frame
		void Update()
		{
			if (m_touchPhase == TouchPhase.Begin)
			{
				if (TouchManager.Instance.HasNewTouch)
				{
					var touchList = TouchManager.Instance.TouchDataList;
					foreach (var touchData in touchList)
					{
						if (touchData.m_phase == UnityEngine.TouchPhase.Began)
						{
							m_currTouchData = touchData;
							m_touchPhase = TouchPhase.Playing;
							break;
						}
					}
				}
			}
			else
			{
				if ( m_currTouchData.m_phase == UnityEngine.TouchPhase.Ended )
				{
					m_currTouchData = null;
					m_touchPhase = TouchPhase.Begin;
				}
			}

			if (m_currTouchData != null)
			{
				m_debugText.text = "Touch pos = " + m_currTouchData.m_position + "(" + m_currTouchData.m_phase + ":" + m_currTouchData.m_fingerId + ")";
			} else
			{
				m_debugText.text = "Touch ---";
			}

		}
	}


}

