using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour {

	public void ButtonPush()
	{
		Debug.Log ("test");
		SceneManager.LoadScene("GameMain");
	}
}
