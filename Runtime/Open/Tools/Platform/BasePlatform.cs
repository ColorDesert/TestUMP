using System;
using System.IO;
using UnityEngine;

namespace Open.Tools.Platform
{
    public abstract class BasePlatform
    {
        public abstract void ShowDialog(string title, string message);

        public abstract long FreeDiskSize();

        public abstract long MemoryUsage();

        public abstract long MemoryAvailable();

        public abstract void CopyToClipboard(string text);

        public abstract string PasteFromClipboard();

        public abstract bool IsNotchScreen();

        public abstract int GetOffsetSafeArea();

        public abstract void TryHideStatusBar(bool hidden);

        public abstract void SystemShare(string content, byte[] image, int length);

        public abstract void Restart();

        public abstract void CopyAssetsFile(string srcFile, string destFile);

        public abstract int GetNetState();

        public abstract float GetBatteryLevel();

        public abstract string GetCPUInfo();

        public abstract long GetElapsedRealtime();

        protected static bool CopyFile(string srcPath, string destFile)
        {
            var srcInfo = new FileInfo(srcPath);
            var destInfo = new FileInfo(destFile);
            if (!Directory.Exists(destInfo.DirectoryName) && destInfo.DirectoryName != null)
            {
                Directory.CreateDirectory(destInfo.DirectoryName);
            }

            try
            {
                srcInfo.CopyTo(destFile, true);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取网络类型
        /// </summary>
        /// <returns>EPNetWorkType(None：-1，Mobile:1，WiFi：2)</returns>
        public abstract EPNetWorkType GetNetworkState();

        /// <summary>
        ///  添加网络类型变化监听
        /// </summary>
        /// <param name="callback">网络类型变化回调 EPNetWorkType</param>
        public abstract void AddNetworkListener(Action<EPNetWorkType> callback);

        /// <summary>
        /// 移除网络类型变化监听
        /// </summary>
        /// <param name="callback">网络类型变化回调</param>
        public abstract void RemoveNetworkListener(Action<EPNetWorkType> callback);
    }
}