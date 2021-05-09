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
		private RectTransform m_rectTr = null;

		private float m_stickMoveMax = 0.0f;
		private float m_stickMoveMin = 0.0f;
		private Vector2 m_inputDir = Vector2.zero;		// VirtualPadの入力結果(-1.0 ～ +1.0)

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
			m_rectTr = GetComponent<RectTransform>();

			m_stickMoveMax = m_rectTr.sizeDelta.x * 0.4f;
			m_stickMoveMin = m_rectTr.sizeDelta.x * 0.1f;
		}

		// Update is called once per frame
		void Update()
		{
			if (m_touchPhase == TouchPhase.Begin)
			{
				// 新規タッチ有の場合のみ 
				if (TouchManager.Instance.HasNewTouch)
				{
					var touchList = TouchManager.Instance.TouchDataList;
					foreach (var touchData in touchList)
					{
						if (touchData.m_phase == UnityEngine.TouchPhase.Began)
						{
							// 領域内か？ 
							Vector2 touchPos = ScreenToCanvas(touchData.m_position);
							Vector2 dir = touchPos - m_rectTr.anchoredPosition;
							if ( dir.magnitude < m_rectTr.sizeDelta.x*0.5f )
							{
								m_currTouchData = touchData;
								m_touchPhase = TouchPhase.Playing;
								break;
							}
						}
					}
				}
				// リセット 
				m_inputDir = Vector2.zero;
			}

			// スティック位置更新 
			if (m_touchPhase == TouchPhase.Playing )
			{
				Vector2 touchPos = ScreenToCanvas(m_currTouchData.m_position);
				Vector2 dir = touchPos - m_rectTr.anchoredPosition;
				float length = dir.magnitude;
				if ( length < m_stickMoveMin )
				{
					m_stickTr.anchoredPosition = Vector2.zero;
					// 入力結果 
					m_inputDir = Vector2.zero;
				}
				else
				{
					Vector2	newPos = dir * Mathf.Min(length, m_stickMoveMax) / length;
					m_stickTr.anchoredPosition = newPos;
					// 入力結果 
					m_inputDir = newPos / m_stickMoveMax;
				}

				// タッチ終了 
				if ( m_currTouchData.m_phase == UnityEngine.TouchPhase.Ended )
				{
					m_currTouchData = null;
					m_touchPhase = TouchPhase.Begin;
					m_stickTr.anchoredPosition = Vector2.zero;
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


		private Vector2	ScreenToCanvas(Vector2 screenPos)
		{
			return new Vector2(screenPos.x * m_screenToCanvasScale, screenPos.y * m_screenToCanvasScale);
		}


		/// <summary>
		/// 入力結果 
		/// </summary>
		public Vector2	InputDir
		{
			get { return m_inputDir; }
		}


		/// <summary>
		/// 入力結果を取得 
		/// </summary>
		/// <param name="isActive"></param>
		/// <returns></returns>
		public Vector2	GetInputDir( out bool isActive )
		{
			isActive = (m_touchPhase == TouchPhase.Playing);
			return m_inputDir;
		}


	}


}

