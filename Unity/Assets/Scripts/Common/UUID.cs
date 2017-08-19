using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

public class UUID {
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern string _CreateUUIDString();
#endif
    
    
    public static string CreateUUID() {
        
        string uuid = "";
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        uuid = System.Guid.NewGuid().ToString();
#elif UNITY_IPHONE
        uuid = _CreateUUIDString();
#elif UNITY_ANDROID
        var klass = new AndroidJavaClass("java.util.UUID");
        var uuidObj = klass.CallStatic<AndroidJavaObject>("randomUUID");
        uuid = uuidObj.Call<string>("toString");
#endif
        return uuid;
    }
}