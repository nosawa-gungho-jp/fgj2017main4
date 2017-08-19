//#define DEMO_MODE

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ionic.Zlib;

public class GameMain : MonoBehaviour
{
	void Start ()
	{
		if (!GlobalObject.InitGlobalObject()) return;
    }
	
	// Update is called once per frame
	void Update ()
	{
	}
}
