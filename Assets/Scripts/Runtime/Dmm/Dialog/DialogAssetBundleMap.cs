using System.Collections.Generic;

namespace Dmm.Dialog
{
    /// <summary>
    /// 对话框与AssetBundle文件的映射。
    /// </summary>
    public class DialogAssetBundleMap
    {
        /// <summary>
        /// DialogName --> AssetBundleName
        /// </summary>
        private static readonly Dictionary<string, string> _map = new Dictionary<string, string>();

        static DialogAssetBundleMap()
        {
            _map.Add(DialogName.ShopPanel, "shop-dialog");
            _map.Add(DialogName.ChatPanel, "chat-dialog");
            _map.Add(DialogName.CheckinDialog, "checkin-dialog");
            _map.Add(DialogName.BillboardDialog, "billboard-dialog");
            _map.Add(DialogName.DisconnectedDialog, "disconnet-dialog");
            _map.Add(DialogName.BeenReplacedDialog, "been-replace-dialog");
            _map.Add(DialogName.ChangeNicknameDialog, "more-function");
            _map.Add(DialogName.ChangeSexDialog, "more-function");
            _map.Add(DialogName.ResetWinRateDialog, "more-function");
            _map.Add(DialogName.MoreFunctionPanel, "more-function");
            _map.Add(DialogName.ChangePasswordDialog, "more-function");
            _map.Add(DialogName.SettingDialog, "setting-dialog");
            _map.Add(DialogName.RankMeDialog, "more-function");
            _map.Add(DialogName.HelpDialog, "help-dialog");
            _map.Add(DialogName.UserTaskTipDialog, "user-task");
            _map.Add(DialogName.LoginPanel, "login-panel");
            _map.Add(DialogName.VisitorRegularizePanel, "more-function");
            _map.Add(DialogName.PayChannelPanel, "pay-channel-dialog");
            _map.Add(DialogName.FriendRequestDialog, "friend-request-dialog");
            _map.Add(DialogName.ChooseRoomFailDialog, "choose-room-fail-dialog");
            _map.Add(DialogName.GetAwardDialog, "getaward-dialog");
            _map.Add(DialogName.PunishTipDialog, "punish-tip-dialog");
            _map.Add(DialogName.PushItemDialog, "pushitem-dialog");
            _map.Add(DialogName.YuanBaoRecordDialog, "yuanbao-record-dialog");
            _map.Add(DialogName.InputExchangeCountDialog, "input");
            _map.Add(DialogName.HintItemDialog, "hintitem-dialog");
            _map.Add(DialogName.FindFriendDialog, "find-friend-dialog");
            _map.Add(DialogName.PlayerInfoPanel, "player-info-dialog");
            _map.Add(DialogName.RegisterDialog, "register-dialog");
            _map.Add(DialogName.OtherInfoPanel, "other-info-dialog");
            _map.Add(DialogName.VisitorChooseNicknameDialog, "visitor-choose-nickname-dialog");
            _map.Add(DialogName.SelectUpgradeAccountTypeDialog, "more-function");
            _map.Add(DialogName.UserAgreementDialog, "user-agreement-dialog");
            _map.Add(DialogName.ApkDownloadDialog, "apk-download-dialog");
            _map.Add(DialogName.WelfareDescriptionDialog, "welfare-description-dialog");
            _map.Add(DialogName.ReportDialog, "report-dialog");
            _map.Add(DialogName.TTZStartInfoDialog, "ttz-start-info-dialog");
            _map.Add(DialogName.RaceAwardsDialog, "race");
            _map.Add(DialogName.RaceIntroduceDialog, "race");
            _map.Add(DialogName.SelectPokerGuideDialog, "select-poker-guide-dialog");
            _map.Add(DialogName.UserTaskDialog, "user-task");
            _map.Add(DialogName.WeChatShareDialog, "wechat-share-dialog");
            _map.Add(DialogName.ItemDetailDialog, "item-detail-dialog");
            _map.Add(DialogName.RealNameDialog, "real-name-dialog");

            //Test
            _map.Add(DialogName.SetPServerDialog, "set-pserver-dialog");
            _map.Add(DialogName.SetClientVersionDialog, "client-version-dialog");
        }

        public static string GetAssetBundleName(string dialogName)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return null;
            }

            if (!_map.ContainsKey(dialogName))
            {
                return null;
            }

            return _map[dialogName];
        }
    }
}