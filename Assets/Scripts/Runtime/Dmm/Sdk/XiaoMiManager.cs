using System;
using Dmm.Data;
using Dmm.Dialog;
using Dmm.Log;
using UnityEngine;
using Zenject;

namespace Dmm.Sdk
{
    public class XiaoMiManager : MonoBehaviour
    {
        #region Inject

        private AndroidSDK _android;

        private IDialogManager _dialogManager;

        private ConfigHolder _configHolder;

        [Inject]
        public void Initialize(
            AndroidSDK android,
            IDialogManager dialogManager,
            ConfigHolder configHolder)
        {
            _android = android;
            _dialogManager = dialogManager;
            _configHolder = configHolder;
        }

        #endregion

        public void LoginResult(string result)
        {
            MyLog.InfoWithFrame("xiaomi", "login result: " + result);
            if (string.IsNullOrEmpty(result))
            {
                _dialogManager.ShowToast("小米账户登陆失败\n请点击\"切换账号\"按钮重试", 3, true);
                return;
            }

            MiLoginResult res = null;
            try
            {
                res = JsonUtility.FromJson<MiLoginResult>(result);
            }
            catch (Exception e)
            {
            }

            if (res == null)
            {
                _dialogManager.ShowToast("小米账户登陆失败\n请点击\"切换账号\"按钮重试", 3, true);
                return;
            }

            LoginResultData = res;

            if (res.result == MiLoginResult.SUCCESS)
            {
                LoginRecord.LastLoginType = LoginRecord.XiaoMi;
                LoginRecord.LastUsername = res.uid;
                LoginRecord.LastNickname = res.nickname;
                LoginRecord.SaveAll();
            }
            else
            {
                switch (res.result)
                {
                    case MiLoginResult.CANCEL:
                        _dialogManager.ShowToast("玩家取消登陆", 2);
                        break;

                    case MiLoginResult.EXECUTING:
                        _dialogManager.ShowToast("正在登陆", 2);
                        break;

                    default:
                        _dialogManager.ShowToast("登陆失败\n请点击\"切换账号\"按钮重试", 2, true);
                        break;
                }
            }
        }

        public void Init()
        {
#if UNITY_ANDROID
            if (!_configHolder.XiaoMiMode)
                return;

            _android.MiInit();
#endif
        }

        public void Login()
        {
            LoginResultData = null;

#if UNITY_ANDROID
            if (!_configHolder.XiaoMiMode)
                return;

            _android.MiLogin();
#endif
        }

        public MiLoginResult LoginResultData { get; private set; }
    }
}