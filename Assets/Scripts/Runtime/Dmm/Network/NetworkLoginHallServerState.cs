using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.StateLogic;

namespace Dmm.Network
{
    public class NetworkLoginHallServerState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkLoginHallServerState";

        /// <summary>
        /// loginHallServer的时间间隔
        /// </summary>
        public float LoginHallServerTimeout = 30f;

        /// <summary>
        /// loginHallServer的次数
        /// </summary>
        public int LoginHallServerTimes = 3;

        /// <summary>
        /// 开始loginHallServer的时间
        /// </summary>
        private float _loginHallServerStartTime = 0;

        /// <summary>
        /// 当前状态是否超时
        /// </summary>
        private bool _isTimeout = false;

        /// <summary>
        /// 当前loginHallServer的次数
        /// </summary>
        private int _currentloginHallServerTimes = 0;

        private IDataContainer<User> _userContainer;

        private IDataContainer<HLoginResult> _hLoginResult;

        public override int GetStateCode()
        {
            return NetworkState.LoginHallServer;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Begin login hall server.");
            _isTimeout = false;
            _currentloginHallServerTimes = 0;
            LoginHallServer(context, time);

            var dataRepository = context.GetDataRepository();
            _hLoginResult = dataRepository.GetContainer<HLoginResult>(DataKey.HLoginResult);
            _userContainer = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        public override bool Process(IAppContext context, float time)
        {
            var res = _hLoginResult.Read();
            if ((res == null) && ((time - _loginHallServerStartTime) > LoginHallServerTimeout))
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
            MyLog.DebugWithFrame(Tag, "Finish login hall server.");
            var res = _hLoginResult.Read();
            var stateResult = new StateResult();
            stateResult.NextStateCode = StateResult.Error;
            stateResult.Result = StateResult.Error;

            if ((res != null) && (res.result == ResultCode.OK))
            {
                LoginOk(context);
                stateResult.NextStateCode = NetworkState.LoginHallServerOk;
                stateResult.Result = StateResult.Ok;
                return stateResult;
            }

            if (!_isTimeout)
            {
                stateResult.ErrMsg = string.Format("登录大厅服务器失败，请重新登陆\n【错误码{0}】）",
                    NetworkStateErrorCode.LoginHallServerFailCode);
            }
            else
            {
                stateResult.ErrMsg = string.Format("登录大厅服务器登录超时，请重新登陆\n【错误码{0}】",
                    NetworkStateErrorCode.LoginHallServerFailCode);
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
            var analyticManager = context.GetAnalyticManager();
            var configHolder = context.GetConfigHolder();
            var user = _userContainer.Read();
            dialogManager.HideDialog(DialogName.LoginPanel);

            // 统计登陆玩家的金蛋分布。
            var attrs = new Dictionary<string, string>();
            attrs.Add("sale_channel", configHolder.SaleChannel);
            attrs.Add("level", "" + (user == null ? 1 : user.level));
            analyticManager.EventValue("login_ok", attrs, (int) user.MyCurrency(CurrencyType.GOLDEN_EGG));
            analyticManager.SignIn(user == null ? "" : user.username);
        }

        private bool CheckBigReconnect(IAppContext context, float time)
        {
            if (_currentloginHallServerTimes < LoginHallServerTimes)
            {
                LoginHallServer(context, time);
                return false;
            }

            _isTimeout = true;
            return true;
        }

        private void LoginHallServer(IAppContext context, float time)
        {
            _loginHallServerStartTime = time;
            _currentloginHallServerTimes++;

            var remoteAPI = context.GetRemoteAPI();
            string username = null;
            string token = null;

            var dataRepository = context.GetDataRepository();
            var container = dataRepository.GetContainer<PLoginResult>(DataKey.PLoginResult);
            var res = container.Read();

            if (res != null)
            {
                username = res.username;
                token = res.token;
            }
            else
            {
                ShowBigReconnectDialog(context, "登录数据异常，请重新登录！");
            }

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
            {
                remoteAPI.Login(username, token);
                MyLog.InfoWithFrame(Tag, "Login hserver.");
            }
            else
            {
                ShowBigReconnectDialog(context, "用户名或密码无效，请重新登陆！");
            }
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