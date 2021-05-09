using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonSystem
{

	public class Startup
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void InitializeApplication()
		{
			// TouchManager生成 
			TouchManager.Create();
		}
	}

}

