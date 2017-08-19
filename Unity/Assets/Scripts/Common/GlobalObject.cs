using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class GlobalObject : MonoBehaviour
{
	public	static	string			m_bootup = "GameMain";
	public	static	string			m_OpenDialogName = "";
	public	static	bool			show = true;

	private static	string			uuid;
	private	static	GlobalObject	m_Instance;

	public	string				PushTokenString { get; set; }
	public	string[]			CommandLineArgs { get; set; }
	public	bool				showFPS = false;
	public	bool				showInEditor = false;
	public	System.Action		BackKeyAction = null;
	private	bool				BackKeyDown;

	private float		lastCollect = 0;
	private float		lastCollectNum = 0;
	private float		delta = 0;
	private float		lastDeltaTime = 0;
	private int			allocRate = 0;
	private int			lastAllocMemory = 0;
	private float		lastAllocSet = -9999;
	private int			allocMem = 0;
	private int			collectAlloc = 0;
	private int			peakAlloc = 0;

	private	void	Start()
	{
		//Debug.Log("?? GlobalObject Start");
		name = "GlobalObject";
		
		// コマンドライン引数
		CommandLineArgs    = new string[1];
		CommandLineArgs[0] = "";
        //		CommandLineArgs = System.Environment.GetCommandLineArgs();

#if _ENV_PRODUCTION
		var	er = GetComponent<ExceptionReporter>();
		er.OnShowDialog = OnLogCallback;
#endif

#if !UNITY_EDITOR
		PushTokenString = PlayerPrefs.GetString("PushToken_Prefs", "");
#if ENABLE_PUSH && UNITY_IPHONE
		if (NotificationServices.remoteNotificationCount != 0) {
			var	notification = NotificationServices.remoteNotifications[0];
			var	tmp = "";
			foreach (var key in notification.userInfo.Keys) {
				if (tmp != "") {
					tmp += ",";
				}
				tmp += key.ToString() + "," + notification.userInfo[key].ToString();
			}
			CommandLineArgs[0] = tmp;
			Debug.Log("iOS Launch Arguments:" + tmp);
		} else {
			CommandLineArgs[0] = "";
			Debug.Log("iOS Launch Arguments:none");
		}
		
		if (string.IsNullOrEmpty(PushTokenString)) {
			NotificationServices.RegisterForRemoteNotificationTypes(RemoteNotificationType.Alert |  RemoteNotificationType.Sound);
		}
#endif
#if ENABLE_PUSH && UNITY_ANDROID
		var	plugin = new AndroidJavaObject("net.gree.unitywebview");

		CommandLineArgs[0] = plugin.CallStatic<string>("getExtras");
		Debug.Log("getExtras:" + CommandLineArgs[0]);

		if (string.IsNullOrEmpty(PushTokenString)) {
#if _ENV_PRODUCTION
			// 本番用
			plugin.CallStatic("gcm_Register", "0");
#else
			plugin.CallStatic("gcm_Register", "0");
#endif
		}
#endif
#else
        PushTokenString = "";
#endif

		CreateUUID();

		transform.localPosition = new Vector3(10, 0, 0);	// Editor時に邪魔なので
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
	}
	
	// Update is called once per frame
	private	void	Update()
	{
		if (UnityEngine.Input.GetKey(KeyCode.Escape)) {
			if (!BackKeyDown &&
			    BackKeyAction != null) {
				BackKeyDown = true;
				BackKeyAction();
			}
		} else {
			BackKeyDown = false;
		}
	}

#if !UNITY_EDITOR
	private	void	FixedUpdate()
	{
#if ENABLE_PUSH
        if (string.IsNullOrEmpty(PushTokenString)) {
#if UNITY_IPHONE
			byte[] token   = NotificationServices.deviceToken;
			if(token != null) {
				PushTokenString = System.BitConverter.ToString(token).Replace("-", "").ToLower();
				Debug.Log("OnTokenReived:" + PushTokenString);
			}
#endif
#if UNITY_ANDROID
			var	plugin = new AndroidJavaObject("net.gree.unitywebview.RsUtils");
			PushTokenString = plugin.CallStatic<string>("gcm_GetRegistrationId");
			Debug.Log("OnTokenReived:" + PushTokenString);
#endif
			if (!string.IsNullOrEmpty(PushTokenString)) {
				PlayerPrefs.SetString("PushToken_Prefs", PushTokenString);
				PlayerPrefs.Save();
			}
		}
#endif
	}
#endif

	// 各シーンで最初に一回呼び出す
	public static	bool	InitGlobalObject()
	{
		if (m_Instance != null) {
			return true;
		}
#if UNITY_5_3_OR_NEWER
        if (SceneManager.GetActiveScene().name != "Boot") {
			// ブートじゃない場合は一度ブートを起動する
			m_bootup = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("Boot");
			return false;
		}
#else
        if (Application.loadedLevelName != "boot") {
			// ブートじゃない場合は一度ブートを起動する
			m_bootup = Application.loadedLevelName;
            Application.LoadLevel("Boot");
			return false;
        }
#endif

		var	obj = (GameObject)GameObject.Instantiate(Resources.Load("GlobalObject"));
		GameObject.DontDestroyOnLoad(obj);
		GlobalObject.m_Instance = obj.GetComponent<GlobalObject>();

		return true;
	}

	// インスタンスの取得
	public	static	GlobalObject	Instance
	{
		get {
			return GlobalObject.m_Instance;
		}
	}
    
	// 例外表示
	private void OnLogCallback(string message, string stackTrace) {
	}

#if UNITY_DEBUG
	public void OnGUI () {
		if (!show || (!Application.isPlaying && !showInEditor)) {
			return;
		}
		
		int collCount = System.GC.CollectionCount (0);
		
		if (lastCollectNum != collCount) {
			lastCollectNum = collCount;
			delta = Time.realtimeSinceStartup-lastCollect;
			lastCollect = Time.realtimeSinceStartup;
			lastDeltaTime = Time.deltaTime;
			collectAlloc = allocMem;
		}
		
		allocMem = (int)System.GC.GetTotalMemory (false);
		
		peakAlloc = allocMem > peakAlloc ? allocMem : peakAlloc;
		
		if (Time.realtimeSinceStartup - lastAllocSet > 0.3F) {
			int diff = allocMem - lastAllocMemory;
			lastAllocMemory = allocMem;
			lastAllocSet = Time.realtimeSinceStartup;
			
			if (diff >= 0) {
				allocRate = diff;
			}
		}
		
		StringBuilder text = new StringBuilder ();
		
		text.Append ("Currently allocated			");
		text.Append ((allocMem/1000000F).ToString ("0"));
		text.Append ("mb\n");
		
		text.Append ("Peak allocated				");
		text.Append ((peakAlloc/1000000F).ToString ("0"));
		text.Append ("mb (last	collect ");
		text.Append ((collectAlloc/1000000F).ToString ("0"));
		text.Append (" mb)\n");
		
		
		text.Append ("Allocation rate				");
		text.Append ((allocRate/1000000F).ToString ("0.0"));
		text.Append ("mb\n");
		
		text.Append ("Collection frequency		");
		text.Append (delta.ToString ("0.00"));
		text.Append ("s\n");
		
		text.Append ("Last collect delta			");
		text.Append (lastDeltaTime.ToString ("0.000"));
		text.Append ("s (");
		text.Append ((1F/lastDeltaTime).ToString ("0.0"));
		
		text.Append (" fps)");
		
		if (showFPS) {
			text.Append ("\n"+(1F/Time.deltaTime).ToString ("0.0")+" fps");
		}

		GUI.Box (new Rect (5,5,310,106+(showFPS ? 16 : 0)),"");
		GUI.Label (new Rect (10,5,1000,200), text.ToString ());
	}
#endif

	private	void	CreateUUID()
	{
		uuid = PlayerPrefs.GetString("UUID", "");
		if (string.IsNullOrEmpty(uuid))
		{
			uuid = UUID.CreateUUID();
			PlayerPrefs.SetString("UUID", uuid);
			PlayerPrefs.Save();
		}
	}

	public static string DeviceUniqueID {
		get
		{
			return uuid;
		}
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if(!pauseStatus){
		}
	}
}
