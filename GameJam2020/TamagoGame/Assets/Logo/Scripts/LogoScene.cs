using CommonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logo
{

	public class LogoScene : MonoBehaviour
	{
		const float WAIT_TIME = 3.0f;

		// Start is called before the first frame update
		IEnumerator Start()
		{
			yield return null;

			// フェードイン 
			ColorFade.Instance.FadeIn(1.0f);
			while (ColorFade.Instance.IsAnimate)
			{
				yield return null;
			}

			// 待ち 
			{
				float time = 0.0f;

				while (time < WAIT_TIME)
				{
					time += Time.deltaTime;
					// 画面タップで抜ける 
					if ( TouchManager.Instance.HasNewTouch )
					{
						break;
					}
					yield return null;
				}
			}

			// フェードアウト 
			ColorFade.Instance.FadeOut(1.0f);
			while (ColorFade.Instance.IsAnimate)
			{
				yield return null;
			}

			// ゲームシーンに遷移 
			SceneManager.LoadSceneAsync("TamagoGame");

		}


		public void OnCallSanukiGameN()
		{
		//	Application.OpenURL("https://sanuki-gamen.jimdofree.com/");
		}

		public void OnCallSaikyoGameJam()
		{
		//	Application.OpenURL("https://www.e-topia-kagawa.jp/lecture/saikyo_game_jam/");
		}

	}

}

