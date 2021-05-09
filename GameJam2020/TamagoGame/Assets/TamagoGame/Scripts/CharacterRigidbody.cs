using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRigidbody : MonoBehaviour
{
    private Rigidbody m_rigidbody = null;
    private Animator m_animator = null;

    [SerializeField] private float 走るスピード = 5.0f;
    [SerializeField] private float 後退するスピード = 2.0f;
    [SerializeField] private float 走る加速度 = 4.0f;
    [SerializeField] private float 旋回角速度 = 90.0f;
    [SerializeField] private float ジャンプ力 = 5.0f;

    [SerializeField] private float 走るアニメーションスピード = 6.0f;

    [SerializeField] private float 卵を投げる速度と自分の速度の関係 = 2.5f;

    readonly int ANIM_HASH_STANDING = Animator.StringToHash("Standing");
    readonly int ANIM_HASH_WALKING = Animator.StringToHash("Walking");
    readonly int ANIM_HASH_RUNNING = Animator.StringToHash("Running");

    const float ANIM_BLEND_TIME = 0.2f;

    private int m_lastAnimHash = 0;

    private float m_moveSpeed = 0.0f;
    private bool m_isOnGround = false;
    private bool m_isJumping = false;

    private float m_eggLocalRot = 0.0f;     // 卵を持つローカルの角度
    private Vector3 m_eggHandlePosition = Vector3.zero; // 卵を持つべき場所 

    private ObjCatchable m_handlingEgg = null;    // 捕まえている卵 

    private float m_jumpingTime = 0.0f;


    private void ChangeAnim(int anim_hash)
	{
        if (m_lastAnimHash != anim_hash )
		{
            m_animator.CrossFade(anim_hash, ANIM_BLEND_TIME);
        //    Debug.Log("CrossFade : " + anim_hash);
            m_lastAnimHash = anim_hash;
		}
	}
        　

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMaster.Instance.IsGamePlaying)
        {
            UpdateInGame();
        } else {

		}



    //    Debug.Log("COLL = " + m_collisionCount);

    //    Debug.Log("OnGround = " + m_isOnGround);

    }

    private void UpdateInGame()
    {
        // 前進・移動速度制御
        if (Input.GetKey(KeyCode.W))
        {
            // 前進入力
            if (m_moveSpeed < 0)
            {
                m_moveSpeed *= 0.2f;
            }
            m_moveSpeed = Mathf.Min(m_moveSpeed + 走る加速度 * Time.deltaTime, 走るスピード);
            //    ChangeAnim(ANIM_HASH_WALKING);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // 後退 
            if (0 < m_moveSpeed)
            {
                m_moveSpeed *= 0.2f;
            }
            m_moveSpeed = Mathf.Max(m_moveSpeed - 走る加速度 * Time.deltaTime, -後退するスピード);
        }
        else
        {
            // 入力なし
            //    ChangeAnim(ANIM_HASH_STANDING);
            if (m_isOnGround)
            {
                m_moveSpeed *= 0.5f;
            }
        }

        // 左右に旋回
        bool isRot = false;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -旋回角速度 * Time.deltaTime);
            isRot = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, 旋回角速度 * Time.deltaTime);
            isRot = true;
        }

        // ジャンプ 
        if (m_isOnGround && !m_isJumping)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpSpeed = transform.forward * m_moveSpeed;
                jumpSpeed.y = ジャンプ力;
                m_rigidbody.velocity = jumpSpeed;
                //    m_isOnGround = false;
                m_isJumping = true;
                m_jumpingTime = 0.0f;
            }
            else
            {
                m_rigidbody.velocity = m_rigidbody.velocity * 0.5f;
            }
        }
        else
        {
            // 卵を所持中にジャンプボタンを押すと投げる 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (m_handlingEgg != null)
                {
                    float speed = m_moveSpeed * 卵を投げる速度と自分の速度の関係;
                    speed = Mathf.Max(4.0f, speed);

                    // 投擲速度 
                    Vector3 throwSpeed =
                        transform.TransformVector(Vector3.forward * speed);
                    throwSpeed.y = Mathf.Abs(m_moveSpeed) * (卵を投げる速度と自分の速度の関係 * 0.2f);
                    m_handlingEgg.Throw(throwSpeed);
                    m_handlingEgg = null;

                }
            }
        }

        // 移動速度に応じてアニメーション
        float abs_speed = Mathf.Abs(m_moveSpeed);
        if (abs_speed < 0.1f)
        {
            if (isRot)
            {
                ChangeAnim(ANIM_HASH_WALKING);
            }
            else
            {
                ChangeAnim(ANIM_HASH_STANDING);
            }
            m_animator.speed = 1.0f;
        }
        else if (abs_speed < 走るスピード * 0.5f)
        {
            ChangeAnim(ANIM_HASH_WALKING);
            //	Debug.Log("Walking");
            {
                float k = abs_speed / 走るスピード * 走るアニメーションスピード;
                m_animator.speed = k;
            }
            //} else
            //{
            //          ChangeAnim(ANIM_HASH_RUNNING);
            //      //    Debug.Log("Running");
        }

        // ジャンプ中タイマー 
        if (m_isJumping)
        {
            m_jumpingTime += Time.deltaTime;
            if (3.0f < m_jumpingTime && 0 < m_collisionCount && m_isOnGround )
			{
                m_isJumping = false;
            }
        }

        // 接地判定
        //   CheckOnGround();
        // 移動 
        if (m_isOnGround)
        {
            Vector3 nowPos = m_rigidbody.position;
            nowPos += transform.forward * (m_moveSpeed * Time.deltaTime);
			if (nowPos.y < 0.0f)
			{
				nowPos.y = 0.0f;
				//m_rigidbody.MovePosition(nowPos);
			}
			//Vector3 vel = transform.forward * m_moveSpeed;
			//vel.y = m_rigidbody.velocity.y;
			//m_rigidbody.velocity = vel;
			m_rigidbody.MovePosition(nowPos);
        }

        // 卵を持つハンドル位置の更新 
        UpdateEggHandle();

    }


    private Vector3 m_eggHandleLocalPos = new Vector3(0, 0, 0.8f);
    private Vector3 m_eggHandleBodyLocal = new Vector3(0, 0.6f, 0);

    /// <summary>
    /// 卵を持つハンドル位置の更新 
    /// </summary>
    private void UpdateEggHandle()
    {
        const float HANDLE_ROT_SPD = 450.0f;

        float rotSpeed = (m_isOnGround) ? HANDLE_ROT_SPD : -HANDLE_ROT_SPD;
        m_eggLocalRot = Mathf.Clamp(m_eggLocalRot + rotSpeed * Time.deltaTime, -120.0f, -20.0f);
        Quaternion localRot = Quaternion.AngleAxis(m_eggLocalRot, Vector3.right);
        Vector3 localPos = localRot * m_eggHandleLocalPos;

        m_eggHandlePosition = transform.TransformPoint(localPos) + m_eggHandleBodyLocal;

    //    Debug.Log("POS=" + transform.position + " / Handle=" + m_eggHandlePosition + " / angle=" + m_eggLocalRot);
    }


    //   private void CheckOnGround()
    //{
    //       if (!m_isOnGround)
    //	{
    //           // 落下中
    //           if (m_rigidbody.velocity.y < 0.0f)
    //		{

    //		}
    //	}
    //   //    Debug.Log("vel=" + m_rigidbody.velocity.y.ToString("0.000"));
    //}


    private int m_collisionCount = 0;

    private void OnCollisionEnter(Collision collision)
	{
        m_isOnGround = true;
        //if (!m_isOnGround)
        //{

        //}
        m_collisionCount++;
        if (m_collisionCount <= 0) {
            m_collisionCount = 1;
		}

        if (1.0f < m_jumpingTime)
        {
            m_isJumping = false;
		} else
		{
            StartCoroutine(CoCheckJumping());
		}

    }

    private IEnumerator CoCheckJumping()
	{
        while (m_jumpingTime < 1.0f)
		{
            if (m_collisionCount <= 0) yield break;
            yield return null;
		}
        m_isJumping = false;

    }


    private void OnCollisionExit(Collision collision)
	{
        m_collisionCount--;
        if ( m_collisionCount <= 0 )
        {
            m_isOnGround = false;
        }

    }


	/// <summary>
	/// 卵を見つけたお知らせ 
	/// </summary>
	/// <param name="objEgg"></param>
	public void FindNewEgg(GameObject objEgg)
    {
        Debug.Log("卵を見つけた : " + objEgg.name);

        if (m_handlingEgg==null)
        {
            // ObjCatchableコンポーネント探し 
            ObjCatchable newEgg = objEgg.GetComponentInParent<ObjCatchable>();
            if (newEgg == null)
			{
                newEgg = objEgg.GetComponent<ObjCatchable>();
			}
            // キャッチ可能か問い合わせ 
            if (newEgg != null)
			{
                if (newEgg.IsCatchable())
				{
                    newEgg.SetHandled(this);
                    m_handlingEgg = newEgg;
                }
			}
        }
	}

    public Vector3 GetHandledPos()
    {
        return m_eggHandlePosition;
    }
}
