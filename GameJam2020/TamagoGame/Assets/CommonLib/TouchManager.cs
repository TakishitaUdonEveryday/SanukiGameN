using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonSystem
{

	public class TouchData
	{
		public TouchPhase m_phase;
		public Vector2 m_position;
		public int m_fingerId;
		public float m_startTime;		// タッチ開始時のTime.time
	};

	public class TouchManager : MonoBehaviour
	{
		List<TouchData> m_touchDataList = new List<TouchData>();

		private static TouchManager ms_instance = null;

		private bool m_hasNewTouch = false;	// 今フレームで新規タッチはあったか？ 


		/// <summary>
		/// 生成 
		/// </summary>
		/// <returns></returns>
		public static TouchManager	Create()
		{
			if ( ms_instance == null )
			{
				GameObject obj = new GameObject("TouchManager");
				DontDestroyOnLoad(obj);
				ms_instance = obj.AddComponent<TouchManager>();
			}
			return ms_instance;
		}


		public static TouchManager	Instance
		{
			get { return ms_instance; }
		}


		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			// 継続 
			for (int i=0; i < m_touchDataList.Count; )
			{
				var touchData = m_touchDataList[i];
				bool isActive = false;
				for (int j=0; j < Input.touchCount; ++j )
				{
					var touch = Input.GetTouch(j);
					if ( touch.fingerId == touchData.m_fingerId )
					{
						touchData.m_phase = touch.phase;
						touchData.m_position = touch.position;
						isActive = true;
						break;
					}
				}
				// 手が離れた？ 
				if ( !isActive )
				{
					m_touchDataList.RemoveAt(i);
					touchData = null;
				} else
				{
					++i;
				}
			}

			// 新規追加 
			m_hasNewTouch = false;
			for ( int i=0; i < Input.touchCount; ++i )
			{
				var touch = Input.GetTouch(i);
				if ( touch.phase == TouchPhase.Began )
				{
					m_touchDataList.Add(
						new TouchData
						{
							m_phase = TouchPhase.Began,
							m_position = touch.position,
							m_fingerId = touch.fingerId,
							m_startTime = Time.time
						});
					m_hasNewTouch = true;
				}
			}
		}


		/// <summary>
		/// タッチ数を取得 
		/// </summary>
		public int	TouchCount
		{
			get { return m_touchDataList.Count; }
		}

		/// <summary>
		/// 新規タッチはあったか？ 
		/// </summary>
		public bool	HasNewTouch
		{
			get { return m_hasNewTouch; }
		}


		public TouchData	GetTouchData(int id)
		{
			if ( id < m_touchDataList.Count )
			{
				return m_touchDataList[id];
			} else
			{
				return null;
			}
		}

		public List<TouchData>	TouchDataList
		{
			get { return m_touchDataList; }
		}

	}

}

