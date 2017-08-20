using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		var pos = transform.position;
		pos.x -= 3 * Time.deltaTime;
		if (pos.x < -17.75f)
		{
			pos.x += 17.75f;
		}
		transform.position = pos;
	}
}
