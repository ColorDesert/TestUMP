#if UNITY_ANDROID
using System;
using System.Runtime.InteropServices;
using Open.Tools.Manager;
using UnityEngine;

namespace Open.Tools.Platform
{
    public class AndroidPlatform : BasePlatform
    {
        private static readonly AndroidJavaObject AndroidObject =
            new AndroidJavaObject("com.open.sdk.tools.AndroidPluginTools");

        private static readonly AndroidJavaObject currentActivity =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        public override void ShowDialog(string title, string message)
        {
            AndroidObject.Call("showMessageBox", currentActivity, title, message);
        }

        public override long FreeDiskSize()
        {
            return AndroidObject.Call<long>("getFreeDiskSpace");
        }

        public override long MemoryUsage()
        {
            return AndroidObject.Call<long>("getMemorySize", currentActivity);
        }

        public override long MemoryAvailable()
        {
            return AndroidObject.Call<long>("getMemoryAvailable", currentActivity);
        }

        public override void CopyToClipboard(string text)
        {
            AndroidObject.Call<bool>("copyToClipboard", currentActivity, text);
        }

        public override string PasteFromClipboard()
        {
            return AndroidObject.Call<string>("pasteFromClipboard", currentActivity);
        }

        public override bool IsNotchScreen()
        {
            return AndroidObject.Call<bool>("hasNotchInScreen", currentActivity);
        }

        /// <summary>
        /// 获取安全偏移量
        /// </summary>
        /// <returns></returns>
        public override int GetOffsetSafeArea()
        {
            return AndroidObject.Call<int>("getOffsetSafeArea", currentActivity);
        }

        public override void TryHideStatusBar(bool hidden)
        {
            // 因为Android的在Native层测试没问题，但是在Unity中不行，所有Android不支持此功能
            // AndroidObject.Call("setStatusBar", currentActivity, hidden);
        }

        public override void SystemShare(string content, byte[] image, int length)
        {
            AndroidObject.Call("shareBySystem", currentActivity, content, image, length);
        }

        public override void Restart()
        {
            AndroidObject.Call("restartApp", currentActivity);
        }

        public override void CopyAssetsFile(string srcFile, string destFile)
        {
            AndroidObject.Call("copyAssetsFile", currentActivity, srcFile, destFile);
        }

        public override int GetNetState()
        {
            return AndroidObject.Call<int>("getNetState", currentActivity);
        }

        public override float GetBatteryLevel()
        {
            var strBatLevel = AndroidObject.Call<string>("getBatteryLevel");
            return string.IsNullOrEmpty(strBatLevel) ? 100.0f : float.Parse(strBatLevel.Remove(strBatLevel.Length - 1));
        }

        public override string GetCPUInfo()
        {
            var cpuInfo = AndroidObject.Call<string>("getCpuInfo");
            if (string.IsNullOrEmpty(cpuInfo)) return cpuInfo;

            var index = cpuInfo.IndexOf(' ');
            if (index == 0)
            {
                cpuInfo = cpuInfo.Substring(1, cpuInfo.Length - 1);
            }

            return cpuInfo;
        }

        public override long GetElapsedRealtime()
        {
            return AndroidObject.Call<long>("getElapsedRealtime");
        }

        public override EPNetWorkType GetNetworkState()
        {
            var type = AndroidObject.Call<int>("getNetworkState", currentActivity);
            return (EPNetWorkType)type;
        }

        public override void AddNetworkListener(Action<EPNetWorkType> callback)
        {
            //保存之前的回调
            var call = NetworkListenerManager.Instance.NetTypeCallback;
            NetworkListenerManager.Instance.NetTypeCallback += callback;
            if (call == null || call.GetInvocationList().Length == 0)
            {
                AndroidObject.Call("addNetworkListener", currentActivity, AndroidNetworkListener.Instance);
            }
        }

        public override void RemoveNetworkListener(Action<EPNetWorkType> callback)
        {
            NetworkListenerManager.Instance.NetTypeCallback -= callback;

            var call = NetworkListenerManager.Instance.NetTypeCallback;
            if (call == null || call.GetInvocationList().Length == 0)
            {
                AndroidObject.Call("RemoveNetworkListener", AndroidNetworkListener.Instance);
            }
        }

        /******************************************当前平台独有接口***********************************/
        /// <summary>
        /// 判断指定路径下的文件是否存在
        /// </summary>
        /// <param name="srcPath">文件路径</param>
        /// <returns></returns>
        public bool AssetFileExist(string srcPath)
        {
            return AndroidObject.Call<bool>("checkAssetsFileExist", currentActivity, srcPath);
        }

        /// <summary>
        /// 检查指定包名的app是否安装
        /// </summary>
        /// <param name="packageName">app 包名</param>
        /// <returns></returns>
        public bool CheckAppExist(string packageName)
        {
            return AndroidObject.Call<bool>("appIsInstall", currentActivity, packageName);
        }

        /// <summary>
        /// 获取Android 包中签名文件信息
        /// </summary>
        /// <param name="type">取值 “MD5”,"SHA1", "SHA256"</param>
        /// <returns></returns>
        public string GetAppSignValue(string type)
        {
            return AndroidObject.Call<string>("getAppSignatureValue", currentActivity, type);
        }
    }
}
#endif