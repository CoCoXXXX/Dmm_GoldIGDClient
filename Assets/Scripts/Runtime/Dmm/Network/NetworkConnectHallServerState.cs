using System;
using System.Net;
using System.Net.Sockets;
using Dmm.App;
using Dmm.Log;
using Dmm.Msg;
using Dmm.StateLogic;

namespace Dmm.Network
{
    public class NetworkConnectHallServerState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkConnectHallServerState";

        public override int GetStateCode()
        {
            return NetworkState.ConnectHallServer;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        /// <summary>
        /// 重连的时间间隔
        /// </summary>
        public float ReconnectTimeout = 10f;

        /// <summary>
        /// 重连的次数
        /// </summary>
        public int ReconnectTimes = 3;

        /// <summary>
        /// 开始连接的时间
        /// </summary>
        private float _startConnectTime = 0;

        /// <summary>
        /// 当前状态是否超时
        /// </summary>
        private bool _isTimeout = false;

        /// <summary>
        /// 当前重连的次数
        /// </summary>
        private int _currentReconnectTimes = 0;

        public override void Initialize(IAppContext context, float time)
        {
            _isTimeout = false;
            _currentReconnectTimes = 0;
            ConnectHallServer(context, time);
        }

        public override bool Process(IAppContext context, float time)
        {
            var network = context.GetNetworkManager();
            var isConnected = network.IsConnected();
            if ((!isConnected) && ((time - _startConnectTime) > ReconnectTimeout))
            {
                if (CheckBigReconnect(context, time))
                {
                    return true;
                }
            }
            return isConnected;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var network = context.GetNetworkManager();
            var stateResult = new StateResult();
            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;

            if (network.GetServer() == Server.HServer &&
                network.IsConnected())
            {
                MyLog.InfoWithFrame(Tag, "HServer connect ok.");
                stateResult.NextStateCode = NetworkState.LoginHallServer;
                stateResult.Result = StateResult.Ok;
                return stateResult;
            }

            MyLog.InfoWithFrame(Tag, "Connect HServer Fail!");
            if (!_isTimeout)
            {
                stateResult.ErrMsg = string.Format("连接服务器失败，请重新登陆\n【错误码{0}】",
                    NetworkStateErrorCode.ConnectHallServerFailCode);
            }
            else
            {
                stateResult.ErrMsg = string.Format("登录超时，请重新登陆\n【错误码{0}】",
                    NetworkStateErrorCode.ConnectHallServerFailCode);
            }
            return stateResult;
        }

        private bool CheckBigReconnect(IAppContext context, float time)
        {
            if (_currentReconnectTimes < ReconnectTimes)
            {
                ConnectHallServer(context, time);
                return false;
            }

            _isTimeout = true;
            return true;
        }

        private void ConnectHallServer(IAppContext context, float time)
        {
            var network = context.GetNetworkManager();
            var pip = context.GetPIPLogic();

            _startConnectTime = time;
            _currentReconnectTimes++;
            var addressFamily = AddressFamily.InterNetwork;
#if UNITY_IOS
            if (pip.EnableIpV6())
            {
                try
                {
                    var addresses = Dns.GetHostAddresses("www.baidu.com");
                    addressFamily = addresses[0].AddressFamily;
                }
                catch (Exception e)
                {
                    MyLog.ErrorWithFrame(Tag,"DNS 解析异常！");
                }
            }
#endif

            network.Connect(Server.HServer, addressFamily);
        }
    }
}