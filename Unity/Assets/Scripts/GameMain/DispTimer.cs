using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DispTimer : MonoBehaviour
{
	public	Sprite[]		m_Image;

	private	List<Image>		m_NumObject;
	private	int				m_Number;

	public void SetNum(int num)
	{
		m_Number = num;
		foreach (var obj in m_NumObject)
		{
			obj.sprite = m_Image[num % 10];
			num /= 10;
		}
	}

	void Awake()
	{
		m_NumObject = new List<Image>(transform.Find("Num").GetComponentsInChildren<Image>());
		m_NumObject.Sort((x,y)=> {
			return x.name.Length - y.name.Length;
		});
	}
}
