//--------------------------------------------
// debug:ゲーム時間を無限にする
//#define     DEBUG_GAME_TIME_INFINITE
//--------------------------------------------



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private static GameMaster ms_instance = null;
        
    const int FIRST_EGG_COUNT = 20;

#if DEBUG_GAME_TIME_INFINITE
    const float GAME_TIME = 60.0f * 1000.0f;
#else
    const float GAME_TIME = 60.0f * 3.0f;
//    const float GAME_TIME = 60.0f * 1.0f;
#endif

    private GameObject m_playerCharacter = null;
    [SerializeField] private Transform m_startPosition = null;
    [SerializeField] private Transform m_eggSpawnArea = null;

    [SerializeField] private GameObject[] m_eggObjPrefab = null;   // 卵のプレハブ 
    [SerializeField] private Transform m_gameObjRoot = null;

    [SerializeField] private GameObject[] m_animalPrefab = null;    // 孵化した後の動物のプレハブ 
    [SerializeField] private GameObject m_shippingScoreUiPrefab = null;
    [SerializeField] private GameObject m_playerPrefab = null;

    [SerializeField] private Canvas m_mainCanvas = null;

    [SerializeField] private GameObject m_eggHatchingText = null;   // 卵の孵化カウントダウンのUI 

    [SerializeField] private GameObject m_titleUiNode = null;
    [SerializeField] private GameObject m_gameUiNode = null;

    [SerializeField] private UiSalesText m_salesText = null;

    [SerializeField] private ResultUI m_resultUI = null;


    [System.Serializable]
    public class BornData
    {
        public string m_name;
        public GameObject m_prefab;
        public float m_percent;
        public int m_price;
	};

    [SerializeField] private BornData[] m_bornData = null;
    private int[] m_percentTable = null;
    const int PERCENT_BASE = 1000;


    public enum GameStep
    {
        None,
        Setup,
        Title,
        Game,
        Result,
	};
    private GameStep m_gameStep = GameStep.None;


    private Rect m_eggSpawnRect;    // 卵の出現範囲 

    public static GameMaster    Instance
	{
        get { return ms_instance; }
	}

    public GameObject   GetPlayer
	{
        get { return m_playerCharacter; }
	}

	private void Awake()
	{
        ms_instance = this;
    }

	// Start is called before the first frame update
	void Start()
    {
        StartCoroutine(CoSetup());
    }

    IEnumerator CoSetup()
	{
        m_gameStep = GameStep.Setup;

        // カメラ設定
        Camera.main.gameObject.AddComponent<GameCamera>();

        // プレイヤー生成
        m_playerCharacter = Instantiate(m_playerPrefab);

        // プレイヤー座標をリセット
        m_playerCharacter.transform.SetPositionAndRotation(
                        m_startPosition.position, m_startPosition.rotation);

        // 
        m_salesText.SetScore(0);

        // SaveData
        if (GameSaveData.Instance == null)
		{
            GameObject objSave = new GameObject("GameSaveData");
            objSave.AddComponent<GameSaveData>();
		}

        // 卵の出現範囲 
        {
            Transform tr = m_eggSpawnArea;
            m_eggSpawnRect.Set(
                        tr.position.x - tr.localScale.x * 0.5f, 
                        tr.position.z - tr.localScale.z * 0.5f,
                        tr.localScale.x, tr.localScale.z);
        }

        // 卵の初期配
        for ( int i=0; i < FIRST_EGG_COUNT; ++i )
        {
            FallNewEgg();
        }

        // パーセントのテーブル
        m_percentTable = new int[m_bornData.Length];
        float percentSum = 0.0f;
        for (int i=0; i < m_bornData.Length; ++i )
        {
            percentSum += m_bornData[i].m_percent;
        }
        int sumInt = 0;
        for (int i=0; i < m_percentTable.Length; ++i)
        {
            sumInt += (int)(m_bornData[i].m_percent * (float)PERCENT_BASE / percentSum);
            m_percentTable[i] = sumInt;
        }
        m_percentTable[m_percentTable.Length - 1] = PERCENT_BASE;



        yield return null;

        // ゲームタイトルへ
        StartCoroutine(CoStepGameTitle());
	}


    private float m_uiTimeScale = 1.0f;


    /// <summary>
    /// ゲームタイトル表示 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoStepGameTitle()
    {
        m_gameStep = GameStep.Title;

        // タイトル表示ON 
        m_titleUiNode.SetActive(true);
        m_gameUiNode.SetActive(false);
        m_resultUI.gameObject.SetActive(false);

        bool isEnd = false;
        m_uiTimeScale = 1.0f;
        while (!isEnd)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                isEnd = true;
            }

            yield return null;
		}

        // 時間早く 
        m_uiTimeScale = 6.0f;
        yield return new WaitForSeconds(1.0f);
        m_uiTimeScale = 1.0f;

        // タイトル表示OFF 
        m_titleUiNode.gameObject.SetActive(false);

        // ゲーム開始
        StartCoroutine(CoStepGamePlay());
    }


    private float m_gameTime = 10.0f;


    public float GameTime
    {
        get { return m_gameTime; }
	}

    private IEnumerator CoStepGamePlay()
    {
        m_gameStep = GameStep.Game;

        // ゲーム用UI表示ON 
        m_gameUiNode.SetActive(true);

        m_gameTime = GAME_TIME;

        while (0 <= m_gameTime)
        {
            m_gameTime -= Time.deltaTime;
            yield return null;
		}

        // ゲーム用UI表示OFF 
        m_gameUiNode.SetActive(false);

        // リザルトへ 
        StartCoroutine(CoStepResult());

    }


    private IEnumerator CoStepResult()
    {
        m_gameStep = GameStep.Result;

        // スコアランキング 
        int rank = GameSaveData.Instance.RegistScore(m_gameSales);

        // UI
        m_resultUI.gameObject.SetActive(true);
        m_resultUI.Setup(GameSaveData.Instance.ScoreList, rank, m_gameSales);

  //      Debug.Log("Rank in?  = " + rank);
  //      var scoreList = GameSaveData.Instance.ScoreList;
  //      for (int i=0; i < scoreList.Count; ++i)
		//{
  //          var scoreChip = scoreList[i];
  //          Debug.Log("[" + i + "] " + scoreChip.m_score + " (" + scoreChip.m_date + ")");
		//}


        // 待ち時間 
        yield return new WaitForSeconds(2.0f);

        // 入力待ち 
        bool isEnd = false;
        m_uiTimeScale = 1.0f;
        while (!isEnd)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                isEnd = true;
            }

            yield return null;
        }

        // 再起動 
        SceneManager.LoadSceneAsync("TamagoGame");
    }


    /// <summary>
    /// UIの時間スケール 
    /// </summary>
    public float    UiTimeScale
    {
        get { return m_uiTimeScale; }
	}



    /// <summary>
    /// ゲームプレイ中 
    /// </summary>
    public bool IsGamePlaying
    {
        get { return m_gameStep == GameStep.Game; }
	}


    /// <summary>
    /// 新しい卵が空から降ってくる 
    /// </summary>
    private void FallNewEgg()
    {
        GameObject prefab = m_eggObjPrefab[Random.Range(0, m_eggObjPrefab.Length)];
        var obj = Instantiate(prefab, m_gameObjRoot);
        obj.transform.position = new Vector3(
            Random.Range(m_eggSpawnRect.x, m_eggSpawnRect.x+m_eggSpawnRect.width),
            m_eggSpawnArea.position.y,
            Random.Range(m_eggSpawnRect.y, m_eggSpawnRect.y + m_eggSpawnRect.height)
        );
    }


    /// <summary>
    /// 卵の孵化カウンターUIを表示 
    /// </summary>
    /// <param name="egg"></param>
    public void CreateEggHatchingText(ObjEgg egg)
    {
        GameObject objText = Instantiate(m_eggHatchingText, m_mainCanvas.transform);
        EggHatchingText hatching = objText.GetComponent<EggHatchingText>();
        if ( hatching != null )
        {
            hatching.Setup(egg);
		}
	}

    private int m_gameSales = 0;

    /// <summary>
    /// 出荷成功テキストを表示 
    /// </summary>
    /// <param name="score"></param>
    public void CreateShippingSuccessText(string name, int score)
    {
        m_gameSales += score;
        m_salesText.SetScore(m_gameSales);

        GameObject objText = Instantiate(m_shippingScoreUiPrefab, m_mainCanvas.transform);
        ShippingSuccess success = objText.GetComponent<ShippingSuccess>();
        if ( success != null )
        {
            success.Setup(name, score);
		}
	}




    /// <summary>
    /// 生まれる！ 
    /// </summary>
    /// <param name="animalType"></param>
    /// <param name="position"></param>
    public void BornAnimal(Vector3 position)
    {
        int index = 0;
        int randomValue = Random.Range(0, PERCENT_BASE);
        for (int i=0; i < m_percentTable.Length; ++i )
        {
            if (randomValue <= m_percentTable[i])
            {
                index = i;
                break;
            }
		}

     //   index = 4;

        BornData data = m_bornData[index];
        var bornObj = Instantiate(data.m_prefab, m_gameObjRoot.transform);
        bornObj.transform.SetPositionAndRotation(position,
                    Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
        ObjProduct product = bornObj.GetComponent<ObjProduct>();
        if (product != null)
        {
            product.SetBornData(data);
        }

    }



    // Update is called once per frame
    //void Update()
    //{

    //}
}
