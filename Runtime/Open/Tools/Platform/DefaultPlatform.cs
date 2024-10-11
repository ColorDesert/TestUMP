#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Open.Tools.Platform
{
    public class DefaultPlatform : BasePlatform
    {
        public override void ShowDialog(string title, string message)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog(title, message, "OK");
#else
            Debug.LogWarning("Show dialog: title=" + title + ", message=" + message);
#endif
        }

        public override long FreeDiskSize()
        {
            var LocalDrive = DriveInfo.GetDrives();
            for (var i = 0; i < LocalDrive.Length; i++)
            {
                var size = LocalDrive[LocalDrive.Length - 1].AvailableFreeSpace / 1000 / 1000;
                if (size > 0)
                {
                    return LocalDrive[LocalDrive.Length - 1].AvailableFreeSpace / 1000 / 1000;
                }
            }

            return 0;
        }

        public override long MemoryUsage()
        {
            return UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024 / 1024;
        }

        public override long MemoryAvailable()
        {
            return UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1024 / 1024;
        }

        public override void CopyToClipboard(string text)
        {
            var textEditor = new TextEditor
            {
                text = text
            };
            textEditor.SelectAll();
            textEditor.Copy();
        }

        public override string PasteFromClipboard()
        {
            var textEditor = new TextEditor();
            textEditor.Paste();
            return textEditor.text;
        }

        public override bool IsNotchScreen()
        {
            return false;
        }

        public override int GetOffsetSafeArea()
        {
            return 0;
        }

        public override void TryHideStatusBar(bool hidden)
        {
        }

        public override void SystemShare(string content, byte[] image, int length)
        {
        }

        public override void Restart()
        {
        }

        public override void CopyAssetsFile(string srcFile, string destFile)
        {
            CopyFile(srcFile, destFile);
        }

        public override int GetNetState()
        {
            return -1;
        }

        public override float GetBatteryLevel()
        {
            return SystemInfo.batteryLevel;
        }

        public override string GetCPUInfo()
        {
            return string.Empty;
        }

        public override long GetElapsedRealtime()
        {
            return 0L;
        }

        public override EPNetWorkType GetNetworkState()
        {
            return EPNetWorkType.WiFi;
        }

        public override void AddNetworkListener(Action<EPNetWorkType> callbak)
        {
            callbak?.Invoke(EPNetWorkType.WiFi);
        }

        public override void RemoveNetworkListener(Action<EPNetWorkType> callback)
        {
        }
    }
}

#endif