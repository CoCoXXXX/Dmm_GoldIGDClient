namespace Dmm.StateLogic
{
    public class StateResult
    {
        public const int Null = -100;

        /// <summary>
        /// 状态成功结束
        /// </summary>
        public const int Ok = 1;

        /// <summary>
        /// 该状态没有成功结束
        /// </summary>
        public const int Error = -1;

        /// <summary>
        /// 状态不存在
        /// </summary>
        public const int StateNotFound = -2;

        /// <summary>
        /// 状态开始执行
        /// </summary>
        public const int Started = 100;

        /// <summary>
        /// 主动终止，不是意外终止
        /// </summary>
        public const int Aborted = 101;

        public int Result;

        public string ErrMsg;

        public int NextStateCode;
    }

    public class NetworkStateErrorCode
    {
        public const int PipFailCode = 101;

        public const int BuildFirstCacheFailCode = 102;

        public const int DownloadResourcesFailCode = 103;

        public const int ConnectGateServerFailCode = 104;

        public const int GetClientVersionFailCode = 105;

        public const int GetLoginTypeFailCode = 106;

        public const int LoginGateServerFailCode = 107;

        public const int ConnectHallServerFailCode = 108;

        public const int LoginHallServerFailCode = 109;

        public const int LoginOkFailCode = 110;

        public const int ConnectGameServerFailCode = 111;

        public const int LoginGameServerFailCode = 112;
    }
}