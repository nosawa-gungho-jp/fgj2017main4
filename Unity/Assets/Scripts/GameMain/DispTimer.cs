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
		m_NumObject[0].sprite = m_Image[ num         % 10];
		m_NumObject[1].sprite = m_Image[(num /   10) % 10];
		m_NumObject[2].sprite = m_Image[(num /  100) % 10];
		m_NumObject[3].sprite = m_Image[(num / 1000) % 10];
	}

	void Start ()
	{
		m_NumObject = new List<Image>(transform.Find("Num").GetComponentsInChildren<Image>());
		m_NumObject.Sort((x,y)=> {
			return x.name.Length - y.name.Length;
		});
	}
}
