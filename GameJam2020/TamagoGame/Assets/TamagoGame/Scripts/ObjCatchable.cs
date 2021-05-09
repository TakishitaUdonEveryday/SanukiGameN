using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjCatchable : MonoBehaviour
{
    const float UNCATCHABLE_TIME = 1.5f;
    
    private bool m_isCatched = false;
    private Rigidbody m_rigidbody = null;

    private CharacterRigidbody m_playerScript = null;
    private Collider m_collider = null;

    private float m_uncatchableTime = 0.0f;     // 捕まえ禁止時間 
    private bool m_prohibit = false;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponentInChildren<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        // 地面に潜る対策
        if (transform.position.y < 0.0f)
        {
            Vector3 newPos = transform.position;
            newPos.y = 0.0f;
            transform.position = newPos;
        }

        // 捕まえられている間はあプレイヤーに座標を指定される
        if (m_isCatched)
        {
            var handlePos = m_playerScript.GetHandledPos();
            m_rigidbody.position = handlePos;
        }

        // 捕まえ禁止時間制御 
        if (0 < m_uncatchableTime)
		{
            m_uncatchableTime -= Time.deltaTime;
        }
    }


    /// <summary>
    /// プレイヤーにつかまった 
    /// </summary>
    /// <param name="playerScript"></param>
    public void SetHandled(CharacterRigidbody playerScript)
    {
        m_isCatched = true;

        // Rigidbody止める
        m_rigidbody.isKinematic = true;
        m_collider.enabled = false;

        // プレイヤー 
        m_playerScript = playerScript;
    }


    /// <summary>
    /// 捕まった 
    /// </summary>
    /// <returns></returns>
    public bool IsCatched()
    {
        return m_isCatched;
	}

    /// <summary>
    /// キャッチ可能？ 
    /// </summary>
    /// <returns></returns>
    public bool IsCatchable()
	{
        return !m_isCatched && (m_uncatchableTime <= 0.0f && !m_prohibit);
	}

    /// <summary>
    /// 禁止する 
    /// </summary>
    public bool Prohibit
	{
        set { m_prohibit = value; }
	}


    public void Throw(Vector3 throwSpeed)
    {
        // Rididbody有効化 
        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;

        // 投擲 
        m_rigidbody.velocity = throwSpeed;

        m_isCatched = false;

        // 再捕まえ禁止時間の設定 
        m_uncatchableTime = UNCATCHABLE_TIME;
    }

}
