using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fader : MonoBehaviour
{
	public	MeshRenderer	m_Renderer;
	private	bool			m_IsFade;
	private	Collider2D		m_Collider;
	private float			m_FadeColorR;
	private float			m_FadeColorG;
	private float			m_FadeColorB;
	private	Transform		m_LoadingIcon;
	private	float			m_LoadingTimer;

	private static	Fader	m_Instance;

	public static	Fader instance
	{
		get {	return m_Instance;	}
	}

	void Awake ()
	{
		m_Instance = this;
		m_Renderer.sharedMaterial.color = new Color(m_FadeColorR, m_FadeColorG, m_FadeColorB, 0f);
		m_Collider = transform.Find("Plane").GetComponent<BoxCollider2D>();
		m_LoadingIcon = transform.Find("Loading");
		m_LoadingIcon.gameObject.SetActive(false);
		CollisionEnable(true);
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_LoadingIcon.gameObject.activeSelf)
		{
			m_LoadingTimer += Time.deltaTime;
			if (m_LoadingTimer >= 0.05f)
			{
				m_LoadingIcon.localRotation *= Quaternion.Euler(0, 0, -360 / 12);
				m_LoadingTimer = 0;
			}
		}
	}

	// 入力制限
	public	void	CollisionEnable(bool flag)
	{
		var eventSystem = GameObject.Find("/EventSystem");
		if (eventSystem != null)
		{
			var obj = eventSystem.GetComponent<EventSystem>();
			var selObj = obj.currentSelectedGameObject;
			obj.enabled = !flag;
			obj.SetSelectedGameObject(selObj);
		}
		m_Collider.enabled = flag;
	}

	public	bool	IsCollisionEnable()
	{
		return m_Collider.enabled;
	}

	// フェード中かどうか
	public bool IsFade()
	{
		return m_IsFade;
	}

	// フェードカラー
	public void FadeColor(float r, float g, float b)
	{
		float   alpha = m_Renderer.sharedMaterial.color.a;
		m_FadeColorR = r;
		m_FadeColorG = g;
		m_FadeColorB = b;
		m_Renderer.sharedMaterial.color = new Color(m_FadeColorR, m_FadeColorG, m_FadeColorB, alpha);
	}

	// フェードイン
	public void	FadeIn(float time, System.Action finished)
	{
		Fade(true, time, finished);
	}

	// フェードアウト
	public	void	FadeOut(float time, System.Action finished)
	{
		Fade(false, time, finished);
	}

	private	void	Fade(bool fadeIn, float time, System.Action finished)
	{
		if (m_IsFade)
		{
			return;
		}

		if (time == 0)
		{
			m_Renderer.sharedMaterial.color = new Color(m_FadeColorR, m_FadeColorG, m_FadeColorB, fadeIn? 0 : 1);
		}
		else
		{
			var	table = new Hashtable();
			table.Add("alpha", fadeIn? 0 : 1);
			table.Add("time", time);
			table.Add("oncomplete", "cbFadeComplete");
			table.Add("oncompletetarget", this.gameObject);
			table.Add("oncompleteparams", finished != null? finished : new object());
			iTween.FadeTo(m_Renderer.gameObject, table);
			m_IsFade = true;
		}
	}

	public void cbFadeComplete(object param)
	{
		m_IsFade = false;
		if (param.GetType() == typeof(System.Action))
		{
			((System.Action)param)();
		}
	}

	public void	Loading(bool active)
	{
		m_LoadingIcon.gameObject.SetActive(active);
	}
}
