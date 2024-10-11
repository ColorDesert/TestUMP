using System;

namespace Open.Tools.Manager
{
    /// <summary>
    /// 网络状态监听管理类
    /// </summary>
    public class NetworkListenerManager

    {
        private static NetworkListenerManager _instance;

        public static NetworkListenerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NetworkListenerManager();
                }

                return _instance;
            }
        }

        public Action<EPNetWorkType> NetTypeCallback;
    }
}