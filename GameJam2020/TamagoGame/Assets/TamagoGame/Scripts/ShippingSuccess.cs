using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShippingSuccess : MonoBehaviour
{
    [SerializeField] private Text m_text = null;
    private float m_lifeTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_lifeTime -= Time.deltaTime;
        if ( m_lifeTime < 0.0f )
        {
            Destroy(this.gameObject);
		}
    }

    public void Setup(string name, int score)
    {
//        m_text.text = "Shipped the " + name + "! " + score + " yen";
        m_text.text = name + " を出荷! " + score + " 円";
    }
}
