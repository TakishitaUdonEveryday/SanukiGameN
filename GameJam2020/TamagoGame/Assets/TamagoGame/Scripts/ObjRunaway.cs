using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRunaway : MonoBehaviour
{
    [SerializeField] private float 逃げるスピード = 5.0f;

    private Rigidbody m_rigidbody = null;
    private ObjCatchable m_catchable = null;

    private float m_rotationY = 0.0f;
    private float m_rotateTimer = 0.0f;
    private Vector3 m_lastPos = Vector3.zero;

    private float m_sleepTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_catchable = GetComponent<ObjCatchable>();

        // 向きはランダムスタと 
        m_rotationY = Random.Range(0, 360);
        m_lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		if (!m_catchable.IsCatched())
		{
			if (m_sleepTimer <= 0.0f)
			{
				Vector3 speed = m_rigidbody.velocity;
				Vector3 dir = transform.forward * (逃げるスピード);
				speed.x = dir.x;
				speed.z = dir.z;
				m_rigidbody.velocity = speed;

				// 弾に旋回 
				m_rotateTimer -= Time.deltaTime;
				if (m_rotateTimer < 0.0f)
				{
					float dist = (transform.position - m_lastPos).magnitude;
					if (dist < 逃げるスピード * 0.5f)
					{
						m_rotationY += Random.Range(-30.0f, 30.0f);
					}
					else
					{
						m_rotationY += Random.Range(180.0f - 30.0f, 180.0f + 30.0f);
					}

					Quaternion rot = Quaternion.AngleAxis(m_rotationY, Vector3.up);
					m_rigidbody.MoveRotation(rot);

					m_rotateTimer = Random.Range(1.0f, 5.0f);
				}
			} else {
				m_sleepTimer -= Time.deltaTime;
			}
		}
		else
		{
			m_sleepTimer = 3.0f;
		}
    }
}
