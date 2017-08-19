//#define DEMO_MODE

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ionic.Zlib;
using VSSMFU;

public class GameMain : MonoBehaviour
{
    public GameObject m_player;
    public Camera m_camera;
    public GameObject m_sea;
    public SeaMesh m_seaComp;
    Vector3 m_playerposi;
    Vector3 m_cameraposi;
    float	m_count;
	float	m_gravity;
	float	m_cowSpeed;

	void Start ()
	{
		if (!GlobalObject.InitGlobalObject()) return;
        m_player = GameObject.Find("Player");
        m_playerposi = m_player.transform.position;
        m_sea = GameObject.Find("Sea");
        m_seaComp = m_sea.GetComponent<SeaMesh>();
        m_camera = Camera.main;
        m_cameraposi = m_camera.transform.position;
		m_gravity = 0;
		m_cowSpeed = 10.0f;

		SoundManager.instance.LoadSoundSourceFromResource(1, "Sounds/BGM_STAGE");
		SoundMixer.PlayBGM(1, true);
	}
	
	// Update is called once per frame
	void Update ()
	{
		InputProc();
		CowMoveProc();
    }

	// 入力処理
	void InputProc()
	{
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //var touchPos = Input.mousePosition;
        //    //var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //    //var touchWPos = camera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y));
        //    //m_seaComp.ForcePower(touchWPos, 4.0f + m_count);
        //    //Debug.Log("Pushed" + ((int)touchWPos.x).ToString() + "," + ((int)touchWPos.y).ToString());
        //    m_count += Time.deltaTime * 100;
        //    Debug.Log(m_count);
        //}

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(m_count);
            var touchPos = Input.mousePosition;
            var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            var touchWPos = camera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y));
            m_seaComp.ForcePower(touchWPos, 4.0f + m_count);
            m_count = 0;
            //Debug.Log("Pushed" + ((int)touchWPos.x).ToString() + "," + ((int)touchWPos.y).ToString());
        }
        if(Input.GetMouseButton(0))
        {
            m_count += Time.deltaTime * 5;
            Debug.Log(m_count);
        }
    }

    // 牛移動処理
    void CowMoveProc()
	{
		var playerPosY = m_player.transform.position.y;

		m_playerposi.x += m_cowSpeed * Time.deltaTime;
		m_cowSpeed *= 0.95f;			// スピード減衰率（マジックナンバー）

		var playerFront  = new Vector3(m_playerposi.x - 1.0f, m_playerposi.y, m_playerposi.z);
		var playerBack   = new Vector3(m_playerposi.x + 1.0f, m_playerposi.y, m_playerposi.z);
		var heightFront  = m_seaComp.GetWaveHeight(playerFront);
		var heightBack   = m_seaComp.GetWaveHeight(playerBack);
		m_cowSpeed += (heightFront - heightBack) * 0.5f;
		//Debug.Log("Speed:" + (heightFront - heightBack).ToString());

		var heightCenter = m_seaComp.GetWaveHeight(m_playerposi) - 2.56f;		// 位置調整（マジックナンバー）
		var diff = playerPosY - heightCenter;
		if (diff <= 0.1f)
		{
			playerPosY = heightCenter;
			m_gravity = -m_seaComp.GetWaveVelocity(m_playerposi) * 0.5f;	// 上向きの力補正（マジックナンバー）
		}
		else
		{
			m_gravity += 1.0f * Time.deltaTime;		//自由落下係数（マジックナンバー）
			playerPosY -= m_gravity;
		}
        m_player.transform.position = new Vector3(m_playerposi.x, playerPosY, m_playerposi.z);

		// カメラ位置調整
		if (m_player.transform.position.x >= 0)
        {
            m_camera.transform.position = new Vector3(m_player.transform.position.x, m_cameraposi.y, m_cameraposi.z);
        }
	}
}
