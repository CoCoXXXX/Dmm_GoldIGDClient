using System;
using System.Collections.Generic;
using anysdk;
using Dmm.Data;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Network;
using Dmm.Sdk;
using UnityEngine;
using Zenject;

namespace Dmm.AnySdk
{
    public class AnySDKManager : MonoBehaviour, IAnySDKManager
    {
        #region Inject

        private ConfigHolder _configHolder;

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private AndroidSDK _android;

        private IosSDK _ios;

        [Inject]
        public void Initialize(
            ConfigHolder configHolder,
            AndroidSDK android,
            IosSDK ios,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI)
        {
            _configHolder = configHolder;
            _android = android;
            _ios = ios;
            _remoteAPI = remoteAPI;
            _dialogManager = dialogManager;
        }

        #endregion

        public void Init()
        {
            var appKey = _configHolder.AnySDKAppKey;
            var appSecret = _configHolder.AnySDKAppSecret;
            var privateKey = _configHolder.AnySDKPrivateKey;
            var oauthLoginServer = _configHolder.OauthLoginServer;

            AnySDK.getInstance().init(appKey, appSecret, privateKey, oauthLoginServer);
            AnySDKUser.getInstance().setListener(this, "UserExternalCall");
            AnySDKIAP.getInstance().setListener(this, "IAPExternalCall");
        }

        #region AnySDKUser

        public void Login()
        {
            MyLog.InfoWithFrame(name, "AnySDKManager any sdk login");
            AnySDKUser.getInstance().login();
        }

        public void Login(Dictionary<string, string> data)
        {
            AnySDKUser.getInstance().login(data);
        }

        public void UserExternalCall(string msg)
        {
            MyLog.InfoWithFrame(name, "UserExternalCall(" + msg + ")");
            var dic = AnySDKUtil.stringToDictionary(msg);
            var code = Convert.ToInt32(dic["code"]);
            var result = dic["msg"];
            MyLog.InfoWithFrame(name, "UserExternalCall result is  (" + result + ")");
            switch (code)
            {
                case (int) UserActionResultCode.kInitSuccess: //初始化SDK成功回调
                    //login方法需要在初始化成功之后调用
                    break;
                case (int) UserActionResultCode.kInitFail: //初始化SDK失或者退出游戏败回调
                    //尝试重新初始化，
                    break;
                case (int) UserActionResultCode.kLoginSuccess: //登陆成功回调
                    //可使用getUserID()获取用户ID
                    break;
                case (int) UserActionResultCode.kLoginNetworkError: //登陆网络出错回调
                case (int) UserActionResultCode.kLoginCancel: //登陆取消回调
                case (int) UserActionResultCode.kLoginFail: //登陆失败回调
                    break;
                case (int) UserActionResultCode.kLogoutSuccess: //登出成功回调
                    //一般可以做初始化游戏，并且重新调用登录接口操作
                    break;
                case (int) UserActionResultCode.kLogoutFail: //登出失败回调
                    break;
                case (int) UserActionResultCode.kPlatformEnter: //平台中心进入回调
                    break;
                case (int) UserActionResultCode.kPlatformBack: //平台中心退出回调
                    break;
                case (int) UserActionResultCode.kPausePage: //暂停界面回调
                    break;
                case (int) UserActionResultCode.kExitPage: //退出游戏回调
                    //参考下方退出界面文档
                    break;
                case (int) UserActionResultCode.kAntiAddictionQuery: //防沉迷查询回调
                    break;
                case (int) UserActionResultCode.kRealNameRegister: //实名注册回调
                    break;
                case (int) UserActionResultCode.kAccountSwitchSuccess: //切换账号成功回调
                    //一般可以做重新获取用户ID，和初始化游戏操作
                    break;
                case (int) UserActionResultCode.kAccountSwitchFail: //切换账号失败回调
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region AnySDKIAP

        public void IAPExternalCall(string msg)
        {
            MyLog.InfoWithFrame(name, "IAPExternalCall(" + msg + ")");
            var dic = AnySDKUtil.stringToDictionary(msg);
            var code = Convert.ToInt32(dic["code"]);
            var result = dic["msg"];

            switch (code)
            {
                case (int) PayResultCode.kPaySuccess: //支付成功回调
                    break;
                case (int) PayResultCode.kPayFail: //支付失败回调
                    break;
                case (int) PayResultCode.kPayCancel: //支付取消回调
                    break;
                case (int) PayResultCode.kPayNetworkError: //支付超时回调
                    break;
                case (int) PayResultCode.kPayProductionInforIncomplete: //支付信息不完整
                    break;
                /**
                * 新增加:正在进行中回调
                * 支付过程中若 SDK 没有回调结果，就认为支付正在进行中
                * 游戏开发商可让玩家去判断是否需要等待，若不等待则进行下一次的支付
                */
                case (int) PayResultCode.kPayNowPaying:
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}