using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VSSMFU;

public class NextTitle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ButtonPush()
    {
		SoundMixer.StopBGM(1.0f);
		SoundManager.instance.StopByLayer(SoundMixer.Layer_SE);
        SceneManager.LoadScene("Title");
    }
}
