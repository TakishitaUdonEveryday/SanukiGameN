using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CommonSystem
{

	public class ColorFade : MonoBehaviour
	{
		const float DEFAULT_FADE_TIME = 0.5f;

		[SerializeField] private Image m_fadeImage = null;

		private static ColorFade ms_instance = null;

		private float m_alpha = 1.0f;
		private bool m_isAnimate = false;
		private Color m_fadeColor = Color.white;

		private void Awake()
		{
			ms_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}

		public static ColorFade Instance
		{
			get { return ms_instance; }
		}

		// Start is called before the first frame update
		void Start()
		{
		}


		/// <summary>
		/// 色を設定 
		/// </summary>
		/// <param name="newColor"></param>
		public void SetColor(Color newColor)
		{
			m_fadeColor = newColor;
			m_fadeImage.color = newColor;
		}


		/// <summary>
		/// フェードアウト 
		/// </summary>
		/// <param name="time"></param>
		public void FadeOut(float time=DEFAULT_FADE_TIME)
		{
			if ( !gameObject.activeSelf )
			{
				gameObject.SetActive(true);
			}

			StartCoroutine(CoFadeAnim(m_alpha, 1.0f, time, false));
		}

		/// <summary>
		/// フェードイン 
		/// </summary>
		/// <param name="time"></param>
		public void FadeIn(float time=DEFAULT_FADE_TIME)
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}

			StartCoroutine(CoFadeAnim(m_alpha, 0.0f, time, true));
		}


		/// <summary>
		/// フェード処理 
		/// </summary>
		/// <param name="startAlpha"></param>
		/// <param name="endAlpha"></param>
		/// <param name="animTime"></param>
		/// <param name="isStop"></param>
		/// <returns></returns>
		private IEnumerator CoFadeAnim(float startAlpha, float endAlpha, float animTime, bool isStop)
		{
			float time = 0.0f;
			m_isAnimate = true;
			while (time < animTime)
			{
				time += Time.deltaTime;
				m_alpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(time / animTime));
				m_fadeColor.a = m_alpha;
				m_fadeImage.color = m_fadeColor;
				yield return null;
			}
			m_isAnimate = false;

			if ( isStop )
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// アニメーション中？ 
		/// </summary>
		public bool	IsAnimate
		{
			get { return m_isAnimate; }
		}


		public static ColorFade Create()
		{
			if ( ms_instance == null )
			{
				var prefab = Resources.Load("SystemFadeCanvas");
				Instantiate(prefab);
			}
			return ms_instance;
		}


	}


}

