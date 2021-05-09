using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GameButton : MonoBehaviour
{
	private Button m_button = null;

	private static GameButton ms_instance = null;

	private void Awake()
	{
		ms_instance = this;
		m_button = GetComponent<Button>();
	}


	public static GameButton Instance
	{
		get { return ms_instance; }
	}

	public Button	button
	{
		get { return m_button; }
	}
}
