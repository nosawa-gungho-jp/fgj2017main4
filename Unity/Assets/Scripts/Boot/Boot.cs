using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		if (!GlobalObject.InitGlobalObject()) return;

		SoundMixer.Initialize(8);

		GameData.instance.GameStart();

		SceneManager.LoadScene(GlobalObject.m_bootup);

		Input.multiTouchEnabled = true;
    }

    // Update is called once per frame
    void Update ()
	{
	
	}
}
