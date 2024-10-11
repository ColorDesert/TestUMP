using System;
using Open.Tools.Manager;
using UnityEngine;

namespace Open.Tools.Platform
{
    public class AndroidNetworkListener : AndroidJavaProxy

    {
        private static AndroidNetworkListener _instance;

        public static AndroidNetworkListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AndroidNetworkListener();
                }

                return _instance;
            }
        }

        public AndroidNetworkListener() : base("com.open.sdk.tools.utils.NetConnectManager$OnNetTypeListener")
        {
        }

        /// <summary>
        /// 网络类型变化的回调
        /// </summary>
        /// <param name="netType">网络类型 none：-1，mobile:1，wifi：2</param>
        public void onNetTypeChange(int netType)
        {
            NetworkListenerManager.Instance.NetTypeCallback?.Invoke((EPNetWorkType)netType);
        }
    }
}