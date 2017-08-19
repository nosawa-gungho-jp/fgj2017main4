﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMesh : MonoBehaviour
{
	public	float			Friction = 0.5f;
	public	float			Tension = 0.5f;

	private	MeshFilter		m_MeshFilter;
	private	Mesh			m_Mesh;
	private	float			m_Scale;
	private	int				m_Width, m_Height;
	//private	float			m_Value, m_Counter;
	private	float[]			m_HeightValue;
	private	float[]			m_HeightForce;

	// 波の高さの取得(World座標)
	public float GetWaveHeight(Vector3 pos)
	{
		pos -= transform.localPosition;
		pos /= m_Scale;
		var idx = (int)pos.x;

		return (0 <= idx && idx < m_HeightForce.Length)? m_HeightForce[idx] : 0;
	}

	public void ForcePower(Vector3 pos, float power)
	{
		pos -= transform.localPosition;
		pos /= m_Scale;
		var idx = (int)pos.x;
		if (idx != 0)
		{
			m_HeightForce[idx - 1] = -power * 0.5f;
		}
		m_HeightForce[idx] = -power;
		if (idx < m_HeightForce.Length - 1)
		{
			m_HeightForce[idx + 1] = -power * 0.5f;
		}
	}

	private void Start()
	{
		m_MeshFilter = GetComponent<MeshFilter>();

		m_Scale = 0.5f;
		m_Width = 100;
		m_Height = 20;

		m_HeightValue = new float[m_Width + 1];
		m_HeightForce = new float[m_Width + 1];

		var rect = new Rect();
		rect.x = 0;
		rect.y = 0;
		rect.width = m_Width;
		rect.height = m_Height;

		m_Mesh = new Mesh();
		var numOfUvs = (m_Width * m_Height) * 4;
		var numOfTrianfles = (m_Width * m_Height) * 6;

		// 頂点の指定
		SetVertex();

		// UV座標の指定
		var uv = new Vector2[numOfUvs];
		for (var i = 0; i < numOfUvs; i += 4)
		{
			uv[i + 0] = new Vector2(0, 0);
			uv[i + 1] = new Vector2(1, 0);
			uv[i + 2] = new Vector2(1, 1);
			uv[i + 3] = new Vector2(0, 1);
		}
		m_Mesh.uv = uv;

		// 頂点インデックスの指定
		var tri = new int[numOfTrianfles];
		for (int i = 0, idx = 0; i < numOfTrianfles; i += 6, idx += 4)
		{
			tri[i + 5] = idx + 0;
			tri[i + 4] = idx + 1;
			tri[i + 3] = idx + 2;
			tri[i + 2] = idx + 0;
			tri[i + 1] = idx + 2;
			tri[i + 0] = idx + 3;
		}
		m_Mesh.triangles = tri;
 
		m_Mesh.RecalculateNormals();
		m_Mesh.RecalculateBounds();
 
		m_MeshFilter.sharedMesh = m_Mesh;

		//var meshRender = GetComponent<MeshRenderer>();
		//meshRender.material.SetTexture("sea", Resources.Load<Texture2D>("seaTex"));
	}
	
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var	touchPos = Input.mousePosition;
			var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
			var touchWPos = camera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y));
			ForcePower(touchWPos, 4.0f);
			//Debug.Log("Pushed" + ((int)touchWPos.x).ToString() + "," + ((int)touchWPos.y).ToString());
		}
		for (var i = 1; i < m_Width; i++)
		{
			m_HeightForce[i] = (m_HeightValue[i + 1] + m_HeightValue[i - 1] - 2.0f * m_HeightValue[i]) * Tension + Friction * m_HeightForce[i];
		}
		for (var i = 0; i <= m_Width; i++)
		{
/*
			var force = 0.0f;
			if (m_HeightValue[i] < 0)		force =  0.10f;
			else if (m_HeightValue[i] > 0)	force = -0.10f;
			m_HeightForce[i] += force;
			m_HeightForce[i] *= 0.6f;
			if (Mathf.Abs(m_HeightForce[i]) < 0.02f)
			{
				m_HeightForce[i] = 0;
				m_HeightValue[i] = 0;
			}
			else
			{
				m_HeightValue[i] += m_HeightForce[i];
			}
*/
			m_HeightValue[i] += m_HeightForce[i];
		}
		SetVertex();
	}

	private void SetVertex()
	{
		var numOfVertices = (m_Width * m_Height) * 4;
		var vert = new Vector3[numOfVertices];
		for (int i = 0, idx = 0; i < numOfVertices; i += 4, idx++)
		{
			var px1 =  (float)(idx % m_Width) * m_Scale;
			var px2 = ((float)(idx % m_Width) + 1) * m_Scale;
			var py1a = ((float)(idx / m_Width) - m_HeightValue[(idx % m_Width)]) * m_Scale;
			var py1b = ((float)(idx / m_Width) - m_HeightValue[(idx % m_Width) + 1]) * m_Scale;
			var py2a = ((float)(idx / m_Width) + 1 - m_HeightValue[(idx % m_Width)]) * m_Scale;
			var py2b = ((float)(idx / m_Width) + 1 - m_HeightValue[(idx % m_Width) + 1]) * m_Scale;
			vert[i + 0] = new Vector3(px1, py1a, 0);
			vert[i + 1] = new Vector3(px2, py1b, 0);
			vert[i + 2] = new Vector3(px2, py2b, 0);
			vert[i + 3] = new Vector3(px1, py2a, 0);
		}
		m_Mesh.vertices = vert;
	}
}
