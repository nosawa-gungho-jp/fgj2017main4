using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour {

	public void ButtonPush()
	{
		Debug.Log ("test");
		var canvas = GameObject.Find("Canvas").transform;
		canvas.Find("TitleMain").gameObject.SetActive(false);
		canvas.Find("Explain").gameObject.SetActive(true);
		//SceneManager.LoadScene("GameMain");
	}
}
