using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private GameObject m_playerObj = null;
    private Vector3 m_camPos = Vector3.zero;
    //[SerializeField] private float カメラの距離 = 6.0f;
    //[SerializeField] private Vector3 カメラの角度 = new Vector3(30.0f, 0, 0);
    [SerializeField] private Vector3 プレイヤーのどこを見るか = new Vector3(0, 1.0f, 0);

    [SerializeField] private Vector3 カメラの位置 = new Vector3(0, 1.2f, -4);
    [SerializeField] private float カメラの左右の補正の強さ = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_playerObj = GameMaster.Instance.GetPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        //    UpdateCamera1();
        UpdateCamera2();
    }


    private Quaternion m_cameraRot = Quaternion.identity;

    /// <summary>
    /// プレイヤーの後ろにゆっくりついていく
    /// </summary>
    private void UpdateCamera2()
    {
        Vector3 lookatPos = m_playerObj.transform.TransformPoint(プレイヤーのどこを見るか);
        m_cameraRot = Quaternion.Slerp(m_cameraRot, m_playerObj.transform.rotation, Mathf.Clamp01(カメラの左右の補正の強さ * Time.deltaTime));
        Vector3 cameraPos = lookatPos + m_cameraRot * カメラの位置;
        Quaternion cameraRot = Quaternion.LookRotation(lookatPos - cameraPos, Vector3.up);

        float slowY = Mathf.Lerp(transform.position.y, cameraPos.y, 5.0f * Time.deltaTime);
        cameraPos.y = Mathf.Min(slowY, cameraPos.y);

        // カメラ座標を設定 
        transform.SetPositionAndRotation(cameraPos, cameraRot);
    }


    /// <summary>
    /// 視点が近い位置からついていく
    /// </summary>
    //private void UpdateCamera1()
    //{
    //    Vector3 lookatPos = m_playerObj.transform.TransformPoint(プレイヤーのどこを見るか);
    //    Quaternion revLookRot = Quaternion.LookRotation(transform.position - lookatPos, Vector3.up);
    //    Quaternion rotOffset = Quaternion.Euler(カメラの角度);
    //    Quaternion camRotLocal = revLookRot/* * rotOffset*/;
    //    Vector3 localPos = Vector3.forward * カメラの距離;
    //    Vector3 camPos = lookatPos + camRotLocal * localPos;

    //    // カメラ座標を設定 
    //    Quaternion camRot = Quaternion.LookRotation(lookatPos - camPos, Vector3.up);
    //    transform.SetPositionAndRotation(camPos, camRot);
    //}
}
