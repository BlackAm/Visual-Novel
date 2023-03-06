using UnityEngine;
using System.Collections;
using Almond.Util;
using System;
public class PlatformSdkManager : MonoBehaviour
{
    virtual public string Uid
    {
        get
        {
            return "";
        }
        set
        {

        }
    }

    virtual public int RecommendedServerID
    {
        get
        {
            return 0;
        }
    }
    virtual public void Log(string msg)
    {

    }

    void Start()
    {
        Instance = this;
    }
    public bool m_isLoginDone = false;
    virtual public bool IsLoginDone
    {
        get
        {
            return false;
        }
        set
        {
            m_isLoginDone = value;
        }
    }
    public static PlatformSdkManager Instance;
#if UNITY_IPHONE
    public const string LOGIN_URL = "http://{0}/cgi-bin/login_ios/{1}?suid={2}&timestamp={3}&plataccount={4}&sign={5}&tocken={6}&port={7}";
#else
    public const string LOGIN_URL = "{0}?gameid={1}&udid={2}&osstr=android{3}&device={4}&channel={5}&ver={6}&username={7}";
#endif
    string GetSdkManagerName()
    {
        string name = string.Empty;
#if UNITY_ANDROID
        name = "AndroidSdkManager";
#elif UNITY_IPHONE
        name = "IOSPlatformSDKManager";
#endif
        return name;
    }
    virtual public void Shake(long milliseconds)
    {

    }
    virtual public void StopShake()
    {

    }
    virtual public void RestartGame()
    {
    }

    virtual public void Login()
    {
        
    }
    virtual public void Login(Action<string,string> loginResult)
    {
        loginResult?.Invoke("Almond_15", "11");
    }

    virtual public void Charge(int amount, string productName = "", string roleName = "", string level = "", string tongName = "")
    {
    }

    virtual public void GotoGameCenter()
    {
    }

    virtual public void CheckUpdate()
    {
    }

    virtual public void CleanLocalData()
    {
    }

    virtual public void SendLoginLog()
    {
    }

    virtual public void SendStartGameLog()
    {
    }

    virtual public void SendBeforeLoginLog()
    {
    }

    virtual public void OpenForum()
    {
    }

    virtual public void OnSwitchAccount()
    {

    }

    virtual public void SetNetwork()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject m_mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                    m_mainActivity.Call("gotoNetworkSetting");
#endif
    }
    virtual public void LoginResult(string str)
    {

    }

    virtual public void ShowAssistant()
    {
    }

    virtual public void AddNotificationRecord(int tag)
    {

    }

    virtual public void SetupNotificationData()
    {

    }
    virtual public void OnSetupNotification()
    {
    }

    virtual public void CreateRoleLog(string roleName, string serverId)
    {

    }

    virtual public void RoleLevelLog(string roleName, string serverId)
    {

    }

    virtual public void EnterGameLog(string serverId)
    {
        
    }

    virtual public void SetupInfo()
    {
    }

    virtual public void LoginAnalytics(ulong dbid, string name, int level, int vipLevel,long crateTime) { }
    virtual public void RegistAnalytics(ulong dbid, string name, int level, int vipLevel, long crateTime) { }
    virtual public void UpdateAnalytics(ulong dbid, string name, int level, int vipLevel, long crateTime) { }
    virtual public void PayAnalytics() { }
    virtual public void TutorialStepAnalytics(ulong dbid, int step) { }
    virtual public void Exit() { }
    virtual public void BindAccount() { }

    virtual public void ItemBuy(string itemID) { }
    virtual public void ItemConsumeResult(string result) { }

    virtual public string GetID() { return ""; }
}


