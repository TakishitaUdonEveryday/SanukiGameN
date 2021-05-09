using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePlateUI : MonoBehaviour
{
	[SerializeField] private Text m_rank = null;
	[SerializeField] private Text m_sales = null;
	[SerializeField] private Text m_date = null;
	[SerializeField] private Text m_new = null;

	public void Setup(int rank, GameSaveData.ScoreChip chip, bool isNew)
	{
		m_rank.text = (rank + 1).ToString() + "位";
		m_sales.text = chip.m_score + " 円";
		m_date.text = chip.m_date;

		m_new.enabled = isNew;
	}
}
