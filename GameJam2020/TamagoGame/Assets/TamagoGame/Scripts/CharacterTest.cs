using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTest : MonoBehaviour
{
    private CharacterController m_charaCon = null;
    private Animator m_animator = null;

    [SerializeField] private float 走るスピード = 5.0f;
    [SerializeField] private float 走る加速度 = 4.0f;
    [SerializeField] private float 旋回角速度 = 90.0f;

    [SerializeField] private float 走るアニメーションスピード = 6.0f;

    readonly int ANIM_HASH_STANDING = Animator.StringToHash("Standing");
    readonly int ANIM_HASH_WALKING = Animator.StringToHash("Walking");
    readonly int ANIM_HASH_RUNNING = Animator.StringToHash("Running");

    const float ANIM_BLEND_TIME = 0.2f;

    private int m_lastAnimHash = 0;

    private float m_moveSpeed = 0.0f;

    private void ChangeAnim(int anim_hash)
	{
        if (m_lastAnimHash != anim_hash )
		{
            m_animator.CrossFade(anim_hash, ANIM_BLEND_TIME);
            Debug.Log("CrossFade : " + anim_hash);
            m_lastAnimHash = anim_hash;
		}
	}
        　

    // Start is called before the first frame update
    void Start()
    {
        m_charaCon = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
		{
            m_moveSpeed = Mathf.Min(m_moveSpeed + 走る加速度 * Time.deltaTime, 走るスピード);
            ChangeAnim(ANIM_HASH_WALKING);
        } else
		{
            ChangeAnim(ANIM_HASH_STANDING);
            m_moveSpeed *= 0.5f;
		}


        if (Input.GetKey(KeyCode.A))
		{
            transform.Rotate(Vector3.up, -旋回角速度 * Time.deltaTime);
		}
        else if (Input.GetKey(KeyCode.D))
		{
            transform.Rotate(Vector3.up, 旋回角速度 * Time.deltaTime);
		}

        // 移動速度に応じてアニメーション
        if (m_moveSpeed < 0.1f)
		{
            ChangeAnim(ANIM_HASH_STANDING);
            m_animator.speed = 1.0f;

        }
		else if (m_moveSpeed < 走るスピード * 0.5f)
		{
			ChangeAnim(ANIM_HASH_WALKING);
			Debug.Log("Walking");
            {
                float k = m_moveSpeed / 走るスピード * 走るアニメーションスピード;
                m_animator.speed = k;
            }
            //} else
            //{
            //          ChangeAnim(ANIM_HASH_RUNNING);
            //      //    Debug.Log("Running");
        }

        // 移動 
        m_charaCon.Move(transform.forward * (m_moveSpeed * Time.deltaTime));
    }
}
