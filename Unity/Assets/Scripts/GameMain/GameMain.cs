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
    Vector3 m_playerposi;
    Vector3 m_cameraposi;

	void Start ()
	{
		if (!GlobalObject.InitGlobalObject()) return;
        m_player = GameObject.Find("Player");
        m_playerposi = m_player.transform.position;
        m_sea = GameObject.Find("sea");
        m_camera = Camera.main;
        m_cameraposi = m_camera.transform.position;

		SoundManager.instance.LoadSoundSourceFromResource(1, "Sounds/BGM_STAGE");
		SoundMixer.PlayBGM(1, true);
	}
	
	// Update is called once per frame
	void Update ()
	{
        //if(m_camera.transform.position.x < 6)
        //{
        //    m_camera.transform.position = new Vector3(6, m_cameraposi.y, m_cameraposi.z);
        //}
        if(m_player.transform.position.x > 5)
        {
            m_camera.transform.position = new Vector3(m_player.transform.position.x, m_cameraposi.y, m_cameraposi.z);
        }
        m_player.transform.position = new Vector3(m_playerposi.x, m_playerposi.y, m_playerposi.z);
        m_playerposi.x += Time.deltaTime;
    }

    void Hit()
    {
        if(m_player.GetComponent<Collider2D>().name == "sea")
        {
           
        }
    }
}
