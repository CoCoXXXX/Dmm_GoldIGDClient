using com.morln.game.gd.command;
using Dmm.AnySdk;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.PIP;
using Dmm.Res;
using Dmm.WeChat;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Login
{
    public class LoginModeWindow : MonoBehaviour
    {
        #region Inject

        public class Factory : PrefabFactory<LoginModeWindow>
        {
        }

        private IWeChatManager _wechatManager;

        private IDialogManager _dialogManager;

        private IAppController _appController;

        private IAnySDKManager _anySdkManager;

        private ConfigHolder _config;

        private IPIPLogic _pip;

        private IDataContainer<VersionResult> _versionResult;

        private IDataContainer<DownloadAssetBundleInfo> _downloadAssetBundleInfo;

        [Inject]
        public void Inject(
            IDialogManager dialogManager,
            IDataRepository dataRepository,
            IAppController appController,
            IWeChatManager wechatManager,
            ConfigHolder configHolder,
            IAnySDKManager anySdkManager,
            IPIPLogic pipLogic)
        {
            _dialogManager = dialogManager;
            _wechatManager = wechatManager;
            _appController = appController;
            _config = configHolder;
            _pip = pipLogic;
            _anySdkManager = anySdkManager;

            _versionResult = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            _downloadAssetBundleInfo =
                dataRepository.GetContainer<DownloadAssetBundleInfo>(DataKey.DownloadAssetBundleInfo);
        }

        #endregion

        #region 组件

        public Text LoginText;

        public GameObject BottomBar;

        public GameObject ModeBtnGroup;

        public Toggle UserAggreement;

        public Button VisitorBtn;

        public Button WechatBtn;

        public Button AccountBtn;

        public Text VersionTxt;

        public GameObject DownloadProcess;

        public Text DownloadTips;

        public Text ProcessText;

        public Slider ProcessSlider;

        #endregion

        #region 刷新内容

        private void OnEnable()
        {
            var waiting = LoginRecord.CurrentLoginType != LoginRecord.NoLogin;

            BottomBar.SetActive(!waiting);
            LoginText.gameObject.SetActive(false);
            var version = _config.VersionTxt;
            if (!string.IsNullOrEmpty(version))
            {
                VersionTxt.text = "当前版本：" + version;
            }

            if (ModeBtnGroup.activeSelf)
            {
                ModeBtnGroup.SetActive(false);
            }
        }

        private void Update()
        {
            var clientVersion = _versionResult.Read();
            var downloadAssetBundleInfo = _downloadAssetBundleInfo.Read();

            DownloadProcess.SetActive(downloadAssetBundleInfo != null);
            if (downloadAssetBundleInfo != null)
            {
                switch (downloadAssetBundleInfo.LoadType)
                {
                    case DownloadAssetBundleInfo.DownloadType.FirstBuild:
                        DownloadTips.text = "正在为您解压资源 （此过程不下消耗流量）";
                        break;
                    case DownloadAssetBundleInfo.DownloadType.Download:
                        DownloadTips.text = "正在为您下载最新资源 （建议在WiFi环境下载）";
                        break;
                    default:
                        break;
                }

                ProcessSlider.minValue = 0;
                ProcessSlider.maxValue = downloadAssetBundleInfo.TotalCount;
                ProcessSlider.value = downloadAssetBundleInfo.CompleteCount;
                ProcessText.text = downloadAssetBundleInfo.CompleteCount + "/"
                                   + downloadAssetBundleInfo.TotalCount;

                LoginText.gameObject.SetActive(false);
                return;
            }

            var clientVersionOk = clientVersion != null && clientVersion.result == ResultCode.OK;

            var waiting = !clientVersionOk;

            if (clientVersionOk)
            {
                waiting = LoginRecord.CurrentLoginType != LoginRecord.NoLogin;
            }

            if (ModeBtnGroup.activeSelf == waiting)
            {
                ModeBtnGroup.SetActive(!waiting);
            }

            if (BottomBar.activeSelf == waiting)
            {
                BottomBar.SetActive(!waiting);
            }

            bool personalInfoEnable = false;

            if (clientVersion == null)
            {
                personalInfoEnable = false;
            }
            else
            {
                if (clientVersion.result != ResultCode.OK)
                {
                    personalInfoEnable = false;
                }
                else
                {
                    var config = clientVersion.release_config;
                    if (config == null)
                    {
                        personalInfoEnable = false;
                    }
                    else
                    {
                        personalInfoEnable = config.enable_personal_info;
                    }
                }
            }

            var enableWechat = personalInfoEnable;
#if UNITY_IOS
            if (!_wechatManager.IsWechatInstalled())
            {
                enableWechat = false;
            }
#endif
            if (WechatBtn.gameObject.activeSelf != enableWechat)
            {
                WechatBtn.gameObject.SetActive(enableWechat);
            }

            if (LoginText.gameObject.activeSelf != waiting)
            {
                LoginText.gameObject.SetActive(waiting);
            }

            if (waiting)
            {
                LoginText.text = "正在登陆中";
                int count = ((int) Time.time) % 6 + 1;
                for (int i = 0; i < count; i++)
                {
                    LoginText.text += ".";
                }
            }

            VisitorBtn.interactable = UserAggreement.isOn;
            WechatBtn.interactable = UserAggreement.isOn;
            AccountBtn.interactable = UserAggreement.isOn;

            CheckAndroidBackKey();
        }

        #endregion

        #region Function

        public void VisitorLogin()
        {
            LoginRecord.CurrentLoginType = LoginRecord.Visitor;
            LoginRecord.SaveAll();
        }

        public void WechatLogin()
        {
            LoginRecord.CurrentLoginType = LoginRecord.Wechat;
            LoginRecord.RemoveAuthCode();
            LoginRecord.RemoveOpenId();
            LoginRecord.SaveAll();
        }

        public void AccountLogin()
        {
            _dialogManager.ShowDialog<LoginPanel>(DialogName.LoginPanel, true, true);
        }

        public void ShowBillboard()
        {
            _dialogManager.ShowDialog<BillboardPanel>(DialogName.BillboardDialog, true, true);
        }

        public void ShowUserAgreement()
        {
            _dialogManager.ShowDialog<UserAgreementDialog>(DialogName.UserAgreementDialog, true, true);
        }

        public void GoToSingleMode()
        {
            _appController.GoToSingleGame();
        }

        #endregion

        private AlertBox _quitSingleDialog;

        private void CheckAndroidBackKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_quitSingleDialog == null)
                {
                    _quitSingleDialog = _dialogManager.ShowConfirmBox("是否退出游戏？",
                        true, "退出", () => { Application.Quit(); },
                        true, "继续", null, true, true, true);
                }
            }
        }
    }
}