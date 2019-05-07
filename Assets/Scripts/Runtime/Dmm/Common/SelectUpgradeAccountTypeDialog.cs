using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.WeChat;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class SelectUpgradeAccountTypeDialog : MyDialog
    {
        public Button BindingWechatBtn;

        public Button BindingUserNameBtn;

        public Text TipsTxt;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        private IDataContainer<WechatBindResult> _wechatBindResult;

        private void OnEnable()
        {
            _featureSwitch = GetDataRepository().GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
            _wechatBindResult = GetDataRepository().GetContainer<WechatBindResult>(DataKey.WechatBindResult);
        }

        private void Update()
        {
            RefreshWechatBindResult();
        }

        private User _user;

        public void ApplyData(User data)
        {
            if (data == null)
            {
                return;
            }

            _user = data;
        }

        public override void BeforeShow()
        {
            TipsTxt.text = "升级账号可以让您的账号更安全便捷！";

            // 非微信用户才能升级微信。
            var enableWechat = _user != null && _user.type != UserType.Wechat;
            var isEnablePersonalInfo = false;
            var featureSwitch = _featureSwitch.Read();
            if (featureSwitch != null)
            {
                isEnablePersonalInfo = featureSwitch.personal_info;
            }

            if (!isEnablePersonalInfo)
            {
                enableWechat = false;
            }

            if (BindingWechatBtn.gameObject.activeSelf != enableWechat)
            {
                BindingWechatBtn.gameObject.SetActive(enableWechat);
            }

            // 游客账户可以升级账户。
            var enableBindUsername = _user != null && _user.type == UserType.Visitor;
            if (BindingUserNameBtn.gameObject.activeSelf != enableBindUsername)
            {
                BindingUserNameBtn.gameObject.SetActive(enableBindUsername);
            }

            if (!enableWechat && !enableBindUsername)
            {
                TipsTxt.text = "您的账号无需升级";
            }

            _getAuthCode = false;
            _wechatBindResult.ClearAndInvalidate(0);
        }

        public void OnBindingUserNameBtnClicked()
        {
            GetDialogManager().ShowDialog<VisitorRegularizeDialog>(DialogName.VisitorRegularizePanel, false, false,
                (dialog) =>
                {
                    if (_user != null)
                    {
                        dialog.ApplyData(_user);
                        dialog.Show();
                    }
                });

            Exit();
        }

        public void OnBindingWechatBtnClick()
        {
            var dialogManager = GetDialogManager();
            dialogManager.ShowConfirmBox("是否把当前登陆的账号绑定到微信？\n绑定后成功可使用微信登陆此账号。",
                true, "确定", OnBindingWechatConfirmBtnClick, true, "取消",
                OnBindingWechatCancleBtnClick, true, false, false);
        }

        public void OnBindingWechatConfirmBtnClick()
        {
            GetAuthCode();
            GetAnalyticManager().Event("confirm_binding_wechat");
        }

        public void OnBindingWechatCancleBtnClick()
        {
            GetAnalyticManager().Event("canc_binding_wechat");
            Hide();
        }

        public void Exit()
        {
            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }

        private void GetAuthCode()
        {
            var deviceId = GetDeviceId();
            MyLog.InfoWithFrame(name, string.Format("Start GetAuthCode deviceId: {0}", deviceId));
            GetDialogManager().ShowWaitingDialog(true);
            GetTaskManager().ExecuteTask(CheckIsGetAuthCode, GetAuthCodeTimeOut);
            GetWeChatManager().Auth(deviceId);
        }

        private bool CheckIsGetAuthCode()
        {
            var wechatManager = GetWeChatManager();
            var dialogManager = GetDialogManager();
            var authResult = wechatManager.GetWxAuthResult();
            if (authResult == null)
            {
                return false;
            }

            dialogManager.ShowWaitingDialog(false);
            if (authResult.Result != WxAuthResult.Ok)
            {
                if (!string.IsNullOrEmpty(authResult.ErrMsg))
                {
                    dialogManager.ShowToast(authResult.ErrMsg, 2);
                }
                else
                {
                    dialogManager.ShowToast("绑定微信失败，请重新绑定！", 2);
                }

                Exit();
            }
            else
            {
                _getAuthCode = true;
                DoWechatBind(authResult.Code);
            }

            return true;
        }

        private void GetAuthCodeTimeOut()
        {
            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);
            dialogManager.ShowConfirmBox("绑定失败，请检查您的微信是否正常安装或者被禁用\n如有疑问请与客服联系");
            Exit();
        }

        private void DoWechatBind(string authCode)
        {
            MyLog.InfoWithFrame(name, string.Format("Start WechatBind authCode: {0}", authCode));
            GetDialogManager().ShowWaitingDialog(true);
            GetRemoteAPI().WechatBind(authCode);
        }

        private float _wechatBindResultRefreshTime;
        private bool _getAuthCode;

        private void RefreshWechatBindResult()
        {
            if (!_getAuthCode)
            {
                return;
            }

            if (_wechatBindResultRefreshTime >= _wechatBindResult.Timestamp)
            {
                return;
            }

            _wechatBindResultRefreshTime = _wechatBindResult.Timestamp;

            var res = _wechatBindResult.Read();
            if (res.res == null)
            {
                return;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);
            if (res.res.code == ResultCode.OK)
            {
                // 转正成功。
                GetAppController().ClearAppStateData();
                dialogManager.ShowConfirmBox(
                    "恭喜您绑定微信成功！",
                    true, "马上登陆", () =>
                    {
                        LoginRecord.LastLoginType = LoginRecord.Wechat;
                        LoginRecord.SaveOpenId(res.open_id);
                        LoginRecord.SaveAll();

                        GetNetworkManager().InitLogin();
                    },
                    false, null, null,
                    true, false, false);

                GetRemoteAPI().RequestUserInfo();
            }
            else
            {
                var msg = res.res.msg;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowConfirmBox(msg);
                }
                else
                {
                    dialogManager.ShowToast("绑定微信失败！", 2, true);
                }
            }

            Exit();
        }

        private string GetDeviceId()
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier;
#if UNITY_IOS
            var ios = GetIosSDK();
            deviceId = ios.GetDeviceId();
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                ios.SaveDeviceId(deviceId);
                MyLog.InfoWithFrame(name, string.Format("save deviceId:{0}", deviceId));
            }
            else
            {
                MyLog.InfoWithFrame(name, string.Format("use deviceId:{0}", deviceId));
            }
#endif
#if UNITY_ANDROID // TODO 从安卓取出deviceId。
#endif
            return deviceId;
        }
    }
}