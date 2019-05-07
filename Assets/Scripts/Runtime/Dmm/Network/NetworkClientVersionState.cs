using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Msg;
using Dmm.StateLogic;
using UnityEngine;

namespace Dmm.Network
{
    public class NetworkClientVersionState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkClientVersionState";

        public override int GetStateCode()
        {
            return NetworkState.ClientVersion;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        /// <summary>
        /// 获取ClientVersion的时间间隔
        /// </summary>
        public float GetClientVersionTimeout = 10f;

        /// <summary>
        /// 获取ClientVersion的次数
        /// </summary>
        public int GetClientVersionTimes = 3;

        /// <summary>
        /// 开始获取ClientVersion的时间
        /// </summary>
        private float _getClientVersionStartTime = 0;

        /// <summary>
        /// 当前获取ClientVersion的次数
        /// </summary>
        private int _currentGetClientVersionTimes = 0;

        /// <summary>
        /// 当前状态是否超时
        /// </summary>
        private bool _isTimeout = false;

        public override void Initialize(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Send client version.");
            _currentGetClientVersionTimes = 0;
            _isTimeout = false;
            GetClientVersion(context, time);
        }

        public override bool Process(IAppContext context, float time)
        {
            var dataRepository = context.GetDataRepository();
            var container = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            var res = container.Read();
            if ((res == null) && ((time - _getClientVersionStartTime) > GetClientVersionTimeout))
            {
                if (CheckBigReconnect(context, time))
                {
                    return true;
                }
            }
            return res != null;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var dataRepository = context.GetDataRepository();
            var container = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            var res = container.Read();
            var loginType = LoginRecord.LastLoginType;
            LoginRecord.CurrentLoginType = loginType;
            var stateResult = new StateResult();
            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;

            MyLog.InfoWithFrame(Tag, string.Format("Finish client version, login type: {0}", loginType));

            if ((res != null) && (res.result == ResultCode.OK))
            {
                stateResult.Result = StateResult.Ok;
                if (loginType == LoginRecord.NoLogin)
                {
                    stateResult.NextStateCode = NetworkState.ChooseLoginType;
                }
                else
                {
                    stateResult.NextStateCode = NetworkState.LoginGateServer;
                }
                return stateResult;
            }

            if (!_isTimeout)
            {
                stateResult.ErrMsg = string.Format("连接服务器失败，请重新登陆\n【错误码{0}】",
                    NetworkStateErrorCode.GetClientVersionFailCode);
            }
            else
            {
                stateResult.ErrMsg = string.Format("登录超时，请重新登陆\n【错误码{0}】",
                    NetworkStateErrorCode.GetClientVersionFailCode);
            }
            return stateResult;
        }

        private bool CheckBigReconnect(IAppContext context, float time)
        {
            if (_currentGetClientVersionTimes < GetClientVersionTimes)
            {
                GetClientVersion(context, time);
                return false;
            }

            _isTimeout = true;
            return true;
        }

        private void GetClientVersion(IAppContext context, float time)
        {
            _getClientVersionStartTime = time;
            _currentGetClientVersionTimes++;

            var network = NetworkType.NetworkTypeOf(Application.internetReachability);
            var remoteAPI = context.GetRemoteAPI();
            var configHolder = context.GetConfigHolder();
            // 发送ClientVersion命令。
            remoteAPI.GetVersionData(
                configHolder.ClientVersion,
                // 如果传给服务器端的是null，则服务器端会设置成默认的渠道。
                // 所以不必担心，直接使用SaleChannel。
                configHolder.SaleChannel,
                configHolder.Product,
                configHolder.Platform,
                network,
                SystemInfo.deviceModel,
                GetDeviceId(context)
            );
        }

        private string GetDeviceId(IAppContext context)
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier;
            var ios = context.GetIosSDK();
#if UNITY_IOS
            deviceId = ios.GetDeviceId();
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                ios.SaveDeviceId(deviceId);
                MyLog.InfoWithFrame(Tag, string.Format("save deviceId:{0}", deviceId));
            }
            else
            {
                MyLog.InfoWithFrame(Tag, string.Format("use deviceId:{0}", deviceId));
            }
#endif
#if UNITY_ANDROID // TODO 从安卓取出deviceId。
#endif
            return deviceId;
        }
    }
}