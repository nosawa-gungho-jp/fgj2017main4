//#define DEMO_MODE

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ionic.Zlib;

public class GameMain : MonoBehaviour
{
    public GameObject m_player;
    public Camera m_camera;
    Vector3 m_posi;

	void Start ()
	{
		if (!GlobalObject.InitGlobalObject()) return;
        m_player = GameObject.Find("Player");
        m_posi = m_player.transform.position;
        m_camera = Camera.main;
    }
	
	// Update is called once per frame
	void Update ()
	{
        m_player.transform.position = new Vector3(m_posi.x, m_posi.y, m_posi.z);
        m_posi.x += Time.deltaTime;
        //if(Screen.width / 2 < m_player.transform)
        //{
        //    m_camera.transform.position = new Vector3(m_player.transform.position.x, m_camera.transform.position.y, m_camera.transform.position.z);
        //}
	}
}
