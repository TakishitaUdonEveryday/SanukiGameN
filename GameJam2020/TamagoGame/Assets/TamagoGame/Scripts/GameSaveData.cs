using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class GameSaveData : MonoBehaviour
{
    [System.Serializable]
    public class ScoreChip
	{
        public int m_score = 0;     // スコア（金額）
        public string m_date;     // 更新日付 
	};

    [System.Serializable]
    public class ScoreSaveData
	{
        public ScoreChip[] m_ranking;
	};


    const int MAX_HIGH_SCORE_HISTORY = 5;   // 記録を残す数 
    const string SAVEDATA_KEY = "HIGH";

    private List<ScoreChip> m_scoreList = new List<ScoreChip>();

    static GameSaveData ms_instance = null;

 

	private void Awake()
	{
        ms_instance = this;
        DontDestroyOnLoad(this.gameObject);
	}

    /// <summary>
    /// 生成 
    /// </summary>
    public static GameSaveData  Instance
	{
        get { return ms_instance; }
	}

	// Start is called before the first frame update
	void Start()
    {   
        // データロード 
        string data = PlayerPrefs.GetString(SAVEDATA_KEY, "");
        if (!string.IsNullOrEmpty(data))
        {
            try
			{
                ScoreSaveData savedata = JsonUtility.FromJson<ScoreSaveData>(data);
                for (int i=0; i < savedata.m_ranking.Length; ++i)
				{
                    m_scoreList.Add(savedata.m_ranking[i]);
                }
            } catch (Exception e)
			{
                Debug.LogError(e.ToString());
			}
        }
    }


    /// <summary>
    /// スコア登録 
    /// </summary>
    /// <param name="score"></param>
    public int RegistScore(int score)
    {
        ScoreChip newScore = new ScoreChip
        {
            m_score = score,
            m_date = DateTime.Now.ToString()
        };
        m_scoreList.Add(newScore);
        m_scoreList.Sort((a,b) =>
        {
            return b.m_score - a.m_score;
        });

        // 多すぎるなら削除 
        if (MAX_HIGH_SCORE_HISTORY < m_scoreList.Count )
		{
            m_scoreList.RemoveAt(MAX_HIGH_SCORE_HISTORY);
        }

        // 何番目に採用されたか
        int rank = 0;
        foreach (var scoreChip in m_scoreList)
		{
            if (scoreChip.m_score == score)
			{
                Save();
                break;
			}
            rank++;
		}
        return rank;
    }

    private void Save()
	{
        var savedata = new ScoreSaveData();
        savedata.m_ranking = m_scoreList.ToArray();

        string saveStr = JsonUtility.ToJson(savedata);
        PlayerPrefs.SetString(SAVEDATA_KEY, saveStr);
    }


    /// <summary>
    /// スコアリストを参照 
    /// </summary>
    public List<ScoreChip> ScoreList
	{
        get { return m_scoreList; }
	}


    //// Update is called once per frame
    //void Update()
    //{

    //}
}
