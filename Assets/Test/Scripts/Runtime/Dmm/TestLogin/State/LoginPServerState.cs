using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;
using Dmm.StateLogic;
using Dmm.WeChat;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class LoginPServerState : StateAdapter<IAppContext>
    {
        private const string Tag = "LoginPServerState";

        public override int GetStateCode()
        {
            return TestLoginStateCode.LoginPServerState;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        private IAppContext _context;

        private bool _loginStarted = false;

        private float _wechatAuthResultRefreshTime;

        private float _wechatLoginResultRefreshTime;

        private bool _isServerVerifyWechatSucc = false;

        /// <summary>
        /// 获取登陆结果的时间间隔
        /// </summary>
        public float LoginResultTimeOut = 90f;

        private float _startLoginTime = 0f;

        /// <summary>
        /// 当前状态是否超时
        /// </summary>
        private bool _isTimeout = false;

        private IDataContainer<WechatAuthResult> _wechatAuthResultContainer;
        private IDataContainer<WechatLoginResult> _wechatLoginResultContainer;

        public override void Initialize(IAppContext context, float time)
        {
            _context = context;
            _startLoginTime = time;
            _isTimeout = false;

            var dataRepository = _context.GetDataRepository();
            _wechatAuthResultContainer = dataRepository.GetContainer<WechatAuthResult>(DataKey.WechatAuthResult);
            _wechatLoginResultContainer = dataRepository.GetContainer<WechatLoginResult>(DataKey.WechatLoginResult);

            var loginType = LoginRecord.CurrentLoginType;
            var remoteAPI = context.GetRemoteAPI();
            switch (loginType)
            {
                case LoginRecord.NoLogin:
                case LoginRecord.Visitor:
                    _loginStarted = LoginVisitor(context, time);
                    break;

                case LoginRecord.NormalUser:
                {
                    // 上一次是用户登陆。
                    var username = LoginRecord.LastUsername;
                    var password = LoginRecord.LastPassword;

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        // 存在之前使用的用户名和密码，则直接使用之前的用户名和密码登陆。
                        remoteAPI.PLogin(username, password);
                        _loginStarted = true;
                        MyLog.InfoWithFrame(Tag, string.Format("user login: {0}:{1}", username, password));
                    }
                    else
                    {
                        // 用户名密码格式不正确，使用游客登录。
                        _loginStarted = LoginVisitor(context, time);
                    }
                    break;
                }

                case LoginRecord.XiaoMi:
                {
                    // 小米账号不准使用其他方式登陆，也不可以使用绑定账户功能。
                    remoteAPI.PVisitorLogin(LoginRecord.LastNickname, LoginRecord.LastUsername, null);
                    MyLog.InfoWithFrame(
                        Tag,
                        string.Format(
                            "xiaomi login: {0}:{1}",
                            LoginRecord.LastUsername,
                            LoginRecord.LastNickname)
                    );
                    break;
                }

                case LoginRecord.Wechat:
                {
                    //微信登陆
                    _loginStarted = LoginWechat(context, time);
                    break;
                }

                default:
                    _loginStarted = false;
                    break;
            }
        }

        private bool LoginVisitor(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Start visitor login gate server.");

            var remoteAPI = context.GetRemoteAPI();
            // 读取之前有没有用游客登陆过。
            var visitor = LoginRecord.LastVisitorId;
            var visitorUsername = LoginRecord.LastVisitorUsername;

            if (string.IsNullOrEmpty(visitor))
            {
                // 没有的visitorId的情况下，则使用设备的deviceUniqueIdentifier作为游客用户名。
                visitor = GetDeviceId(context);
                LoginRecord.LastVisitorId = visitor;
            }
            else
            {
                MyLog.InfoWithFrame(Tag, string.Format("old visitor: {0}", visitor));
            }

            if (string.IsNullOrEmpty(visitorUsername))
            {
                visitorUsername = GetVisitorUsername(context);
                LoginRecord.LastVisitorUsername = visitorUsername;
            }
            MyLog.InfoWithFrame(Tag, string.Format("use visitorUsername: {0}", visitorUsername));

            LoginRecord.SaveAll();

            // 上一次是游客登录。
            // 如果是游客登陆的话，就不要记录用户名了。

            remoteAPI.PVisitorLogin("", visitor, visitorUsername);
            MyLog.InfoWithFrame(Tag, string.Format("visitor login: {0} : {1}", visitor, visitorUsername));
            return true;
        }

        #region LoginWechat

        private bool LoginWechat(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Start wechat login gate server.");

            _isServerVerifyWechatSucc = false;
            _wechatAuthResultContainer.ClearNotInvalidate();
            _wechatLoginResultContainer.ClearNotInvalidate();

            var openId = LoginRecord.GetOpenId();

            if (!string.IsNullOrEmpty(openId))
            {
                DoWechatLogin(context, openId);
            }
            else
            {
                GetAuthCode(context);
            }
            return true;
        }

        private float getAuthCodeTimeout = 180f;

        private void GetAuthCode(IAppContext context)
        {
            var deviceId = GetDeviceId(context);
            var task = context.GetTaskManager();
            var wechat = context.GetWeChatManager();

            MyLog.DebugWithFrame(Tag, string.Format("Start GetAuthCode deviceId: {0}", deviceId));

            task.ExecuteTask(CheckIsGetAuthCode, GetAuthCodeTimeOut, getAuthCodeTimeout);
            wechat.Auth(deviceId);
        }

        private bool CheckIsGetAuthCode()
        {
            var wechat = _context.GetWeChatManager();
            var authResult = wechat.GetWxAuthResult();
            var dialog = _context.GetDialogManager();
            if (authResult == null)
            {
                return false;
            }
            if (authResult.Result != WxAuthResult.Ok)
            {
                if (!string.IsNullOrEmpty(authResult.ErrMsg))
                {
                    dialog.ShowToast(authResult.ErrMsg, 2);
                }
                else
                {
                    dialog.ShowToast("登陆失败，请重新登录！", 2);
                }
                ChangeAccount();
            }
            else
            {
                WechatAuth(_context, authResult.Code);
            }
            return true;
        }

        private void GetAuthCodeTimeOut()
        {
            var dialog = _context.GetDialogManager();
            dialog.ShowConfirmBox("请检查您的微信是否正常安装或者被禁用\n如有疑问请与客服联系");
            ChangeAccount();
        }

        private void WechatAuth(IAppContext context, string authCode)
        {
            MyLog.DebugWithFrame(Tag, string.Format("Start WechatAuth authCode: {0}", authCode));
            var remoteAPI = context.GetRemoteAPI();
            remoteAPI.WechatAuth(authCode);
        }

        private void DoWechatLogin(IAppContext context, string openid)
        {
            MyLog.DebugWithFrame(Tag, string.Format("Start WechatLogin openid: {0}", openid));

            var remoteAPI = context.GetRemoteAPI();
            remoteAPI.WechatLogin(openid);
        }

        private void RefreshWechatAuthResult(IAppContext context)
        {
            var dialogManager = _context.GetDialogManager();
            if (_wechatAuthResultRefreshTime >= _wechatAuthResultContainer.Timestamp)
            {
                return;
            }

            _wechatAuthResultRefreshTime = _wechatAuthResultContainer.Timestamp;

            var res = _wechatAuthResultContainer.Read();
            if (res.res.code != ResultCode.OK)
            {
                MyLog.ErrorWithFrame(Tag, "WechatAuthResult res.res.code =" + res.res.code);
                var msg = res.res.msg;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowToast(msg, 2, true);
                }
                else
                {
                    dialogManager.ShowToast("登陆失败，请重新登录！", 2, true);
                }
                ChangeAccount();
            }
            else
            {
                var openId = res.open_id;
                LoginRecord.SaveOpenId(openId);

                MyLog.InfoWithFrame(Tag, "WechatAuthResult SaveOpenId openId = " + openId);
                DoWechatLogin(context, openId);
            }
        }

        private void RefreshWechatLoginResult(IAppContext context)
        {
            ;
            var dialogManager = _context.GetDialogManager();
            if (_wechatLoginResultRefreshTime >= _wechatLoginResultContainer.Timestamp)
            {
                return;
            }

            _wechatLoginResultRefreshTime = _wechatLoginResultContainer.Timestamp;

            var res = _wechatLoginResultContainer.Read();
            if (res.res.code != ResultCode.OK)
            {
                MyLog.ErrorWithFrame(Tag, "WechatLoginResult res.res.code =" + res.res.code);
                var msg = res.res.msg;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowToast(msg, 2, true);
                }
                else
                {
                    dialogManager.ShowToast("登陆失败，请重新登录！", 2, true);
                }
                ChangeAccount();
            }
            else
            {
                MyLog.InfoWithFrame(Tag, "WechatLoginResult succ");
                _isServerVerifyWechatSucc = true; //微信登录验证成功
            }
        }

        private void ChangeAccount()
        {
            Abort();
            var network = _context.GetNetworkManager();
            network.Logout();
        }

        #endregion

        public override bool Process(IAppContext context, float time)
        {
            var dialogManager = _context.GetDialogManager();
            if (LoginRecord.CurrentLoginType == LoginRecord.Wechat && !_isServerVerifyWechatSucc)
            {
                RefreshWechatAuthResult(context);
                RefreshWechatLoginResult(context);
                return false;
            }

            if (!_loginStarted)
            {
                return true;
            }

            var dataRepository = _context.GetDataRepository();
            var container = dataRepository.GetContainer<PLoginResult>(DataKey.PLoginResult);
            var res = container.Read();

            if ((res == null) && ((time - _startLoginTime) > LoginResultTimeOut))
            {
                _isTimeout = true;
                return true;
            }

            if (res == null)
            {
                return false;
            }

            var dialog = context.GetDialogManager();

            if (res.result != ResultCode.OK)
            {
                // 登陆出错了，弹出对话框提示玩家。
                switch (res.result)
                {
                    case ResultCode.P_LOGIN_INVALID_CLIENT_VERSION:
                        dialogManager.ShowToast("客户端版本无效，请下载最新的客户端使用！", 4, true);
                        break;

                    case ResultCode.P_LOGIN_USER_NOT_FOUND:
                        dialogManager.ShowToast("用户不存在！", 2, true);
                        break;

                    case ResultCode.P_LOGIN_PASSWORD_WRONG:
                        dialogManager.ShowToast("密码错误！", 2, true);
                        break;

                    case ResultCode.P_LOGIN_NO_HALL_SERVER:
                        dialogManager.ShowToast("没有找到服务器，请稍后重新登陆！", 4, true);
                        break;

                    case ResultCode.P_USER_WRONG_DEVICE:
                        dialogManager.ShowToast("登陆设备错误，请重新使用游客登陆！", 4, true);
                        // 清空当前记录的visitorUsername。
                        ClearVisitorUsername(context);
                        // 下一次自动使用游客登陆。
                        LoginRecord.LastLoginType = LoginRecord.Visitor;
                        LoginRecord.SaveAll();
                        break;
                }

                dialog.ShowConfirmBox("登陆失败，res.result ==" + res.result, true, "重新登录", () => { ChangeAccount(); }, false, "", null, false, false, false);
            }
            else
            {
                dialog.ShowConfirmBox("PLoginResult.username = " + res.username + 
                                      "\n\nPLoginResult.token = " + res.token
                                      + "\n\nPLoginResult.hall_server_addr = " + res.hall_server_addr,true,
                    "重新登录", () => { ChangeAccount(); },true, "正式登录", 
                    () =>
                    {
                        AssetBundle.UnloadAllAssetBundles(false);
                        SceneManager.LoadScene("GameCore");
                    },false, false, false);
            }

            LoginRecord.LastLoginType = LoginRecord.CurrentLoginType;
            
            return true;
        }

        public override StateResult Finish(IAppContext context, float time)
        {

            return null;
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

        private string GetVisitorUsername(IAppContext context)
        {
            var ios = context.GetIosSDK();
            string visitorUsername = null;
#if UNITY_IOS // 如果是IOS，则尝试从keyChain中读取一下。
            visitorUsername = ios.GetUsername();
#endif
#if UNITY_ANDROID // TODO 从安卓取出visitorUsername。
#endif
            return visitorUsername;
        }

        #region 清空VisitorUsername

        private void ClearVisitorUsername(IAppContext context)
        {
            var ios = context.GetIosSDK();
            LoginRecord.LastVisitorUsername = null;

#if UNITY_IOS
            ios.ResetUsername();
#endif
        }

        #endregion
    }
}