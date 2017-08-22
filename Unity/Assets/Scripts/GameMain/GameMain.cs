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
	DispTimer	m_TimerText;
	DispTimer	m_HiScoreText;
	DispTimer	m_SpeedText;
    bool m_goalflag;
    public GameObject m_goal;
    private GameObject m_Fail;
    private GameObject m_HandObj;
    Image m_Sampleresu;
    Text m_ResuText;
	float	m_VoiceTimer;
    public Sprite[] m_Princess;
    Transform m_goalSprite;
	float		m_CameraVib;
	float		m_CameraVibAdd;
	float		m_CowStopCounter;
	int			m_Step;


    public void OnRetryButton()
	{
		SoundManager.instance.StopByLayer(SoundMixer.Layer_SE);
        SceneManager.LoadScene("GameMain");
	}

    public void OnClearButton()
	{
		GameData.instance.m_HiScore = 0;
		GameData.instance.Save();
		m_HiScoreText.SetNum(GameData.instance.m_HiScore);
	}

    void Start ()
	{
		if (!GlobalObject.InitGlobalObject()) return;

		var canvas = GameObject.Find("Canvas").transform;
		m_player = GameObject.Find("Player");
        m_playerposi = m_player.transform.position;
        m_sea = GameObject.Find("Sea");
        m_seaComp = m_sea.GetComponent<SeaMesh>();
        m_camera = Camera.main;
        m_cameraposi = m_camera.transform.position;
		m_gravity = 0;
		m_cowSpeed = 10.0f;
		m_CameraVib = 0;
		m_CameraVibAdd = 0;
		m_Timer = 0;
		m_TimerText = canvas.Find("Timer").GetComponent<DispTimer>();
        m_goalflag = false;
		m_Fail = canvas.Find("FailWin").gameObject;
        m_goalSprite = canvas.Find("GoalWin").transform;
        m_Sampleresu = m_goalSprite.transform.Find("SampleResult").GetComponent<Image>();
        m_ResuText = m_goalSprite.transform.Find("ResultTime").GetComponent<Text>();
        m_goal = GameObject.Find("Goal");
		m_HandObj = GameObject.Find("Hand");

		m_HiScoreText = canvas.Find("HiScore").GetComponent<DispTimer>();
		m_HiScoreText.SetNum(GameData.instance.m_HiScore);

		m_SpeedText = canvas.Find("Speed").GetComponent<DispTimer>();
		m_SpeedText.SetNum(0);
	
        SoundManager.instance.LoadSoundSourceFromResource(1, "Sounds/BGM_STAGE2");
        SoundManager.instance.LoadSoundSourceFromResource(2, "Sounds/BGM_RESULT");
		SoundManager.instance.LoadSoundSourceFromResource(10, "Sounds/SE_COW1");
		SoundManager.instance.LoadSoundSourceFromResource(11, "Sounds/SE_COW2");
		SoundManager.instance.LoadSoundSourceFromResource(12, "Sounds/SE_CLEAR");
		SoundManager.instance.LoadSoundSourceFromResource(13, "Sounds/SE_ocean 2");
		SoundManager.instance.LoadSoundSourceFromResource(14, "Sounds/SE_wave_S");
		SoundMixer.PlayBGM(1, true);
		SoundMixer.PlaySE(13);
		m_VoiceTimer = 1.0f;
		m_Step = 1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (m_Step)
		{
		case 1:
			InputProc();
			CowMoveProc();
			TimerProc();
			VoiceProc();
			goalproc();
			break;

		case 20:
			goalproc();
			break;

		case 30:
			// 沈む
			var playerPos = m_player.transform.position;
			playerPos.y -= m_CowStopCounter * m_CowStopCounter * Time.deltaTime;
			m_player.transform.position = playerPos;
            m_Fail.gameObject.SetActive(true);
			break;
		}
    }

	// 入力処理
	void InputProc()
	{
        if (Input.GetMouseButtonUp(0))
        {
            var touchPos = Input.mousePosition;
            var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            var touchWPos = camera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y));
            m_seaComp.ForcePower(touchWPos, 2.0f + m_count * 10.0f);
            m_count = 0;
			m_HandObj.SetActive(false);
			SoundMixer.PlaySE(14, true, 0.5f);
            //Debug.Log("Pushed" + ((int)touchWPos.x).ToString() + "," + ((int)touchWPos.y).ToString());
        }
        if(Input.GetMouseButton(0))
        {
            if (m_count <= 0.5f)
            {
                m_count += Time.deltaTime;
                Debug.Log(m_count);
            }
        }
    }

    // 牛移動処理
    void CowMoveProc()
	{
		var playerPosY = m_player.transform.position.y;

		m_playerposi.x += m_cowSpeed * Time.deltaTime;
		m_SpeedText.SetNum(Mathf.Abs((int)(m_cowSpeed * 10)));

		var heightCenter = m_seaComp.GetWaveHeight(m_playerposi) - 2.56f;		// 位置調整（マジックナンバー）
		var diff = playerPosY - heightCenter;

		if (diff <= 0.1f)
		{
			var playerFront  = new Vector3(m_playerposi.x - 1.0f, m_playerposi.y, m_playerposi.z);
			var playerBack   = new Vector3(m_playerposi.x + 1.0f, m_playerposi.y, m_playerposi.z);
			var heightFront  = m_seaComp.GetWaveHeight(playerFront);
			var heightBack   = m_seaComp.GetWaveHeight(playerBack);
			m_cowSpeed += (heightFront - heightBack) * 0.5f;
			//Debug.Log("Speed:" + (heightFront - heightBack).ToString());

			playerPosY = heightCenter;
			m_gravity = -m_seaComp.GetWaveVelocity(m_playerposi) * 0.5f;	// 上向きの力補正（マジックナンバー）
			m_cowSpeed *= 0.95f;			// スピード減衰率（マジックナンバー）
			m_cowSpeed = (Mathf.Abs(m_cowSpeed) < 0.01f)? 0 : m_cowSpeed;
		}
		else
		{
			m_gravity += 1.0f * Time.deltaTime;		//自由落下係数（マジックナンバー）
			playerPosY -= m_gravity;
		}
		if (m_playerposi.x < 0)
		{
			m_playerposi.x = 0;
		}
        m_player.transform.position = new Vector3(m_playerposi.x, playerPosY, m_playerposi.z);

		// カメラ位置調整
		if (m_player.transform.position.x >= 0)
        {
			m_CameraVibAdd += 1.5f / 180 * Mathf.PI;
			m_CameraVibAdd %= Mathf.PI * 2;
			m_CameraVib += Mathf.Sin(m_CameraVibAdd) * 0.04f;
            m_camera.transform.position = new Vector3(m_player.transform.position.x + m_CameraVib, m_cameraposi.y, m_cameraposi.z);
        }

		// 停止判定
		if (m_cowSpeed == 0)
		{
			m_CowStopCounter += Time.deltaTime;
			if (m_CowStopCounter >= 3.0f)
			{
				// 沈む
				m_Fail.gameObject.SetActive(true);
				m_Step = 30;
			}
		}
		else
		{
			m_CowStopCounter = 0;
		}
	}

	// タイマー表示処理
    void TimerProc()
	{
		if (m_TimerText != null)
		{
			m_Timer += Time.deltaTime;
			m_TimerText.SetNum((int)(m_Timer * 100));
			//m_TimerText.text = string.Format("TIME:{0:D2}:{1:D2}", (int)m_Timer, (int)(m_Timer * 100) % 100);
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

        if(m_goalflag && m_Step == 1)
        {
			SoundMixer.PlayBGM(2, true);
			SoundMixer.PlaySE(12);
            m_goalSprite.gameObject.SetActive(true);
            if (0.0f <= m_Timer && m_Timer < 10.0f)
            {
                m_goalSprite.transform.Find("Princess").GetComponent<Image>().sprite = m_Princess[1];
            }
            else if (20 <= m_Timer)
            {
                m_goalSprite.transform.Find("Princess").GetComponent<Image>().sprite = m_Princess[0];
            }
			var score = (int)(m_Timer * 100);
			GameData.instance.m_Score = score;
			if (GameData.instance.m_HiScore == 0 || GameData.instance.m_HiScore > score)
			{
				// 記録更新
				GameData.instance.m_HiScore = score;
				GameData.instance.Save();
				m_ResuText.gameObject.SetActive(true);
				m_HiScoreText.SetNum(GameData.instance.m_HiScore);
			}
			m_Step = 20;
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
