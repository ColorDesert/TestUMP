#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using Open.Tools.Manager;

namespace Open.Tools.Platform
{
    public class IOSPlatform : BasePlatform
    {
        // dllimport start
        [DllImport("__Internal")]
        private static extern void _UnityPluginTools_ShowDialog(string title, string message);

        [DllImport("__Internal")]
        private static extern long _UnityPluginTools_FreeDiskSize();

        [DllImport("__Internal")]
        private static extern long _UnityPluginTools_MemoryUsage();

        [DllImport("__Internal")]
        private static extern long _UnityPluginTools_MemoryAvailable();

        [DllImport("__Internal")]
        private static extern void _UnityPluginTools_Copy(string text);

        [DllImport("__Internal")]
        private static extern string _UnityPluginTools_Paste();

        [DllImport("__Internal")]
        private static extern bool _UnityPluginTools_IsNotchScreen();

        [DllImport("__Internal")]
        private static extern float _UnityPluginTools_GetNotchHeight();

        [DllImport("__Internal")]
        private static extern void _UnityPluginTools_TryHideStatusBar(bool hidden);

        [DllImport("__Internal")]
        private static extern void _UnityPluginTools_SystemShare(string content, byte[] image, int length);

        [DllImport("__Internal")]
        private static extern void _UnityPluginTools_Restart();

        [DllImport("__Internal")]
        private static extern float _UnityPluginTools_BatteryLevel();

        [DllImport("__Internal")]
        private static extern long _UnityPluginTools_SystemUptime();


		public delegate void UnityPluginNetworkStatusCallback(long status);
		[MonoPInvokeCallback(typeof(UnityPluginNetworkStatusCallback))] 
		public static void UnityPluginNetworkStatusDidChanged(long status) {
			NetworkListenerManager.Instance.NetTypeCallback?.Invoke((EPNetWorkType)status);
		}

		[DllImport("__Internal")]
		private static extern long _UnityPluginTools_getNetworkStatus();

		[DllImport("__Internal")]
		private static extern void _UnityPluginTools_startNetworkStatusMonitoring(UnityPluginNetworkStatusCallback callback);
		
		[DllImport("__Internal")]
		private static extern void _UnityPluginTools_stopNetworkStatusMonitoring();

        // dllimport end

        public override void ShowDialog(string title, string message)
        {
            _UnityPluginTools_ShowDialog(title, message);
        }

        public override long FreeDiskSize()
        {
            return _UnityPluginTools_FreeDiskSize();
        }

        public override long MemoryUsage()
        {
            return _UnityPluginTools_MemoryUsage();
        }

        public override long MemoryAvailable()
        {
            return _UnityPluginTools_MemoryAvailable();
        }

        public override void CopyToClipboard(string text)
        {
            _UnityPluginTools_Copy(text);
        }

        public override string PasteFromClipboard()
        {
            return _UnityPluginTools_Paste();
        }

        public override bool IsNotchScreen()
        {
            return _UnityPluginTools_IsNotchScreen();
        }

        public override int GetOffsetSafeArea()
        {
            return (int)_UnityPluginTools_GetNotchHeight();
        }

        public override void TryHideStatusBar(bool hidden)
        {
            _UnityPluginTools_TryHideStatusBar(hidden);
        }

        public override void SystemShare(string content, byte[] image, int length)
        {
            _UnityPluginTools_SystemShare(content, image, length);
        }

        public override void Restart()
        {
            _UnityPluginTools_Restart();
        }

        public override void CopyAssetsFile(string srcFile, string destFile)
        {
            CopyFile(srcFile, destFile);
        }

        public override int GetNetState()
        {
            return 1;
        }

        public override float GetBatteryLevel()
        {
            return _UnityPluginTools_BatteryLevel();
        }

        public override string GetCPUInfo()
        {
            return string.Empty;
        }

        public override long GetElapsedRealtime()
        {
            return _UnityPluginTools_SystemUptime();
        }

        public override EPNetWorkType GetNetworkState()
        {
            return (EPNetWorkType)_UnityPluginTools_getNetworkStatus();
        }

        public override void AddNetworkListener(Action<EPNetWorkType> callback)
        {
			var call = NetworkListenerManager.Instance.NetTypeCallback;
            NetworkListenerManager.Instance.NetTypeCallback += callback;
            if (call == null || call.GetInvocationList().Length == 0)
            {
                _UnityPluginTools_startNetworkStatusMonitoring(UnityPluginNetworkStatusDidChanged);
            }
        }

        public override void RemoveNetworkListener(Action<EPNetWorkType> callback)
        {
			NetworkListenerManager.Instance.NetTypeCallback -= callback;

            var call = NetworkListenerManager.Instance.NetTypeCallback;
            if (call == null || call.GetInvocationList().Length == 0)
            {
				_UnityPluginTools_stopNetworkStatusMonitoring();
            }
        }
        
        /******************************************当前平台独有接口***********************************/
        public int GetIOSDeviceGeneration()
        {
            int iOSGen;
            var iphoneGnr = UnityEngine.iOS.Device.generation;
            switch (iphoneGnr)
            {
                case UnityEngine.iOS.DeviceGeneration.iPhone:
                case UnityEngine.iOS.DeviceGeneration.iPhone3G:
                case UnityEngine.iOS.DeviceGeneration.iPhone3GS:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadMini4Gen:
                case UnityEngine.iOS.DeviceGeneration.iPhone4:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPhone4S:
                case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
                case UnityEngine.iOS.DeviceGeneration.iPodTouch6Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadAir1:
                case UnityEngine.iOS.DeviceGeneration.iPadAir2:
                case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
                case UnityEngine.iOS.DeviceGeneration.iPhone5:
                case UnityEngine.iOS.DeviceGeneration.iPhone5C:
                case UnityEngine.iOS.DeviceGeneration.iPhone5S:
                case UnityEngine.iOS.DeviceGeneration.iPhone6:
                case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
                case UnityEngine.iOS.DeviceGeneration.iPhone6S:
                case UnityEngine.iOS.DeviceGeneration.iPhone6SPlus:
                {
                    iOSGen = 2;
                    break;
                }
                case UnityEngine.iOS.DeviceGeneration.iPhone7:
                case UnityEngine.iOS.DeviceGeneration.iPhone7Plus:
                case UnityEngine.iOS.DeviceGeneration.iPad5Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadPro1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadPro2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadPro10Inch1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadPro10Inch2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPhoneSE1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPhoneX:
                case UnityEngine.iOS.DeviceGeneration.iPhone8:
                case UnityEngine.iOS.DeviceGeneration.iPhone8Plus:
                case UnityEngine.iOS.DeviceGeneration.iPhoneXR:
                case UnityEngine.iOS.DeviceGeneration.iPhoneXS:
                case UnityEngine.iOS.DeviceGeneration.iPhoneXSMax:
                {
                    iOSGen = 1;
                    break;
                }
                case UnityEngine.iOS.DeviceGeneration.iPadPro11Inch:
                case UnityEngine.iOS.DeviceGeneration.iPadPro3Gen:
                {
                    iOSGen = 0;
                    break;
                }
                default:
                {
                    iOSGen = 0;
                    break;
                }
            }

            return iOSGen;
        }
    }
}

#endif