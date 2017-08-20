using UnityEngine;
using System.Collections;

public class GameData
{
	public  int     m_Score;
	public  int     m_HiScore;
	public	string	m_UserName;
	public	string	m_UserID;

	public	void	GameStart(bool firstStage = true)
	{
		m_HiScore = PlayerPrefs.GetInt("HiScore", 0);
		m_UserName = PlayerPrefs.GetString("UserName", "ななし");
		m_UserID = PlayerPrefs.GetString("UserID", "0");
	}

	public	void	Flush()
	{
		PlayerPrefs.Save();
	}

	public	void	Save()
	{
		PlayerPrefs.SetInt("HiScore", m_HiScore);
		PlayerPrefs.SetString("UserName", m_UserName);
		PlayerPrefs.SetString("UserID", m_UserID);
	}

	static  private	GameData    m_Instance;
	static  public	GameData    instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new GameData();
			}
			return m_Instance;
		}
	}
}
