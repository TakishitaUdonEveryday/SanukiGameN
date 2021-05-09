using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjEgg : MonoBehaviour
{
    private bool m_isOnCulture = false;     // 培養中 

    private float m_hatchingTimer = 0.0f;   // 孵化するまでの残り時間 


    // Start is called before the first frame update
    void Start()
    {
        m_hatchingTimer = Random.Range(10, 30);
    }

    // Update is called once per frame
    void Update()
    {
        // 孵化 
        if ( m_isOnCulture )
        {
            m_hatchingTimer -= Time.deltaTime;
            if (m_hatchingTimer < 0.0f)
            {
                // 生まれる 
                GameMaster.Instance.BornAnimal(transform.position);
                
                // 消滅 
                Destroy(this.gameObject);
			}
        }
    }




	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == GameSetting.LAYER_CULTURE_AREA)
        {
            m_isOnCulture = true;

            // 卵の孵化UIを表示 
            GameMaster.Instance.CreateEggHatchingText(this);
        }
	}

	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.layer == GameSetting.LAYER_CULTURE_AREA)
        {
            m_isOnCulture = false;
        }
    }


    /// <summary>
    /// 孵化するまでの残り時間を取得 
    /// </summary>
    /// <param name="time"></param>
    /// <param name="isHatching"></param>
    public void GetRestHatchingTime(out float time, out bool isHatching)
    {
        time = m_hatchingTimer;
        isHatching = m_isOnCulture;
    }
}
