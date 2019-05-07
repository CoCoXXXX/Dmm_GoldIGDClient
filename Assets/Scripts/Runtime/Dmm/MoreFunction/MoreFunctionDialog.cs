using com.morln.game.gd.command;
using Dmm.Background;
using Dmm.Common;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.QuickTools;
using Dmm.Util;
using Dmm.WeChat;
using UnityEngine.UI;

namespace Dmm.MoreFunction
{
    public class MoreFunctionDialog : MyDialog
    {
        public Toggle BGMToggle;

        public Toggle EffectToggle;

        public Toggle BgToggle;

        public Toggle DanZhangToggle;

        public Button RankMeBtn;

        public Button UpgradeAccountBtn;

        public Button ChangeSexBtn;

        public Button WxShareBtn;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        private IDataContainer<User> _myUser;

        private IDataContainer<InGameConfig> _inGameConfig;

        public void OnEnable()
        {
            var dataRepository = GetDataRepository();
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _inGameConfig = dataRepository.GetContainer<InGameConfig>(DataKey.InGameConfig);
            InitComponents();
        }

        public void InitComponents()
        {
            if (BGMToggle)
            {
                BGMToggle.isOn = PrefsUtil.GetBool(PrefsKeys.BgmEnable, true);
            }

            if (EffectToggle)
            {
                EffectToggle.isOn = PrefsUtil.GetBool(PrefsKeys.EffectEnable, true);
            }

            if (BgToggle)
            {
                BgToggle.isOn = IsBgOn();
            }

            if (DanZhangToggle)
            {
                DanZhangToggle.isOn = PrefsUtil.GetBool(PrefsKeys.XuanDanZhangKey, false);
            }

            var featureSwitch = _featureSwitch.Read();
            var enableSex = featureSwitch != null && featureSwitch.personal_info;
            var enableWxShare = featureSwitch != null && featureSwitch.personal_info;

            // 除了ios平台，其他的平台上都不打开评价面板。
            var enableRating = false;
            // 不是微信用户的时候，可以升级账户。
            var user = _myUser.Read();
            var isWechatUser = user != null && user.type == UserType.Wechat;
            var upgradeAccountBtnEnable = !isWechatUser;
#if UNITY_IOS
            enableRating = featureSwitch != null && featureSwitch.rating;
#endif
#if UNITY_ANDROID // 在小米模式下，需要关闭游客转正的功能。
            var configHolder = GetConfigHolder();
            if (configHolder.XiaoMiMode)
            {
                upgradeAccountBtnEnable = false;
            }
#endif
            if (UpgradeAccountBtn.gameObject.activeSelf != upgradeAccountBtnEnable)
            {
                UpgradeAccountBtn.gameObject.SetActive(upgradeAccountBtnEnable);
            }

            if (RankMeBtn && RankMeBtn.gameObject.activeSelf != enableRating)
            {
                RankMeBtn.gameObject.SetActive(enableRating);
            }

            if (ChangeSexBtn && ChangeSexBtn.gameObject.activeSelf != enableSex)
            {
                ChangeSexBtn.gameObject.SetActive(enableSex);
            }

            if (WxShareBtn && WxShareBtn.gameObject.activeSelf != enableWxShare)
            {
                WxShareBtn.gameObject.SetActive(enableWxShare);
            }
        }

        public void OnBgmToggleChanged()
        {
            if (BGMToggle)
            {
                GetSoundController().SetBgmEnable(BGMToggle.isOn);
            }
        }

        public void OnEffectToggleChanged()
        {
            if (EffectToggle)
            {
                GetSoundController().SetEffectEnable(EffectToggle.isOn);
            }
        }

        public void OnBgToggleChanged()
        {
            if (BgToggle)
            {
                SetBgOn(BgToggle.isOn);
            }
        }

        public void OnDanZhangModeChange()
        {
            PrefsUtil.SetBool(PrefsKeys.XuanDanZhangKey, DanZhangToggle.isOn);
            PrefsUtil.Flush();
        }

        public void ShowPlayerInfo()
        {
            GetDialogManager().ShowDialog<PlayerInfoPanel>(DialogName.PlayerInfoPanel);
        }

        public void ChangePassword()
        {
            GetDialogManager().ShowDialog<ChangePasswordDialog>(DialogName.ChangePasswordDialog);
        }

        public void GoToRank()
        {
            GetDialogManager().ShowDialog<RankMeDialog>(DialogName.RankMeDialog);
        }

        public void RealName()
        {
            GetDialogManager().ShowDialog<RealNameDialog>(DialogName.RealNameDialog);
        }

        /// <summary>
        /// 微信分享。
        /// </summary>
        public void WxShare()
        {
            var inGameConfig = _inGameConfig.Read();
            var dialogManager = GetDialogManager();
            if (inGameConfig == null)
            {
                dialogManager.ShowToast("分享失败", 2, true);
                return;
            }

            dialogManager.ShowDialog<WeChatShareDialog>(DialogName.WeChatShareDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(
                        "分享下载",
                        inGameConfig.wx_invite_url,
                        null,
                        null,
                        inGameConfig.wx_invite_title,
                        inGameConfig.wx_invite_description,
                        null);
                    dialog.Show();
                });
        }

        public void UpgradeAccount()
        {
            var dialogManager = GetDialogManager();
            var user = _myUser.Read();
            if (user == null)
            {
                dialogManager.ShowToast("登陆之后才能升级！", 2);
                return;
            }

            if (user.type == UserType.Wechat)
            {
                dialogManager.ShowToast("您已经用微信登陆了", 2);
                return;
            }

            dialogManager.ShowDialog<SelectUpgradeAccountTypeDialog>(DialogName.SelectUpgradeAccountTypeDialog,
                false, false,
                (dialog) =>
                {
                    dialog.ApplyData(user);
                    dialog.Show();
                });
        }

        public void Logout()
        {
            GetDialogManager().ShowConfirmBox("是否退出当前登陆的账号？", true, "确定", () =>
                {
                    var network = GetNetworkManager();
                    network.Logout();
                    Hide();
                },
                true, "取消",
                null, true, false, false);
        }

        public void ChangeNickname()
        {
            GetDialogManager().ShowDialog<ChangeNicknameDialog>(DialogName.ChangeNicknameDialog);
        }

        public void ChangeSex()
        {
            GetDialogManager().ShowDialog<ChangeSexDialog>(DialogName.ChangeSexDialog);
        }

        public void ResetWinRate()
        {
            GetDialogManager().ShowDialog<ResetWinRateDialog>(DialogName.ResetWinRateDialog);
        }

        public bool IsBgOn()
        {
            return PrefsUtil.GetBool(BgConstant.EnableBgKey, true);
        }

        public void SetBgOn(bool on)
        {
            PrefsUtil.SetBool(BgConstant.EnableBgKey, on);
            PrefsUtil.Flush();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}