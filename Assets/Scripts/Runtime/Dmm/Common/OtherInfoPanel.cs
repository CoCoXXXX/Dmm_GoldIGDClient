using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Report;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class OtherInfoPanel : MyDialog
    {
        public NicknameGroup Nickname;

        public Text Level;

        public Text WinRate;

        public CurrencyValue Money;

        public Text Description;

        public Button AddFriendBtn;

        public Button ReportBtn;

        public GameObject InteractionDescriptionGroup;

        public Text InteractionDescription;

        public GameObject InteractionBtnGroup;

        private string _username;

        private User _data;

        private IDataContainer<SFriendListResult> _friendtListResult;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        private IDataContainer<string> _interactionDescription;

        private void OnEnable()
        {
            _friendtListResult =
                GetDataRepository().GetContainer<SFriendListResult>(DataKey.SFriendListResult);
            _featureSwitch = GetDataRepository().GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
            _interactionDescription =
                GetDataRepository().GetContainer<string>(DataKey.InteractionDescription);
        }

        public void ApplyData(User data, bool enableAddFriend, bool enableInteraction)
        {
            if (data == null)
            {
                Clear();
                return;
            }

            _data = data;
            _username = data.username;

            Nickname.SetData(data);

            var title = LevelTitle.TitleOf(data.level);
            title = string.IsNullOrEmpty(title) ? "" : title;
            if (!string.IsNullOrEmpty(title))
                title = string.Format("({0})", title);
            else
                title = "";

            Level.text = string.Format("{0}级{1}", data.level, title);

            Money.SetCurrency(data.money, CurrencyType.GOLDEN_EGG);

            // 将胜率改成显示场次。
            // var rate = data.round_count > 0 ? (float)data.win_count/data.round_count : 0;
            var winCount = data.win_count;
            if (winCount < 0)
                winCount = 0;

            var loseCount = data.round_count - data.win_count;
            if (loseCount < 0)
                loseCount = 0;

            WinRate.text = string.Format("胜{0}场，败{1}场", winCount, loseCount);

            // Description.text = data.description;

            // 只有在需要添加好友并且检测是不是好友的情况下才显示添加好友按钮。
            var friendListResult = _friendtListResult.Read();
            var show = enableAddFriend && !GameUtil.IsFriend(friendListResult, _username);

            if (AddFriendBtn.gameObject.activeSelf != show)
                AddFriendBtn.gameObject.SetActive(show);

            InitInteraction(enableInteraction);
        }

        public void ApplyData(UserBase data, bool enableAddFriend, bool enableInteraction, bool showReportBtn = true)
        {
            if (data == null)
            {
                Clear();
                return;
            }

            if (ReportBtn.gameObject.activeSelf != showReportBtn)
            {
                ReportBtn.gameObject.SetActive(showReportBtn);
            }

            _username = data.username;

            Nickname.SetData(data.nickname, data.vip);

            var title = LevelTitle.TitleOf(data.level);
            if (!string.IsNullOrEmpty(title))
                title = string.Format("({0})", title);
            else
                title = "";

            Level.text = string.Format("{0}级{1}", data.level, title);

            Money.SetCurrency(data.local_money, CurrencyType.GOLDEN_EGG);

            var rate = data.round_count > 0 ? (float) data.win_count / data.round_count : 0;
            WinRate.text = DataUtil.FormatWinRate(rate);

            // Description.text = data.description;

            // 只有在需要添加好友并且检测是不是好友的情况下才显示添加好友按钮。
            var friendListResult = _friendtListResult.Read();
            var show = enableAddFriend && !GameUtil.IsFriend(friendListResult, _username);

            if (AddFriendBtn.gameObject.activeSelf != show)
            {
                AddFriendBtn.gameObject.SetActive(show);
            }

            InitInteraction(enableInteraction);
        }

        private void InitInteraction(bool enableInteraction)
        {
            var featureSwitch = _featureSwitch.Read();
            var isEnableInteraction = true;
            if (featureSwitch != null)
            {
                isEnableInteraction = featureSwitch.interaction;
            }

            if (!enableInteraction || !isEnableInteraction)
            {
                if (InteractionDescriptionGroup.activeSelf)
                {
                    InteractionDescriptionGroup.SetActive(false);
                }

                if (InteractionBtnGroup.activeSelf)
                {
                    InteractionBtnGroup.SetActive(false);
                }

                return;
            }

            var hallDataInteractionDescription = _interactionDescription.Read();
            var enableInterDesc = !string.IsNullOrEmpty(hallDataInteractionDescription);

            if (InteractionDescriptionGroup.activeSelf != enableInterDesc)
                InteractionDescriptionGroup.SetActive(enableInterDesc);

            if (enableInterDesc)
                InteractionDescription.text = hallDataInteractionDescription;

            if (!InteractionBtnGroup.activeSelf)
                InteractionBtnGroup.SetActive(true);
        }

        public void Clear()
        {
            Nickname.Clear();
            Level.text = "";
            WinRate.text = "";
            Money.Clear();
            // Description.text = "";
        }

        public void AddFriend()
        {
            if (!string.IsNullOrEmpty(_username))
            {
                var remote = GetRemoteAPI();
                remote.AddFriend(_username);

                var dialogManager = GetDialogManager();
                dialogManager.ShowToast("已向对方发送了好友请求\n等待对方接受", 3);

                if (AddFriendBtn && AddFriendBtn.gameObject.activeSelf)
                {
                    AddFriendBtn.gameObject.SetActive(false);
                }
            }
        }

        public void Report()
        {
            var dialogManager = GetDialogManager();
            if (_data == null)
            {
                dialogManager.ShowToast("举报玩家失败", 2, true);
                return;
            }

            dialogManager.ShowDialog<ReportDialog>(DialogName.ReportDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(_data);
                    dialog.Show();
                }
            );
        }

        public void ThrowEgg()
        {
            var remote = GetRemoteAPI();
            remote.DoInteraction(InteractionCode.Egg, _username);
            Hide();
        }

        public void SendFlower()
        {
            var remote = GetRemoteAPI();
            remote.DoInteraction(InteractionCode.Flower, _username);
            Hide();
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}