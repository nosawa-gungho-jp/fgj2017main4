using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class FixCamera : MonoBehaviour
{
	public float width = 640f;
	public float height = 1136f;

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		float aspect = (float)Screen.height / (float)Screen.width;
		float bgAcpect = height / width;
		var	cam = GetComponent<Camera>();
		if (bgAcpect > aspect)
		{
			// 倍率
			float bgScale = height / Screen.height;
			// viewport rectの幅
			float camWidth = width / (Screen.width * bgScale);
			// viewportRectを設定
			cam.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);
		}
		else {
			// 倍率
			float bgScale = width / Screen.width;
			// viewport rectの幅
			float camHeight = height / (Screen.height * bgScale);
			// viewportRectを設定
			cam.rect = new Rect(0f, (1f - camHeight) / 2f, 1f, camHeight);
		}
	}
}
