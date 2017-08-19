using UnityEngine;
using System.Collections;

public class GameData
{
	//public static string ServerUrl = "http://pickledev.jpn.ph/DamakueData/test/server.php";
	public static string ServerUrl = "https://pickle.ne.jp/Fujita/TowerAdventure/bin/server.php";

	public static int[]	m_VoiceCategory = new int[]{
		16, 10, 9, 2, 8
	};
	public	static string[]	m_VoiceFile = new string[]{
		"voice_00_00", "voice_00_01", "voice_00_02", "voice_00_03", "voice_00_04",
		"voice_00_05", "voice_00_06", "voice_00_07", "voice_00_08", "voice_00_09",
		"voice_00_10", "voice_00_11", "voice_00_12", "voice_00_13", "voice_00_14",
		"voice_00_15",
		"voice_01_00", "voice_01_01", "voice_01_02", "voice_01_03", "voice_01_04",
		"voice_01_05", "voice_01_06", "voice_01_07", "voice_01_08", "voice_01_09",
		"voice_02_00", "voice_02_01", "voice_02_02", "voice_02_03", "voice_02_04",
		"voice_02_05", "voice_02_06", "voice_02_07", "voice_02_08",
		"voice_03_00", "voice_03_01",
		"voice_04_00", "voice_04_01", "voice_04_02", "voice_04_03", "voice_04_04",
		"voice_04_05", "voice_04_06", "voice_04_07",
	};
	public	static string[]	m_VoiceTitle = new string[]{
		"フジタです", "フジフジ", "フフフ", "ジジジ", "タタタ",
		"タイショウちゃん", "にゃお－ん", "いいんじゃない", "ふつうっすね", "どうなんすか",
		"まあまあまあ", "フジタって揃えて", "ギリジャン", "ありがとう", "からの",
		"マッハフジフジ",
		"もう一回いいすか", "今のは無かった", "なんすかね今のは", "想定外の死ですね", "予想外の展開",
		"うーんもう一歩", "早かったかな", "難しいですね", "なんてね", "即死ですね",
		"こんなもんすか", "余裕ですね", "アイム文化人", "クリアです", "おみごと",
		"フジタの真骨頂", "今のは上手かった", "やりました", "大成功",
		"うぎゃー", "ぎゃああ",
		"ちーん", "I'm SEX Machine", "これがプロの技", "出ました", "ついに来ました",
		"あぶなーい", "どうやってもね", "ありがとう２",
	};
	public  int     m_StageNo;
	public  int     m_Score;
	public  int     m_HiScore;
	public	int		m_PlayerNum;
	public	string	m_UserName;
	public	string	m_UserID;
	public  int     m_FujiCoin;
	public  bool[]  m_VoiceOpen;
	public	float	m_TotalVolume;
	public  int     m_ReturnTitle;
	public  int		m_WearOpen;

	public	void	GameStart(bool firstStage = true)
	{
		if (firstStage)
		{
			m_StageNo = 1;
		}
		m_Score = 0;
		m_PlayerNum = 2;
		m_HiScore = PlayerPrefs.GetInt("HiScore", 0);
		// for Debug ----
		//m_StageNo = 2;
		//m_HiScore = 0;
		// -------
		m_UserName = PlayerPrefs.GetString("UserName", "ななし");
		m_UserID = PlayerPrefs.GetString("UserID", "0");
	}

	public	void	Get_VoiceOpen()
	{
		var voiceFlag = PlayerPrefs.GetString("VoiceFlag", "00000000000000000000000000000000000000000000000000");
		m_VoiceOpen = new bool[voiceFlag.Length];
		for (var i = 0; i < voiceFlag.Length; i++)
		{
			m_VoiceOpen[i] = (voiceFlag[i] == '1');
		}
	}

	public	void	Save_VoiceOpen()
	{
		var voiceFlag = "";
		for (var i = 0; i < m_VoiceOpen.Length; i++)
		{
			voiceFlag += m_VoiceOpen[i]? "1" : "0";
		}
		PlayerPrefs.SetString("VoiceFlag", voiceFlag);
		PlayerPrefs.SetInt("FujiCoin", m_FujiCoin);
	}

	public void Add_FujiCoin(int value)
	{
		m_FujiCoin = PlayerPrefs.GetInt("FujiCoin", 0);
		m_FujiCoin += value;
		m_FujiCoin = (m_FujiCoin < 0) ? 0 : ((m_FujiCoin > 999999) ? 999999 : m_FujiCoin);
		PlayerPrefs.SetInt("FujiCoin", m_FujiCoin);
		PlayerPrefs.Save();
	}

	public void	Get_FujiCoin()
	{
		m_FujiCoin = PlayerPrefs.GetInt("FujiCoin", 0);
	}
	public	void	Save_FujiCoin()
	{
		PlayerPrefs.SetInt("FujiCoin", m_FujiCoin);
	}

	public	int		GetRandomWearNo()
	{
		m_WearOpen = PlayerPrefs.GetInt("WearOpen", 1);
		while (true)
		{
			var no = Random.Range(0, 4);
			if ((m_WearOpen & (1<<no)) != 0)
			{
				return no;
			}
		}
	}

	public	bool		OpenRandomWearNo()
	{
		m_WearOpen = PlayerPrefs.GetInt("WearOpen", 1);
		if (m_WearOpen == (1|2|4|8|16))
		{
			return false;
		}
		while (true)
		{
			var no = Random.Range(0, 4);
			if ((m_WearOpen & (1<<no)) == 0)
			{
				m_WearOpen |= (1<<no);
				PlayerPrefs.SetInt("WearOpen", m_WearOpen);
				PlayerPrefs.Save();
				return true;
			}
		}
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
