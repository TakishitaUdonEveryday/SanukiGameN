using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjProduct : MonoBehaviour
{
    private bool m_isShipping = false;  // 出荷済みフラグ 

    private GameMaster.BornData m_bornData = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == GameSetting.LAYER_SHIPPING_AREA)
        {
            if (m_isShipping) return;

            m_isShipping = true;

            // Rigidbody止める 
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                //    rigidbody.Sleep();
                Vector3 velocity = rigidbody.velocity;
                velocity.x *= 0.5f;
                velocity.z *= 0.5f;
                rigidbody.velocity = velocity;


                ObjRunaway runaway = GetComponent<ObjRunaway>();
                if (runaway != null)
                {
                    runaway.enabled = false;
				}

            }

            // 出荷がすんだら再キャッチできなくする 
            ObjCatchable catchable = GetComponent<ObjCatchable>();
            if (catchable != null)
			{
                catchable.Prohibit = true;
            }

            // 出荷成功UI 
            int price = m_bornData.m_price * Random.Range(90, 110) / 100;
            price = price / 100 * 100;
            GameMaster.Instance.CreateShippingSuccessText(m_bornData.m_name, price);
        }
	}


    /// <summary>
    /// データ設定 
    /// </summary>
    /// <param name="bornData"></param>
    public void SetBornData(GameMaster.BornData bornData)
    {
        m_bornData = bornData;
	}

}
