//
// Very Simple Sound Manager For Unity (VSSMFU)
//                                    Version 0.0.1  2015/09/21
//
/*
The MIT License (MIT)

Copyright (c) 2015 Takaaki,Hoshiyama (Pickle inc.)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

#region Namespaces
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace VSSMFU
{
public class SoundManager : MonoBehaviour
{
	class SoundDataElement
	{
		public	int				m_DataUniqueNo;
		public	string			m_SoundFile;
		public	AudioClip		m_AudioClip;
	}

	private	static	SoundManager		m_Instance;

	private	bool						m_IsInitialized;
	private	List<SoundDataElement>		m_SoundClip = new List<SoundDataElement>();
	private	List<SoundSourceElement>	m_SoundElement = new List<SoundSourceElement>();

	public static SoundManager instance
	{
		get {	return m_Instance;	}
	}

	void Awake ()
	{
		m_Instance = this;
	}

	//! 初期化
	//		audioSourceNum		最大発音数
	public void Initialize(int audioSourceNum)
	{
		if (m_IsInitialized)
		{
			return;
		}

		m_IsInitialized = true;
		m_SoundElement.Clear();
		for (int i = 0; i < audioSourceNum; i++)
		{
			var	go = new GameObject("SoundSourceElement");
			m_SoundElement.Add(go.AddComponent<SoundSourceElement>());
			go.transform.parent = this.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
		}
		m_SoundClip.Clear();
	}

	//! 解放処理
	public void Release()
	{
		if (!m_IsInitialized)
		{
			return;
		}

		ReleaseSoundSource(-1);
		m_IsInitialized = false;
	}

	//! サウンドーデータの読み込み（リソース）
	//		dataNo		データナンバー
	//		sound		サウンドファイル名
	public bool LoadSoundSourceFromResource(int dataNo, string sound)
	{
		var	clip = Resources.Load<AudioClip>(sound);
		return AttachSoundSource(dataNo, sound, clip);
	}

	//! サウンドーデータの読み込み（アセット）
	//		dataNo		データナンバー
	//		sound		サウンドファイル名
	public bool LoadSoundSourceFromAssetFile(int dataNo, string sound)
	{
		var asset = AssetBundle.LoadFromFile(sound);
		if (asset == null || asset.mainAsset == null)
		{
			return false;
		}
		return AttachSoundSource(dataNo, sound, (AudioClip)asset.mainAsset);
	}

	//! サウンドデータのアタッチ
	//		dataNo		データナンバー
	//		soundName	サウンドファイル名
	//		clip		オーディオクリップ
	public bool AttachSoundSource(int dataNo, string soundName, AudioClip clip)
	{
		if (!m_IsInitialized || dataNo <= 0 || clip == null)
		{
			return false;
		}
		var data = GetSoundData(dataNo);
		if (data != null && data.m_SoundFile == soundName)
		{
			return true;
		}
		ReleaseSoundSource(dataNo);

		var	soundData = new SoundDataElement();
		soundData.m_DataUniqueNo = dataNo;
		soundData.m_SoundFile = soundName;
		soundData.m_AudioClip = clip;
		m_SoundClip.Add(soundData);

		return true;
	}

	//! サウンドデータの解放
	//		dataNo		データナンバー
	public void ReleaseSoundSource(int dataNo = 0)
	{
		if (!m_IsInitialized)
		{
			return;
		}
		if (dataNo <= 0)
		{
			StopAll();
			m_SoundClip.Clear();
		}
		else
		{
			Stop(dataNo);
			var	soundData = GetSoundData(dataNo);
			if (soundData != null)
			{
				m_SoundClip.Remove(soundData);
			}
		}
	}

	//! サウンドエレメントの追加
	//		elem		SoundSourceElementインスタンス
	public void AddSoundElement(SoundSourceElement elem)
	{
		if (!m_IsInitialized || elem == null)
		{
			return;
		}
		elem.SoundManager = this;
		m_SoundElement.Add(elem);
	}

	//! 再生中かどうか
	//		unique		識別ID
	public bool IsPlaying(int unique)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			if (se.IsPlaying())
			{
				return true;
			}
		}
		return false;
	}

	//! ポーズ中かどうか
	//		unique		識別ID
	public bool IsPause(int unique)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			if (se.IsPause())
			{
				return true;
			}
		}
		return false;
	}

	//! 再生
	public int Play(int dataNo, int unique = 0, int layer = 0, int priority = 255, bool isLoop = false, float volume = -1, float pan = 65535, float pitch = 0, bool exclusive = false)
	{
		var	soundData = GetSoundData(dataNo);
		if (soundData == null)
		{
			return -1;
		}
		var	elem = GetEmptySoundElement(exclusive, unique, priority);
		if (elem == null)
		{
			return -1;
		}

		elem.SoundManager = this;
		elem.ElementUniqueNo = unique;
		elem.DataUniqueNo = dataNo;
		elem.Layer = layer;
		elem.Priority = priority;
		elem.Clip = soundData.m_AudioClip;
		elem.Play(isLoop? 1 : 0, volume, 0);
		if (pan != 65535)
		{
			elem.SetPan(pan);
		}
		if (pitch > 0)
		{
			elem.SetPitch(pitch);
		}

		return 0;
	}

	//! 停止
	//		unique			識別ID
	//		nFadeoutTime	フェード時間（秒）
	public void Stop(int unique, float nFadeoutTime = 0)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			se.Stop(nFadeoutTime);
		}
	}

	//! 停止（レイヤー指定）
	//		layer			レイヤービット
	//		nFadeoutTime	フェード時間（秒）
	public void StopByLayer(int layer, float nFadeoutTime = 0)
	{
		var	lists = FindSoundElementByLayer(layer);
		foreach (var list in lists) {
			list.Stop(nFadeoutTime);
		}
	}

	//! 全停止
	//		nFadeoutTime	フェード時間（秒）
	public void StopAll(float nFadeoutTime = 0)
	{
		foreach (var elem in m_SoundElement)
		{
			elem.Stop(nFadeoutTime);
		}
	}

	//! ポーズ
	//		unique			識別ID
	//		bPause			ポーズフラグ
	//		nFadeoutTime	フェード時間（秒）
	public void Pause(int unique, bool bPause, float nFadeTime = 0)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			se.Pause(bPause, nFadeTime);
		}
	}

	//! ポーズ
	//		layer			レイヤービット
	//		bPause			ポーズフラグ
	//		nFadeoutTime	フェード時間（秒）
	public void PauseByLayer(int layer, bool bPause, float nFadeTime = 0)
	{
		var	soundElement = FindSoundElementByLayer(layer);
		foreach (var se in soundElement)
		{
			se.Pause(bPause, nFadeTime);
		}
	}

	//! ボリュームの取得
	//		unique		識別ID
	//		channel		ボリュームチャンネル
	public float GetVolume(int unique, int channel = 0)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		return (soundElement.Count != 0)? soundElement[0].GetVolume(channel) : 0;
	}

	//! ボリュームの設定
	//		unique		識別ID
	//		volume		ボリューム
	//		fadeTime	遷移時間（秒）
	//		channel		ボリュームチャンネル
	public void SetVolume(int unique, float volume, float fadeTime = 0, int channel = 0)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			se.SetVolume(channel, volume, fadeTime);
		}
	}

	//! ボリュームの設定
	//		layer		レイヤービット
	//		volume		ボリューム
	//		fadeTime	遷移時間（秒）
	//		channel		ボリュームチャンネル
	public void SetVolumeByLayer(int layer, float volume, float fadeTime = 0, int channel = 0)
	{
		var	lists = FindSoundElementByLayer(layer);
		foreach (var list in lists)
		{
			list.SetVolume(channel, volume, fadeTime);
		}
	}

	//! パンの取得
	//		unique		識別ID
	public float GetPanpot(int unique)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		return (soundElement.Count != 0)? soundElement[0].GetPan() : 0;
	}

	//! パンの設定
	//		unique		識別ID
	//		pan			パン
	//		changeTime	遷移時間（秒）
	//		startPan	初期値
	public void SetPanpot(int unique, float pan, float changeTime = 0, float startPan = 65535)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			se.SetPan(pan, changeTime, startPan);
		}
	}

	//! ピッチの取得
	//		unique		識別ID
	public float GetPitch(int unique)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		return (soundElement.Count != 0)? soundElement[0].GetPitch() : 0;
	}

	//! ピッチの設定
	//		unique		識別ID
	//		pitch		ピッチ
	//		changeTime	遷移時間（秒）
	//		startPitch	初期値
	public void SetPitch(int unique, float pitch, float changeTime = 0, float startPitch = -1)
	{
		var	soundElement = FindSoundElementByElementUnique(unique);
		foreach (var se in soundElement)
		{
			se.SetPitch(pitch, changeTime, startPitch);
		}
	}


	private SoundSourceElement GetEmptySoundElement(bool sameUnique, int unique, int priority)
	{
		SoundSourceElement	empty = null;
		SoundSourceElement	lowPriority = null;
		for (int i = 0; i < m_SoundElement.Count; i++)
		{
			if (sameUnique && m_SoundElement[i].DataUniqueNo == unique)
			{
				return m_SoundElement[i];
			}
			if (empty == null && !m_SoundElement[i].IsPlaying() && !m_SoundElement[i].IsPause())
			{
				empty = m_SoundElement[i];
			}
			if (lowPriority == null || lowPriority.Priority < m_SoundElement[i].Priority)
			{
				lowPriority = m_SoundElement[i];
			}
		}

		return (empty == null)? lowPriority : empty;
	}

	private List<SoundSourceElement> FindSoundElementByElementUnique(int unique)
	{
		var	list = new List<SoundSourceElement>();
		foreach (var data in m_SoundElement)
		{
			if (data.ElementUniqueNo == unique)
			{
				list.Add(data);
			}
		}
		return list;
	}

	private List<SoundSourceElement> FindSoundElementByDataUnique(int unique)
	{
		var	list = new List<SoundSourceElement>();
		foreach (var data in m_SoundElement)
		{
			if (data.DataUniqueNo == unique)
			{
				list.Add(data);
			}
		}
		return list;
	}

	private	List<SoundSourceElement>	FindSoundElementByLayer(int layer)
	{
		var	list = new List<SoundSourceElement>();
		foreach (var data in m_SoundElement)
		{
			if ((data.Layer & layer) != 0)
			{
				list.Add(data);
			}
		}
		return list;
	}

	private	SoundDataElement	GetSoundData(string sound)
	{
		foreach (var data in m_SoundClip)
		{
			if (data.Equals(sound))
			{
				return data;
			}
		}
		return null;
	}

	private SoundDataElement GetSoundData(int unique)
	{
		foreach (var data in m_SoundClip)
		{
			if (data.m_DataUniqueNo == unique)
			{
				return data;
			}
		}
		return null;
	}
}
};
