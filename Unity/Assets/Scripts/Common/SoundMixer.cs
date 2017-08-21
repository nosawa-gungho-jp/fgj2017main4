using UnityEngine;
using System.Collections;
using VSSMFU;

public class SoundMixer
{
	public	const	int	Layer_BGM		= 1;
	public	const	int	Layer_SE		= 2;
	public	const	int	Layer_JINGLE	= 4;

	private	static float		m_TotalBgmVolume;
	private	static float		m_TotalSeVolume;
	private	static bool			m_MuteBgm;
	private	static bool			m_MuteSe;

	public	static void	Initialize(int maxSlot)
	{
		SoundManager.instance.Initialize(maxSlot);
		m_TotalBgmVolume = 1;
		m_TotalSeVolume = 1;
		m_MuteBgm = false;
		m_MuteSe = false;
	}

	public static void PlayBGM(int dataNo, bool loop)
	{
		if (SoundManager.instance.IsPlaying(dataNo))
		{
			SoundManager.instance.Stop(dataNo, Layer_BGM);
		}
		else
		{
			SoundManager.instance.StopByLayer(Layer_BGM, 0.5f);
			SoundManager.instance.Play(dataNo, dataNo, Layer_BGM, 0, loop, 1.0f, 65535);
			SoundManager.instance.SetVolume(dataNo, m_TotalBgmVolume, 0, 1);	// Setting Master Bgm Volume
			SoundManager.instance.SetVolume(dataNo, m_MuteBgm? 0 : 1, 0, 2);	// Setting Bgm Mute
		}
	}

	public static void StopBGM(float fadeTime)
	{
		SoundManager.instance.StopByLayer(Layer_BGM, fadeTime);
	}

	public static void PlaySE(int dataNo, bool exclusive = false, float volume = 1.0f)
	{
		SoundManager.instance.Play(dataNo, dataNo, Layer_SE, 1, false, volume, 0, 1, exclusive);
		SoundManager.instance.SetVolume(dataNo, m_TotalSeVolume, 0, 1);
		SoundManager.instance.SetVolume(dataNo, m_MuteSe? 0 : 1, 0, 2);
	}

	public static void SetTotalBgmVolume(float volume)
	{
		m_TotalBgmVolume = volume;
		SoundManager.instance.SetVolumeByLayer(Layer_BGM | Layer_JINGLE, m_TotalBgmVolume, 0, 1);
	}

	public static float GetTotalBgmVolume()
	{
		return m_TotalBgmVolume;
	}

	public static void SetTotalSeVolume(float volume)
	{
		m_TotalSeVolume = volume;
		SoundManager.instance.SetVolumeByLayer(Layer_SE, m_TotalSeVolume, 0, 1);
	}

	public static float GetTotalSeVolume()
	{
		return m_TotalSeVolume;
	}
}
