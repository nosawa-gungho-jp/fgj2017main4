using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffScroll : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var pos = transform.position;
        pos.x -= 3 * Time.deltaTime;
        if (pos.x < -30f)
        {
            pos.x += 60f;
        }
        transform.position = pos;

    }
}
