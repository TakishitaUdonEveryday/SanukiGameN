using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Text m_currentScore = null;
    [SerializeField] private RectTransform m_rankingNode = null;
    [SerializeField] private GameObject m_rankChip = null;

    public void Setup(List<GameSaveData.ScoreChip> scoreList, int nowRank, int nowScore)
	{
        // ランキング
        int i = 0;
        foreach (var score in scoreList)
		{
            GameObject objUI = Instantiate(m_rankChip, m_rankingNode);
            ScorePlateUI ui = objUI.GetComponent<ScorePlateUI>();
            if (ui != null)
			{
                ui.Setup(i, score, i == nowRank);
			}
            i++;
		}

        // 今回の記録 
        m_currentScore.text = "今回の売上は " + nowScore + "円でした";

    }
}
