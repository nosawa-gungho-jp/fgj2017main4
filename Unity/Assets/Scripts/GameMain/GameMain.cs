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
	float	m_Timer;
	Text	m_TimerText;
    bool m_goalflag;
    public GameObject m_goal;
    Image m_Sampleresu;
    Text m_ResuText;
	float	m_VoiceTimer;
	bool	m_Initialize;


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
		m_Timer = 0;
		m_TimerText = GameObject.Find("Canvas").transform.Find("Timer").GetComponent<Text>();
        m_goalflag = false;
        m_goal = GameObject.Find("Goal");
        m_Sampleresu = GameObject.Find("Canvas").transform.Find("SampleResult").GetComponent<Image>();
        m_ResuText = GameObject.Find("Canvas").transform.Find("ResultTime").GetComponent<Text>();
        

		SoundManager.instance.LoadSoundSourceFromResource(1, "Sounds/BGM_STAGE");
		SoundManager.instance.LoadSoundSourceFromResource(10, "Sounds/SE_COW1");
		SoundManager.instance.LoadSoundSourceFromResource(11, "Sounds/SE_COW2");
		SoundMixer.PlayBGM(1, true);
		m_VoiceTimer = 1.0f;
		m_Initialize = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!m_Initialize)
		{
			return;
		}
        if (!m_goalflag)
        {
            InputProc();
            CowMoveProc();
            TimerProc();
        }
        goalproc();
    }

	// 入力処理
	void InputProc()
	{
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
            if (m_count <= 7.0f)
            {
                m_count += Time.deltaTime * 5;
                Debug.Log(m_count);
            }
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

	// タイマー表示処理
    void TimerProc()
	{
		if (m_TimerText != null)
		{
			m_Timer += Time.deltaTime;
			m_TimerText.text = string.Format("TIME:{0:D2}:{1:D2}", (int)m_Timer, (int)(m_Timer * 100) % 100);
		}
	}

    //ゴール時の処理
    void goalproc()
    {
        float leftX = m_goal.transform.position.x - 6.0f;
        float rightX = m_goal.transform.position.x + 1.0f;

        if (m_player.transform.position.x >= leftX && m_player.transform.position.x <= leftX + 2.0f)
        {
            m_goalflag = true;
            m_player.transform.position = new Vector3(m_player.transform.position.x + Time.deltaTime, m_playerposi.y, m_playerposi.z);
        }

        if(m_player.transform.position.x <= rightX && m_player.transform.position.x >= rightX - 2.0f)
        {
            m_goalflag = true;
            m_player.transform.position = new Vector3(m_player.transform.position.x - Time.deltaTime, m_playerposi.y, m_playerposi.z);
        }

        if(m_goalflag)
        {
            m_ResuText.text = m_TimerText.text;
            m_Sampleresu.color = new Color(m_Sampleresu.color.r, m_Sampleresu.color.g, m_Sampleresu.color.b, m_Sampleresu.color.a + Time.deltaTime);
            m_ResuText.color = new Color(m_ResuText.color.r, m_ResuText.color.g, m_ResuText.color.b, m_ResuText.color.a + Time.deltaTime);
        }
    }

    void VoiceProc()
	{
		m_VoiceTimer -= Time.deltaTime;
		if (m_VoiceTimer <= 0)
		{
			SoundMixer.PlaySE(10 + (int)(Random.value + 0.5f), false);
			m_VoiceTimer = Random.Range(1.0f, 4.0f) + 3.0f;
		}
	}
}
