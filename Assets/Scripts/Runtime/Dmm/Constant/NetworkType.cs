using UnityEngine;

namespace Dmm.Constant
{
    public class NetworkType
    {
        /// <summary>
        /// 断网。
        /// </summary>
        public const int Disconnected = 0;

        /// <summary>
        /// 移动网络。
        /// </summary>
        public const int MobilePhone = 1;

        /// <summary>
        /// wifi或者网线。
        /// </summary>
        public const int WifiOrCable = 2;

        public static int NetworkTypeOf(NetworkReachability reachability)
        {
            switch (reachability)
            {
                case NetworkReachability.NotReachable:
                    return Disconnected;

                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return MobilePhone;

                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return WifiOrCable;

                default:
                    return Disconnected;
            }
        }
    }
}