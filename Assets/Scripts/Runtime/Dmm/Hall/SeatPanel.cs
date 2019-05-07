using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Chat;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.MoreFunction;
using Dmm.Network;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    /// <summary>
    /// 选座界面。
    /// </summary>
    public class SeatPanel : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private ChatBubble.Factory _chatBubbleFactory;

        private IDataContainer<Room> _room;

        private IDataContainer<TableUserData> _tableuserData;

        private IDataContainer<Table> _table;

        private IDataContainer<User> _myUser;

        private IDataContainer<BKickOutCounter> _bKickOutCounter;

        private IDataContainer<BTextMsg> _bTextMsg;

        private IDataContainer<BJianMeng> _bJianMeng;

        private IDataContainer<List<JianMengItem>> _jianMengList;

        private IDataContainer<bool> _shieldChat;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        [Inject]
        public void Initialize(
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            IDataRepository dataRepository,
            ChatBubble.Factory chatBubbleFactory)
        {
            _dialogManager = dialogManager;
            _chatBubbleFactory = chatBubbleFactory;
            _remoteAPI = remoteAPI;
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _tableuserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _bKickOutCounter = dataRepository.GetContainer<BKickOutCounter>(DataKey.BKickOutCounter);
            _bTextMsg = dataRepository.GetContainer<BTextMsg>(DataKey.BTextMsg);
            _bJianMeng = dataRepository.GetContainer<BJianMeng>(DataKey.BJianMeng);
            _jianMengList = dataRepository.GetContainer<List<JianMengItem>>(DataKey.JianMengItemList);
            _shieldChat = dataRepository.GetContainer<bool>(DataKey.ShieldChat);
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        #endregion

        #region 座位

        public PlayerSeat NorthSeat;

        public PlayerSeat WestSeat;

        public PlayerSeat SouthSeat;

        public PlayerSeat EastSeat;

        #endregion

        #region 功能按钮

        /// <summary>
        /// 准备按钮。
        /// </summary>
        public Button ReadyButton;

        /// <summary>
        /// 聊天按钮
        /// </summary>
        public Button ChatBtn;

        /// <summary>
        /// 换桌按钮。
        /// </summary>
        public GameObject ChangeTableBtnGroup;

        public GameObject NormalHostGroup;

        public GameObject TTZHostGroup;

        public GameObject RaceTTZHostGroup;

        public void Ready()
        {
            _remoteAPI.Ready();
        }

        public void ChangeTable()
        {
            var room = _room.Read();
            var currenRoomType = room == null ? RoomType.Match : room.type;
            if (currenRoomType == RoomType.Match)
            {
                _remoteAPI.MatchTable(false);
            }
        }

        public void Back()
        {
            var room = _room.Read();
            var currenRoomType = room == null ? RoomType.Match : room.type;
            if (currenRoomType == RoomType.Normal)
            {
                _remoteAPI.LeaveTable(false);
            }
            else
            {
                _remoteAPI.LeaveRoom(false);
            }

            MayShowRankMeDialog();
        }

        private void MayShowRankMeDialog()
        {
#if UNITY_IOS
            var featureSwitch = _featureSwitch.Read();
            var isEnableRating = featureSwitch.rating;
            if (!isEnableRating)
            {
                return;
            }

            var shown = PrefsUtil.GetBool(RankMeDialog.RankMeShowKey, false);
            if (shown)
            {
                return;
            }

            var time = PrefsUtil.GetLong(RankMeDialog.RankMeShowTimeKey, 0);
            var nowTime = DateTime.Now.CurrentTimeMillis();

            // 改回一天。
            var dayTime = 1000 * 3600 * 24;
            if (nowTime - time > dayTime)
            {
                _dialogManager.ShowDialog<RankMeDialog>(DialogName.RankMeDialog);
            }
#endif
        }

        #endregion

        #region 刷新内容

        public void OnEnable()
        {
            var table = _table.Read();
            var enableChat = table != null && table.enable_chat;
            ChatBtn.interactable = enableChat;
            RefreshContent();
        }

        public void Update()
        {
            RefreshContent();
            RefreshKickOutCounter();

            RefreshChat();
            RefreshJianMeng();

            CheckBackKey();
        }

        public float RefreshTime { get; private set; }

        private void RefreshContent()
        {
            if (RefreshTime >= _tableuserData.Timestamp)
            {
                return;
            }

            RefreshTime = _table.Timestamp;

            var table = _table.Read();
            if (table == null)
            {
                return;
            }
            var tableUser = _tableuserData.Read();

            UpdateSeat(SouthSeat, tableUser.GetUserAtSeat(0));
            UpdateSeat(EastSeat, tableUser.GetUserAtSeat(1));
            UpdateSeat(NorthSeat, tableUser.GetUserAtSeat(2));
            UpdateSeat(WestSeat, tableUser.GetUserAtSeat(3));

            var ready = tableUser.SelfReady();
            if (ReadyButton.interactable == ready)
            {
                ReadyButton.interactable = !ready;
            }

            var normal = table.type == RoomType.Normal;
            if (ChangeTableBtnGroup.activeSelf == normal)
            {
                ChangeTableBtnGroup.SetActive(!normal);
            }

            var gameType = table.game_type;
            switch (gameType)
            {
                case TableGameType.NULL:
                    TTZHostGroup.SetActive(false);
                    NormalHostGroup.SetActive(false);
                    RaceTTZHostGroup.SetActive(false);
                    break;
                case TableGameType.NORMAL:
                    TTZHostGroup.SetActive(false);
                    NormalHostGroup.SetActive(true);
                    RaceTTZHostGroup.SetActive(false);
                    break;
                case TableGameType.TTZ:
                    TTZHostGroup.SetActive(true);
                    NormalHostGroup.SetActive(false);
                    RaceTTZHostGroup.SetActive(false);
                    break;
                case TableGameType.RACE_TTZ:
                    TTZHostGroup.SetActive(false);
                    NormalHostGroup.SetActive(false);
                    RaceTTZHostGroup.SetActive(true);
                    break;

                default:
                    TTZHostGroup.SetActive(false);
                    NormalHostGroup.SetActive(false);
                    RaceTTZHostGroup.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// 更新座位的内容。
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="data"></param>
        private void UpdateSeat(PlayerSeat seat, User data)
        {
            if (!seat)
                return;

            seat.ApplyData(data);
        }

        public void OnSeatShowFuncGroup(PlayerSeat seat)
        {
            if (!seat)
                return;

            SouthSeat.ShowFuncGroup(SouthSeat == seat);
            WestSeat.ShowFuncGroup(WestSeat == seat);
            NorthSeat.ShowFuncGroup(NorthSeat == seat);
            EastSeat.ShowFuncGroup(EastSeat == seat);
        }

        #endregion

        #region KickOutCounter

        public float KickOutCounterRefreshTime { get; private set; }

        private void RefreshKickOutCounter()
        {
            if (KickOutCounterRefreshTime >= _bKickOutCounter.Timestamp)
            {
                return;
            }

            KickOutCounterRefreshTime = _bKickOutCounter.Timestamp;

            var msg = _bKickOutCounter.Read();
            if (msg == null)
            {
                SouthSeat.ShowKickOutCounter(false);
                EastSeat.ShowKickOutCounter(false);
                NorthSeat.ShowKickOutCounter(false);
                WestSeat.ShowKickOutCounter(false);
                return;
            }

            switch (msg.seat)
            {
                case 0:
                    SouthSeat.ShowKickOutCounter(msg.start_or_stop == 1, _bKickOutCounter.Timestamp,
                        msg.left_time);
                    break;

                case 1:
                    EastSeat.ShowKickOutCounter(msg.start_or_stop == 1, _bKickOutCounter.Timestamp,
                        msg.left_time);
                    break;

                case 2:
                    NorthSeat.ShowKickOutCounter(msg.start_or_stop == 1, _bKickOutCounter.Timestamp,
                        msg.left_time);
                    break;

                case 3:
                    WestSeat.ShowKickOutCounter(msg.start_or_stop == 1, _bKickOutCounter.Timestamp,
                        msg.left_time);
                    break;
            }
        }

        #endregion

        #region 聊天

        public RectTransform EastChatContainer;

        public RectTransform WestChatContainer;

        public RectTransform NorthChatContainer;

        public RectTransform SouthChatContainer;

        #region TextChat

        public float ChatRefreshTime { get; private set; }

        private void RefreshChat()
        {
            if (ChatRefreshTime >= _bTextMsg.Timestamp)
            {
                return;
            }

            ChatRefreshTime = _bTextMsg.Timestamp;

            var msg = _bTextMsg.Read();
            if (msg == null)
            {
                return;
            }

            var tableUser = _tableuserData.Read();
            var seat = tableUser.GetSeatOfUser(msg.from_username);
            if (seat == -1)
                // 不是当前桌子上的人发的消息，就直接忽略吧。
            {
                return;
            }

            if (string.IsNullOrEmpty(msg.content))
                // 聊天内容为空的时候，也直接忽略。
            {
                return;
            }

            RectTransform container = null;
            switch (seat)
            {
                case 0:
                    container = SouthChatContainer;
                    break;

                case 1:
                    container = EastChatContainer;
                    break;

                case 2:
                    container = NorthChatContainer;
                    break;

                case 3:
                    container = WestChatContainer;
                    break;
            }

            if (!container)
                return;

            // 初始化一个ChatBubble用来显示聊天的内容。
            var bubble = _chatBubbleFactory.Create();
            if (!bubble)
            {
                return;
            }

            bubble.transform.SetParent(container, false);
            bubble.transform.localPosition = Vector3.zero;

            if (Emoji.IsEmoji(msg.content))
            {
                bubble.SetEmoji(msg.content);
            }
            else
            {
                bubble.SetText(msg.content);
            }

            bubble.Show();

            // 清空当前数据。
            _bTextMsg.ClearAndInvalidate(0);
        }

        #endregion

        #region JianMeng

        public float JianMengRefreshTime { get; private set; }

        private void RefreshJianMeng()
        {
            if (JianMengRefreshTime >= _bJianMeng.Timestamp)
            {
                return;
            }

            JianMengRefreshTime = _bJianMeng.Timestamp;

            var msg = _bJianMeng.Read();
            if (msg == null)
            {
                return;
            }

            var tableUser = _tableuserData.Read();
            var myUser = _myUser.Read();
            var list = _jianMengList.Read();
            var data = FindJianMengItem(list, msg.cmd);
            if (data == null)
            {
                return;
            }

            if (myUser == null)
            {
                return;
            }
            if (_shieldChat.Read() &&
                msg.from_username != myUser.username)
            {
                return;
            }

            var seat = tableUser.GetSeatOfUser(msg.from_username);
            if (seat == -1)
                // 不是当前桌子上的人发的消息，就直接忽略吧。
            {
                return;
            }

            RectTransform container = null;
            switch (seat)
            {
                case 0:
                    container = SouthChatContainer;
                    break;

                case 1:
                    container = EastChatContainer;
                    break;

                case 2:
                    container = NorthChatContainer;
                    break;

                case 3:
                    container = WestChatContainer;
                    break;
            }

            if (!container)
            {
                return;
            }

            // 初始化一个ChatBubble用来显示聊天的内容。
            var bubble = _chatBubbleFactory.Create();
            if (!bubble)
            {
                return;
            }

            bubble.transform.SetParent(container, false);
            bubble.transform.localPosition = Vector3.zero;

            bubble.SetJianMeng(data);

            bubble.Show();

            // 清空当前数据。
            _bJianMeng.ClearNotInvalidate();
        }

        #endregion

        public void ShowChatPanel()
        {
            _dialogManager.ShowDialog<UIWindow>(DialogName.ChatPanel, true, true);
        }

        public void ShowMoreFunction()
        {
            _dialogManager.ShowDialog<MoreFunctionDialog>(DialogName.MoreFunctionPanel);
        }

        #endregion

        private void CheckBackKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
        }

        private JianMengItem FindJianMengItem(List<JianMengItem> list, string cmd)
        {
            if (list == null || string.IsNullOrEmpty(cmd))
            {
                return null;
            }

            foreach (var item in list)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.cmd == cmd)
                {
                    return item;
                }
            }

            return null;
        }
    }
}