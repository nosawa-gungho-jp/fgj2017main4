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
//
// フェーダークラス
class CFadeValue
{
	private	float			m_nNowValue;				//!< 現在の値
	private	float			m_nNowTick;					//!< 現在のタイム
	private	float			m_nTotalTick;				//!< 総タイム
	private	float			m_nStartVal;				//!< 開始値
	private	float			m_nDistVal;					//!< 範囲値

	public	bool	IsFade()
	{
		return (m_nNowTick > 0);
	}
	public	void	ResetTime()
	{
		m_nNowTick = 0;
	}
	public	float	GetNowValue()
	{
		return m_nNowValue;
	}
	public	void	SetNowValue(float nValue)
	{
		m_nNowValue = nValue;
	}
	public	void	SetParam(float nTick, float nStart, float nEnd)
	{
		m_nNowTick   = nTick;
		m_nTotalTick = nTick;
		m_nStartVal  = nEnd;
		m_nDistVal   = nStart - nEnd;
	}

	public	float	GetCalcValue()
	{
		return ((m_nDistVal * m_nNowTick) / m_nTotalTick) + m_nStartVal;
	}
	public	bool	Run(float nTick, ref float nValue)
	{
		if (m_nNowTick <= 0)		return false;
		m_nNowTick -= nTick;
		if (m_nNowTick < 0) {
			m_nNowTick = 0;
		}
		nValue = GetCalcValue();
		return true;
	}
};


public class SoundSourceElement : MonoBehaviour
{
	private	static	int		totalVolumeNum = 3;			//!< ボリューム数

	private	SoundManager	m_SoundManager;
	private	int				m_DataUniqueNo;
	private	int				m_ElementUniqueNo;
	private	int				m_Layer;
	private	AudioSource		m_SoundSource;
	private	CFadeValue[]	m_cVolume;					//!< ボリューム
	private	CFadeValue		m_cPanpot;					//!< パン
	private	CFadeValue		m_cPitch;					//!< ピッチ
	private	bool			m_bFinishRequest;			//!< フェード後終了要求
	private	bool			m_bPauseRequest;			//!< フェード後ポーズ要求
	private	bool			m_bPause;					//!< ポーズ中フラグ
	private	bool			m_bVolumeFade;				//!< ボリュームフェード中フラグ
//	private	System.Action	m_finishAction;

	private	void	Awake()
	{
		m_cVolume = new CFadeValue[totalVolumeNum];
		for (int i = 0; i < totalVolumeNum; i++)
		{
			m_cVolume[i] = new CFadeValue();
			m_cVolume[i].SetNowValue(1);
		}
		m_cPanpot = new CFadeValue();
		m_cPitch = new CFadeValue();

		m_SoundSource = gameObject.AddComponent<AudioSource>();
		m_SoundSource.playOnAwake = false;
	}

	public static int TotalVolumeNum
	{
			get { return totalVolumeNum; }
			set { totalVolumeNum = value; }
	}

	public	SoundManager	SoundManager
	{
		get {	return m_SoundManager;	}
		set {	m_SoundManager = value; }
	}

	public	int	DataUniqueNo
	{
		get {	return m_DataUniqueNo;	}
		set {	m_DataUniqueNo = value; }
	}

	public	int	ElementUniqueNo
	{
		get {	return m_ElementUniqueNo;	}
		set {	m_ElementUniqueNo = value; }
	}

	public	int	Layer
	{
		get {	return m_Layer;	}
		set {	m_Layer = value; }
	}

	public	int	Priority
	{
		get {	return m_SoundSource.priority;	}
		set {	m_SoundSource.priority = value;	}
	}

	public	AudioClip	Clip
	{
		get {	return m_SoundSource.clip;	}
		set {	m_SoundSource.clip = value;	}
	}

	//!	フェード中かどうかチェック
	/**
	 *	フェード中かどうかを取得します。<br>
	 *
	 *	@return	フェード中ならtrue
	 */
	public	bool	IsFade(int channel)
	{
		return m_cVolume[channel].IsFade();
	}

	//! 再生中かどうかチェック
	/**
	 *	再生中かどうかを取得します。<br>
	 *
	 *	@return	再生中ならtrue
	 */
	public	bool	IsPlaying()
	{
		return (m_SoundSource != null && m_SoundSource.isPlaying);
	}

	//!	ポーズ中かどうかチェック
	/**
	 *	ポーズ中かどうかを取得します。<br>
	 *
	 *	@return	ポーズ中ならtrue
	 */
	public bool	IsPause()
	{
		return m_bPause;
	}

	//! ボリュームの取得
	/**
	 *	現在のボリュームを取得します。<br>
	 *
	 *	@param	nChannel	[in] ボリュームチャンネル
	 *
	 *	@return	ボリューム値
	 */
	public	float GetVolume(int nChannel)
	{
		if (0 <= nChannel && nChannel < totalVolumeNum)
			return m_cVolume[nChannel].GetNowValue();
		else
			return 1;
	}

	//!	ボリュームの設定
	/**
	 *	ボリュームの設定を行います。<br>
	 *
	 *	@param	nChannel		[in] ボリュームチャンネル
	 *	@param	nVolume			[in] ボリューム(0 ～ 1)
	 *	@param	nTime			[in] 遷移時間[time](省略した場合は即時反映)
	 *	@param	nStartVolume	[in] スタートボリューム(省略した場合は現在値を参照)
	 */
	public	void	SetVolume(int nChannel, float nVolume, float nTime = 0, float nStartVolume = 65535)
	{
		if (0 > nChannel && nChannel >= totalVolumeNum)
		{
			return;
		}
		Mathf.Clamp(nVolume, 0, 1);
		m_bVolumeFade = false;
		if (nTime > 0) {
			if (nStartVolume < 0 || nStartVolume > 1) {
				nStartVolume = InternalGetVolume(nChannel);
			}
			m_cVolume[nChannel].SetParam(nTime, nStartVolume, nVolume);
			m_bVolumeFade = true;
			nVolume = nStartVolume;
		} else {
			m_cVolume[nChannel].ResetTime();
		}
		m_cVolume[nChannel].SetNowValue(nVolume);

		nVolume = 1;
		for (int i = 0; i < totalVolumeNum; i++) {
			nVolume *= m_cVolume[i].GetNowValue();
		}
		_SetVolume(nVolume);
	}

	//!	パンの取得
	/**
	 *	現在のパンを取得します。<br>
	 *
	 *	@return	パン値
	 */
	public	float GetPan()
	{
		return m_cPanpot.GetNowValue();
	}

	//!	パンの設定
	/**
	 *	パンの設定を行います。<br>
	 *
	 *	@param	nPan		[in] パン
	 *	@param	nTime		[in] 遷移時間(省略した場合は即時反映)
	 *	@param	nStartPan	[in] スタートパン(省略した場合は現在値を参照)
	 */
	public	void SetPan(float nPan, float nTime = 0, float nStartPan = 65535)
	{
		nPan = Mathf.Clamp(nPan, -1, 1);
		if (nTime > 0) {
			if (nStartPan < -1 || nStartPan > 1) {
				nStartPan = m_cPanpot.GetNowValue();
			}
			m_cPanpot.SetParam(nTime, nStartPan, nPan);
			nPan = nStartPan;
		} else {
			m_cPanpot.ResetTime();
		}
		m_cPanpot.SetNowValue(nPan);
		_SetPan(nPan);
	}

	//!	ピッチの取得
	/**
	 *	現在のピッチを取得します。<br>
	 *
	 *	@return	ピッチ値
	 */
	public	float GetPitch()
	{
		return m_cPitch.GetNowValue();
	}

	//!	パンの設定
	/**
	 *	パンの設定を行います。<br>
	 *
	 *	@param	nPitch		[in] ピッチ
	 *	@param	nTime		[in] 遷移時間(省略した場合は即時反映)
	 *	@param	nStartPitch	[in] スタートピッチ(省略した場合は現在値を参照)
	 */
	public	void SetPitch(float nPitch, float nTime = 0, float nStartPitch = -1)
	{
		nPitch = Mathf.Max(0, nPitch);
		if (nTime > 0) {
			if (nStartPitch < 0) {
				nStartPitch = m_cPitch.GetNowValue();
			}
			m_cPitch.SetParam(nTime, nStartPitch, nPitch);
			nPitch = nStartPitch;
		} else {
			m_cPitch.ResetTime();
		}
		m_cPitch.SetNowValue(nPitch);
		_SetPitch(nPitch);
	}

	//!	再生
	/**
	 *	再生を行います。<br>
	 *	nLoopに0を指定すると、1回演奏して終了します。<br>
	 *	nLoopInSampleを0以外に設定すると、ループする際のループ開始ポイントを設定できます。<br>
	 *	単位はサンプルです。<br>
	 *
	 *	@param	nLoop			[in] ループ回数(0以外で無限ループ)
	 *	@param	nVolume			[in] ボリューム
	 *	@param	nFadeInTick		[in] フェードイン時間[second]
	 *	@param	nLoopInSample	[in] ループインサンプル
	 */
	public	void Play(int nLoop = 0, float nVolume = -1, float nFadeInTick = 0, int nLoopInSample = 0)
	{
		Stop();
		if (nVolume >= 0)
		{
			SetVolume(0, nVolume, nFadeInTick);
		}
		m_bVolumeFade = false;
		m_bFinishRequest = false;

		_Play(nLoop != 0);
	}

	//!	停止
	/**
	 *	停止します。<br>
	 *	フェードアウト時間が0でない場合、フェード終了後停止します。<br>
	 *
	 *	@param	fFadeOutTime	[in] フェードアウト時間
	 */
	public	void	Stop(float nFadeOutTick = 0)
	{
		m_bPauseRequest = false;
		if (nFadeOutTick > 0 && m_bPause == false) {
			InternalFade(0, nFadeOutTick);
			m_bFinishRequest = true;
		} else {
			AbortFade();
			m_bPause = false;
			_Stop();
		}
	}

	//!	一時停止
	/**
	 *	一時停止します。<br>
	 *
	 *	@param	bPause		[in] ポーズする場合はtrue、ポーズを解除する場合はfalse
	 *	@param	fFadeTime	[in] フェード時間
	 */
	public	void Pause(bool bPause, float nFadeTime)
	{
		if (bPause == true) {
			// ポーズする
			if (m_bPauseRequest == false && IsPause() == false && IsPlaying()) {
				m_bFinishRequest = false;
				if (nFadeTime > 0) {
					InternalFade(0, nFadeTime);
					m_bPauseRequest = true;
				} else {
					AbortFade();
					_Pause();
					m_bPause = true;
				}
			}
		} else {
			// ポーズ解除する
			if (m_bPauseRequest == true || IsPause() == true) {
				if (IsPause() == true) {
					InternalFade(m_cVolume[0].GetNowValue(), nFadeTime, 0);
					_Pause();
					m_bPause = false;
				} else {
					InternalFade(m_cVolume[0].GetNowValue(), nFadeTime);
				}
				m_bPauseRequest = false;
			}
		}
	}

	private	float InternalGetVolume(int nChannel)
	{
		return (m_cVolume[nChannel].IsFade() == true && m_bVolumeFade == false)? m_cVolume[nChannel].GetCalcValue() : m_cVolume[nChannel].GetNowValue();
	}

	private	void	_SetVolume(float vol)
	{
		if (m_SoundSource != null)
		{
			m_SoundSource.volume = vol;
		}
	}

	private	void	_Play(bool isLoop)
	{
		if (m_SoundSource != null)
		{
			m_SoundSource.loop = isLoop;
			m_SoundSource.Play();
		}
	}

	private	void	_Stop()
	{
		if (m_SoundSource != null)
		{
			m_SoundSource.Stop();
		}
	}

	private	void	_Pause()
	{
		if (m_SoundSource != null)
		{
			if (m_bPause)
			{
				m_SoundSource.Play();
			}
			else
			{
				m_SoundSource.Pause();
			}
		}
	}

	private	void	_SetPan(float pan)
	{
		if (m_SoundSource != null)
		{
			m_SoundSource.panStereo = pan;
		}
	}

	private	void	_SetPitch(float pitch)
	{
		if (m_SoundSource != null)
		{
			m_SoundSource.pitch = pitch;
		}
	}

	//!	フェードの中断
	private	void	AbortFade()
	{
		if (IsFade(0) == true) {
			m_cVolume[0].ResetTime();
			m_bVolumeFade = false;
		}
		m_cPanpot.ResetTime();
		m_cPitch.ResetTime();
	}

	private	void	InternalFade(float nVolume, float nTime, float nStartVolume = 65535)
	{
		float	nSave = m_cVolume[0].GetNowValue();
		SetVolume(0, nVolume, nTime, nStartVolume);
		m_cVolume[0].SetNowValue(nSave);
		m_bVolumeFade = false;
	}

	private	void	LateUpdate()
	{
		float	deltaTime = Time.deltaTime;
		float	nValue = 0;

		// パンポット処理
		if (m_cPanpot.Run(deltaTime, ref nValue) == true) {
			m_cPanpot.SetNowValue(nValue);
			_SetPan(nValue);
		}

		// ピッチ処理
		if (m_cPitch.Run(deltaTime, ref nValue) == true) {
			m_cPitch.SetNowValue(nValue);
			_SetPitch(nValue);
		}

		float	nSubVol = 1.0f;
		bool	bSubChange = false;
		// サブボリュームフェード処理
		for (int i = 1; i < totalVolumeNum; i++) {
			if (m_cVolume[i].Run(deltaTime, ref nValue) == true) {
				nSubVol *= nValue;
				m_cVolume[i].SetNowValue(nValue);
				bSubChange = true;
			} else {
				nSubVol *= m_cVolume[i].GetNowValue();
			}
		}
		// メインボリュームフェード処理
		if (m_cVolume[0].Run(deltaTime, ref nValue) == true) {
			_SetVolume(nValue * nSubVol);
			if (m_bVolumeFade == true) {
				m_cVolume[0].SetNowValue(nValue);
			}
			if (m_cVolume[0].IsFade() == false) {
				// フェード終了
				if (m_bFinishRequest == true) {
					Stop();
				} else if (m_bPauseRequest == true) {
					_Pause();
					m_bPauseRequest = false;
					m_bPause        = true;
				}
				m_bVolumeFade = false;
			}
		} else {
			if (bSubChange != false) {
				_SetVolume(nValue * nSubVol);
			}
		}
	}
}
};
