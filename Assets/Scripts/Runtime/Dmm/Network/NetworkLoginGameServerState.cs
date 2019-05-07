using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.StateLogic;

namespace Dmm.Network
{
    public class NetworkLoginGameServerState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkLoginGameServerState";

        /// <summary>
        /// loginGameServer的时间间隔
        /// </summary>
        public float LoginGameServerTimeout = 30f;

        /// <summary>
        /// loginGameServer的次数
        /// </summary>
        public int LoginGameServerTimes = 3;

        /// <summary>
        /// 开始loginGameServer的时间
        /// </summary>
        private float _loginGameServerStartTime = 0;

        /// <summary>
        /// 当前状态是否超时
        /// </summary>
        private bool _isTimeout = false;

        /// <summary>
        /// 当前loginGameServer的次数
        /// </summary>
        private int _currentloginGameServerTimes = 0;

        private IDataContainer<User> _userContainer;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<GLoginResult> _gLoginResult;

        public override int GetStateCode()
        {
            return NetworkState.LoginGameServer;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Begin login game server.");
            _isTimeout = false;
            _currentloginGameServerTimes = 0;
            LoginGameServer(context, time);

            var dataRepository = context.GetDataRepository();
            _gLoginResult = dataRepository.GetContainer<GLoginResult>(DataKey.GLoginResult);
            _userContainer = dataRepository.GetContainer<User>(DataKey.MyUser);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        public override bool Process(IAppContext context, float time)
        {
            return true;
            var res = _gLoginResult.Read();
            if ((res == null) && ((time - _loginGameServerStartTime) > LoginGameServerTimeout))
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
            MyLog.DebugWithFrame(Tag, "Finish login game server.");
            var res = _gLoginResult.Read();
            var stateResult = new StateResult();
            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;

//            if ((res != null) && (res.result == ResultCode.OK))
//            {
                LoginOk(context);
                stateResult.NextStateCode = NetworkState.LoginGameServerOk;
                stateResult.Result = StateResult.Ok;
                return stateResult;
//            }

            if (!_isTimeout)
            {
                stateResult.ErrMsg = string.Format("登录游戏服务器失败，请重新登陆\n【错误码{0}】）",
                    NetworkStateErrorCode.LoginGameServerFailCode);
            }
            else
            {
                stateResult.ErrMsg = string.Format("登录游戏服务器登录超时，请重新登陆\n【错误码{0}】",
                    NetworkStateErrorCode.LoginGameServerFailCode);
            }
            return stateResult;
        }

        /// <summary>
        /// 登录游戏成功
        /// </summary>
        /// <param name="context"></param>
        private void LoginOk(IAppContext context)
        {
            var dialogManager = context.GetDialogManager();
        }

        private bool CheckBigReconnect(IAppContext context, float time)
        {
            if (_currentloginGameServerTimes < LoginGameServerTimes)
            {
                LoginGameServer(context, time);
                return false;
            }

            _isTimeout = true;
            return true;
        }

        private void LoginGameServer(IAppContext context, float time)
        {
            _loginGameServerStartTime = time;
            _currentloginGameServerTimes++;

            var remoteAPI = context.GetRemoteAPI();

            // 发送glogin
        }

        private void ShowBigReconnectDialog(IAppContext context, string errMsg = "")
        {
            var dialogManager = context.GetDialogManager();

            dialogManager.ShowDialog<DisconnectedDialog>(DialogName.DisconnectedDialog, false, true,
                (dialog) =>
                {
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        dialog.ApplyData(errMsg);
                    }

                    dialog.Show();
                });
        }
    }
}