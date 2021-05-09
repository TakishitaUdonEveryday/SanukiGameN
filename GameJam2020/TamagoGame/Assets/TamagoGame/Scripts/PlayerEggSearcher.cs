using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerEggSearcher : MonoBehaviour
{
    [SerializeField] private CharacterRigidbody m_playerScript = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    /// <summary>
    /// 卵を見つけた 
    /// </summary>
    /// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		if ( other.gameObject.layer == GameSetting.LAYER_EGG 
            || other.gameObject.layer == GameSetting.LAYER_ANIMAL)
        {
            m_playerScript.FindNewEgg(other.gameObject);

        }
	}

}
