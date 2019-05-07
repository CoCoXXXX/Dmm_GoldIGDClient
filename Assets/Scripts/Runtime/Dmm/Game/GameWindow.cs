using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Analytic;
using Dmm.App;
using Dmm.Background;
using Dmm.Chat;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game.State;
using Dmm.Hall;
using Dmm.Network;
using Dmm.PokerLogic;
using Dmm.Res;
using Dmm.RoundEnd;
using Dmm.Shop;
using Dmm.Sound;
using Dmm.StateLogic;
using Dmm.Util;
using Dmm.WeChat;
using Dmm.Widget;
using Dmm.ZXing;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using PokerPattern = Dmm.PokerLogic.PokerPattern;

namespace Dmm.Game
{
    public class GameWindow : MonoBehaviour
    {
        #region Inject

        private IAppContext _context;

        private RemoteAPI _remoteAPI;

        private IAppController _app;

        private IWeChatManager _wechatManager;

        private IDialogManager _dialogManager;

        private IAnalyticManager _analyticManager;

        private ChatBubble.Factory _chatBubbleFactory;

        private ISoundController _soundController;

        private FlyItem.Factory _flyItemFactory;

        private IDataContainer<InGameConfig> _inGameConfig;

        private IStateLogic<GameWindow> _stateLogic;

        private IDataContainer<List<string>> _gameTipList;

        private IDataContainer<User> _myUser;

        private IDataContainer<Table> _currentTable;

        private IDataContainer<Room> _currentRoom;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<PlayingData> _playingData;

        private IDataContainer<BFanbei> _bFanBei;

        private IDataContainer<BTextMsg> _textMsg;

        private IDataContainer<BJianMeng> _jianMeng;

        private IDataContainer<List<JianMengItem>> _jianMengList;

        private IDataContainer<BInteraction> _interaction;

        private IDataContainer<bool> _shieldChat;

        private IDataContainer<StartRound> _startRound;

        private IDataContainer<TTZStartBroadcast> _ttzStart;

        private IDataContainer<HostInfoResult> _hostInfo;

        private IDataContainer<BRoundEnd> _roundEnd;

        private IDataContainer<com.morln.game.gd.command.RoundEnd> _raceRoundEnd;

        [Inject]
        public void Initialize(
            IAppContext context,
            IDataRepository dataRepository,
            IAppController appController,
            IWeChatManager wechatManager,
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            ChatBubble.Factory chatBubbleFactory,
            FlyItem.Factory flyItemFactory,
            ISoundController soundController,
            RemoteAPI remoteAPI)
        {
            _context = context;
            _app = appController;
            _wechatManager = wechatManager;
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
            _chatBubbleFactory = chatBubbleFactory;
            _flyItemFactory = flyItemFactory;
            _remoteAPI = remoteAPI;
            _soundController = soundController;

            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _currentTable = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _gameTipList = dataRepository.GetContainer<List<string>>(DataKey.GameTipListCount);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _startRound = dataRepository.GetContainer<StartRound>(DataKey.StartRound);
            _hostInfo = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);
            _ttzStart = dataRepository.GetContainer<TTZStartBroadcast>(DataKey.TTZStartBroadcast);
            _roundEnd = dataRepository.GetContainer<BRoundEnd>(DataKey.BRoundEnd);
            _raceRoundEnd = dataRepository.GetContainer<com.morln.game.gd.command.RoundEnd>(DataKey.RaceRoundEnd);

            _textMsg = dataRepository.GetContainer<BTextMsg>(DataKey.BTextMsg);
            _jianMeng = dataRepository.GetContainer<BJianMeng>(DataKey.BJianMeng);
            _jianMengList = dataRepository.GetContainer<List<JianMengItem>>(DataKey.JianMengItemList);
            _interaction = dataRepository.GetContainer<BInteraction>(DataKey.BInteraction);
            _shieldChat = dataRepository.GetContainer<bool>(DataKey.ShieldChat);
            _bFanBei = dataRepository.GetContainer<BFanbei>(DataKey.BFanbei);
            _inGameConfig = dataRepository.GetContainer<InGameConfig>(DataKey.InGameConfig);

            // 构建游戏界面的状态机。
            _stateLogic = new StateLogic<GameWindow>("GameWindowStateMachine", this);
            _stateLogic.AddState(new MatchingState(context));
            _stateLogic.AddState(new StartRoundState(context));
            _stateLogic.AddState(new JinGongState(context));
            _stateLogic.AddState(new HuanGongState(context));
            _stateLogic.AddState(new ChuPaiState(context));
            _stateLogic.AddState(new RoundEndState(context));
            _stateLogic.AddState(new BetweenRoundState(context));
        }

        public class Factory : PrefabFactory<GameWindow>
        {
        }

        #endregion

        #region Unity方法

        private void OnEnable()
        {
            InitEffects();
            InitComponents();
            InitBgmAndBgPic();
        }

        private void OnDisable()
        {
            DisposeEffects();
            DisposeInteractions();
            DisableBgm();
        }

        private void Start()
        {
            _stateLogic.Start();
        }

        public void Update()
        {
            RefreshTotal();
            //在更新完最新状态之后再 更新状态机。更新状态机中的状态之后再刷新所有组件
            _stateLogic.Process(Time.time);

            RefreshSeat();
            RefreshTextMsg();
            RefreshJianMeng();

            RefreshTempLeave();
            RefreshRoundEndPanel();
            RefreshRaceRoundEndPanel();
            RefreshFanbei();

            RefreshInteraction();
            RefreshInteractionItems();

            RefreshGameTip();
            CheckSingleModeBackKey();

            RefreshDaDaoJi();
        }

        private float _totalRefreshTime;

        private void RefreshTotal()
        {
            if (_totalRefreshTime >= _playingData.Timestamp)
            {
                return;
            }

            _totalRefreshTime = _playingData.Timestamp;

            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return;
            }

            var state = playingData.period;

            _stateLogic.SwitchTo(state);
        }

        #endregion

        #region 背景和背景音乐

        public AsyncImage BgPic;

        private void InitBgmAndBgPic()
        {
            SetBackground();
            _soundController.SetBgmMute(true);
        }

        private void DisableBgm()
        {
            _soundController.SetBgmMute(false);
        }

        public void SetBackground()
        {
            var enableBg = PrefsUtil.GetBool(BgConstant.EnableBgKey, true);

            if (enableBg)
            {
                var bgName = PrefsUtil.GetString(BgConstant.GameBgKey, null);
                if (string.IsNullOrEmpty(bgName))
                {
                    BgPic.gameObject.SetActive(false);
                    return;
                }

                var bgUrl = PrefsUtil.GetString(BgConstant.GameBgUrlKey, null);
                BgPic.SetTargetPic(bgName, ResourcePath.Bg, bgUrl);
                BgPic.gameObject.SetActive(true);
            }
            else
            {
                BgPic.gameObject.SetActive(false);
            }
        }

        #endregion

        /// <summary>
        /// 初始化所有的组件。
        /// </summary>
        public void InitComponents()
        {
            // 牌池应该开启。
            if (!CardLayout.gameObject.activeSelf)
            {
                CardLayout.gameObject.SetActive(true);
            }

            // 倒计时时钟应该开启。
            if (!TableClock.gameObject.activeSelf)
            {
                TableClock.gameObject.SetActive(true);
            }

            // 上一次出牌应该关闭。
            if (LastChuPaiGroup.gameObject.activeSelf)
            {
                LastChuPaiGroup.gameObject.SetActive(false);
            }

            // 出牌按钮应该关闭。
            if (ChuPaiButtonGroup.gameObject.activeSelf)
            {
                ChuPaiButtonGroup.gameObject.SetActive(false);
            }

            // 进贡按钮应该关闭。
            if (JinGongButtonGroup.gameObject.activeSelf)
            {
                JinGongButtonGroup.gameObject.SetActive(false);
            }

            // 进贡信息面板应该关闭。
            if (JinGongInfoPanel.gameObject.activeSelf)
            {
                JinGongInfoPanel.gameObject.SetActive(false);
            }

            // 获取选单张模式数据。
            // XuanDanZhang.isOn = PrefsUtil.GetBool(XuanDanZhangKey, false);

            // 结算面板应该是关闭的。
            if (RoundEndPanel.gameObject.activeSelf)
            {
                RoundEndPanel.gameObject.SetActive(false);
            }

            // 翻倍效果。
            if (_fanbeiTweener != null)
            {
                _fanbeiTweener.Kill();
                _fanbeiTweener = null;
            }

            if (FanbeiGroup.gameObject.activeSelf)
            {
                FanbeiGroup.gameObject.SetActive(false);
            }

            // 设置单机模式的状态。
            SetSingleMode(_app.IsSingleGameMode());
            // 弹出选牌引导
            CheckShowSelectPokerGuideDialog();

            // 获取游戏提示。
            GameTip.text = "";
            _tips.Clear();
            var list = _gameTipList.Read();
            if (list != null && list.Count > 0)
            {
                _tips.AddRange(list);
            }

            // 设置打到几提示的状态。
            if (DaJiTransform.gameObject.activeSelf)
            {
                DaJiTransform.gameObject.SetActive(false);
            }

            var table = _currentTable.Read();
            var gameType = table != null ? table.game_type : TableGameType.NORMAL;
            var enableChat = table != null && table.enable_chat;

            //设置可否聊天
            EnableChatBtn(enableChat);

            LeftPokerCountTop.text = "";
            LeftPokerCountLeft.text = "";
            LeftPokerCountRight.text = "";

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

        #region CardLayout

        public CardLayout CardLayout;

        public void ShowCardLayout(bool show)
        {
            if (CardLayout.gameObject.activeSelf != show)
            {
                CardLayout.gameObject.SetActive(show);
            }
        }

        #endregion

        #region LastChuPaiGroup

        public LastChuPaiGroup LastChuPaiGroup;

        public void ShowLastChuPaiGroup(bool show)
        {
            if (LastChuPaiGroup.gameObject.activeSelf != show)
            {
                LastChuPaiGroup.gameObject.SetActive(show);
            }
        }

        public void ClearLastChuPaiGroup()
        {
            LastChuPaiGroup.ClearAllSeat();
        }

        public void HideTouYouImage()
        {
            LastChuPaiGroup.HideAllTouYouImage();
        }

        #endregion

        #region TableClock

        public TableClock TableClock;

        public void ShowTableClock(bool show)
        {
            if (TableClock.gameObject.activeSelf != show)
            {
                TableClock.gameObject.SetActive(show);
            }
        }

        #endregion

        #region ChuPaiButtonGroup

        public ChuPaiButtonGroup ChuPaiButtonGroup;

        public void ShowChuPaiButtonGroup(bool show)
        {
            if (ChuPaiButtonGroup.gameObject.activeSelf != show)
            {
                ChuPaiButtonGroup.gameObject.SetActive(show);
            }
        }

        #endregion

        #region JinGongButtonGroup

        public JinGongButtonGroup JinGongButtonGroup;

        public void ShowJinGongButtonGroup(bool show)
        {
            if (JinGongButtonGroup.gameObject.activeSelf != show)
            {
                JinGongButtonGroup.gameObject.SetActive(show);
            }
        }

        #endregion

        #region JinGongInfoPanel

        public JinGongInfoPanel JinGongInfoPanel;

        public float CloseJinGongInfoDelay = 3;

        public void ShowJinGongInfoPanel(bool show)
        {
            if (show)
            {
                if (!JinGongInfoPanel.gameObject.activeSelf)
                {
                    JinGongInfoPanel.gameObject.SetActive(true);
                }
            }
            else
            {
                if (JinGongInfoPanel.gameObject.activeSelf)
                {
                    StartCoroutine(CloseJinGongInfoPanelCoroutine());
                }
            }
        }

        private IEnumerator CloseJinGongInfoPanelCoroutine()
        {
            yield return new WaitForSeconds(CloseJinGongInfoDelay);

            if (JinGongInfoPanel.gameObject.activeSelf)
            {
                JinGongInfoPanel.gameObject.SetActive(false);
            }
        }

        #endregion

        #region ReadyGroup

        public ReadyGroup ReadyGroup;

        public void ShowReadyGroup(bool show)
        {
            if (ReadyGroup.gameObject.activeSelf != show)
            {
                ReadyGroup.gameObject.SetActive(show);
            }
        }

        #endregion

        #region PokerPeeperGroup

        public PokerPeeperGroup PokerPeeperGroup;

        public void ShowPokerPeeperGroup(bool show)
        {
            if (PokerPeeperGroup.gameObject.activeSelf != show)
            {
                PokerPeeperGroup.gameObject.SetActive(show);
            }
        }

        #endregion

        #region 记牌器

        public CardRecorder CardRecorder;

        public Vector3 CardRecorderBornPos = new Vector3(0, 295);

        public bool IsCardRecorderOpen()
        {
            return CardRecorder.gameObject.activeSelf;
        }

        public void HideCardRecorder()
        {
            if (IsCardRecorderOpen())
            {
                CardRecorder.gameObject.SetActive(false);
            }
        }

        public void AutoShowCardRecorderInChuPaiState()
        {
            // 如果玩家拥有记牌器，则应该自动开启记牌器。
            var myUser = _myUser.Read();
            if (myUser == null)
            {
                return;
            }

            if (_app.IsSingleGameMode() || myUser.HasCardRecorder())
            {
                if (!CardRecorder.gameObject.activeSelf)
                {
                    CardRecorder.gameObject.SetActive(true);
                    CardRecorder.transform.localPosition = CardRecorderBornPos;
                }
            }
            else
            {
                if (CardRecorder.gameObject.activeSelf)
                {
                    CardRecorder.gameObject.SetActive(false);
                }
            }
        }

        public void ShowCardRecorder()
        {
            if (CardRecorder.gameObject.activeSelf)
            {
                CardRecorder.gameObject.SetActive(false);
            }
            else
            {
                var myUser = _myUser.Read();
                if (_app.IsSingleGameMode() || myUser.HasCardRecorder())
                {
                    CardRecorder.gameObject.SetActive(true);
                    CardRecorder.transform.localPosition = CardRecorderBornPos;
                    _analyticManager.Event("game_card_recorder_show");
                }
                else
                {
                    _dialogManager.ShowConfirmBox(
                        "您还没有记牌器，可以在兑换屋中兑换哦",
                        true, "前去兑换",
                        () =>
                        {
                            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                                (shop) => { shop.Show(ShopPanel.ShopType.Exchange, ShopPanel.ShopType.Commodity); });
                        },
                        false, null, null, false, false, true);

                    _analyticManager.Event("game_card_recorder_vip_tip");
                }
            }
        }

        #endregion

        #region 玩家对象

        public PlayerFigure PlayerTop;

        public PlayerFigure PlayerLeft;

        public PlayerFigure PlayerRight;

        public PlayerFigure PlayerBottom;

        public NicknameGroup NicknameTop;

        public NicknameGroup NicknameLeft;

        public NicknameGroup NicknameRight;

        public NicknameGroup NicknameBottom;

        public Text LeftPokerCountTop;

        public Text LeftPokerCountLeft;

        public Text LeftPokerCountRight;

        public CurrencyValue MyMoney;

        public float SeatRefreshTime { get; private set; }

        public void RefreshSeat()
        {
            if (SeatRefreshTime >= _tableUser.Timestamp)
            {
                return;
            }

            SeatRefreshTime = _tableUser.Timestamp;

            var tableUser = _tableUser.Read();

            var userTop = tableUser.UserTop();
            var userBottom = tableUser.UserBottom();
            var userLeft = tableUser.UserLeft();
            var userRight = tableUser.UserRight();

            PlayerBottom.ApplyData(userBottom);
            PlayerLeft.ApplyData(userLeft);
            PlayerTop.ApplyData(userTop);
            PlayerRight.ApplyData(userRight);

            var room = _currentRoom.Read();
            var currencyType = room != null ? room.currency_type : CurrencyType.GOLDEN_EGG;
            MyMoney.SetCurrency(DataUtil.GetCurrency(userBottom, currencyType), currencyType);

            SetNickname(NicknameTop, userTop);
            SetNickname(NicknameLeft, userLeft);
            SetNickname(NicknameRight, userRight);
            SetNickname(NicknameBottom, userBottom);
        }

        private void SetNickname(NicknameGroup nickname, User user)
        {
            if (user != null)
            {
                if (!nickname.gameObject.activeSelf)
                {
                    nickname.gameObject.SetActive(true);
                }

                nickname.SetData(user);
            }
            else
            {
                nickname.Clear();
            }
        }

        public float LeftPokerCountRefreshTime { get; private set; }

        public void RefreshLeftPokerCount()
        {
            if (LeftPokerCountRefreshTime >= _playingData.Timestamp)
            {
                return;
            }

            LeftPokerCountRefreshTime = _playingData.Timestamp;

            var tableUser = _tableUser.Read();
            var playingData = _playingData.Read();

            var seatLeft = tableUser.SeatOfPosition(SeatPosition.Left);
            var seatTop = tableUser.SeatOfPosition(SeatPosition.Top);
            var seatRight = tableUser.SeatOfPosition(SeatPosition.Right);

            SetLeftPokerCount(LeftPokerCountLeft, playingData.LeftPokerCountOfSeat(seatLeft));
            SetLeftPokerCount(LeftPokerCountTop, playingData.LeftPokerCountOfSeat(seatTop));
            SetLeftPokerCount(LeftPokerCountRight, playingData.LeftPokerCountOfSeat(seatRight));
        }

        private void SetLeftPokerCount(Text text, int count)
        {
            if (count < 32)
            {
                if (!text.gameObject.activeSelf)
                {
                    text.gameObject.SetActive(true);
                }

                if (count > 0)
                {
                    text.text = "剩 " + count + " 张";
                }
                else
                {
                    text.text = "已出完";
                }
            }
            else
            {
                if (text.gameObject.activeSelf)
                {
                    text.gameObject.SetActive(false);
                }
            }
        }

        public void ShowLeftPokerCount(bool isShow)
        {
            if (LeftPokerCountLeft.gameObject.activeSelf != isShow)
            {
                LeftPokerCountLeft.gameObject.SetActive(isShow);
            }

            if (LeftPokerCountTop.gameObject.activeSelf != isShow)
            {
                LeftPokerCountTop.gameObject.SetActive(isShow);
            }

            if (LeftPokerCountRight.gameObject.activeSelf != isShow)
            {
                LeftPokerCountRight.gameObject.SetActive(isShow);
            }
        }

        public void ShowTopInfo()
        {
            if (!_app.IsSingleGameMode())
            {
                ShowOtherInfo(SeatPosition.Top);
            }
        }

        public void ShowLeftInfo()
        {
            if (!_app.IsSingleGameMode())
            {
                ShowOtherInfo(SeatPosition.Left);
            }
        }

        public void ShowRightInfo()
        {
            if (!_app.IsSingleGameMode())
            {
                ShowOtherInfo(SeatPosition.Right);
            }
        }

        private void ShowOtherInfo(SeatPosition pos)
        {
            User user = null;
            var tableUser = _tableUser.Read();

            switch (pos)
            {
                case SeatPosition.Left:
                    user = tableUser.GetUserAtPos(SeatPosition.Left);
                    break;

                case SeatPosition.Top:
                    user = tableUser.GetUserAtPos(SeatPosition.Top);
                    break;

                case SeatPosition.Right:
                    user = tableUser.GetUserAtPos(SeatPosition.Right);
                    break;
            }

            if (user != null)
            {
                _dialogManager.ShowDialog<OtherInfoPanel>(DialogName.OtherInfoPanel, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(user, true, true);
                        dialog.Show();
                    });
            }
        }

        #endregion

        #region TopFuncBar

        public GameObject TopFuncBar;

        public void ShowTopFuncBar(bool isShow)
        {
            if (TopFuncBar.activeSelf != isShow)
            {
                TopFuncBar.SetActive(isShow);
            }
        }

        #endregion

        #region 结算

        public RoundEndPanel RoundEndPanel;

        private float RoundEndRefreshTime { get; set; }

        private void RefreshRoundEndPanel()
        {
            if (RoundEndRefreshTime >= _roundEnd.Timestamp)
            {
                return;
            }

            RoundEndRefreshTime = _roundEnd.Timestamp;

            if (!RoundEndPanel.gameObject.activeSelf)
            {
                RoundEndPanel.gameObject.SetActive(true);
            }

            var data = _roundEnd.Read();
            if (data != null)
            {
                RoundEndPanel.ShowTotalRoundEnd(_roundEnd.Read());
            }
            else
            {
                RoundEndPanel.Hide();
            }
        }

        /// <summary>
        /// 比赛房结算刷新
        /// </summary>
        private float RaceRoundEndRefreshTime { get; set; }

        /// <summary>
        /// 比赛房结算
        /// </summary>
        private void RefreshRaceRoundEndPanel()
        {
            if (RaceRoundEndRefreshTime >= _raceRoundEnd.Timestamp)
            {
                return;
            }

            RaceRoundEndRefreshTime = _raceRoundEnd.Timestamp;

            if (!RoundEndPanel.gameObject.activeSelf)
            {
                RoundEndPanel.gameObject.SetActive(true);
            }

            var data = _raceRoundEnd.Read();
            if (data != null)
            {
                RoundEndPanel.ShowTotalRoundEnd(_raceRoundEnd.Read());
            }
            else
            {
                RoundEndPanel.Hide();
            }
        }

        #endregion

        #region 托管

        public Button TempLeaveCover;

        public float TempLeaveRefreshTime { get; private set; }

        private void RefreshTempLeave()
        {
            var refreshTime = _myUser.Timestamp;
            if (TempLeaveRefreshTime >= refreshTime)
            {
                return;
            }

            TempLeaveRefreshTime = refreshTime;

            var user = _myUser.Read();
            if (user == null)
            {
                ShowTempLeave(false);
                return;
            }

            ShowTempLeave(user.IsTempLeave());
        }

        public void ShowTempLeave(bool show)
        {
            if (TempLeaveCover.gameObject.activeSelf != show)
            {
                TempLeaveCover.gameObject.SetActive(show);
            }
        }

        public void SwitchTempLeave()
        {
            if (_app.IsSingleGameMode())
            {
                // 单机模式下不能托管。
                _dialogManager.ShowToast("单机模式下不能托管哦^_^", 2);
            }
            else
            {
                var user = _myUser.Read();
                if (user != null)
                {
                    _remoteAPI.TempLeave(!user.IsTempLeave());
                    _analyticManager.Event("game_switch_templeave");
                }
            }
        }

        #endregion

        #region 设置

        public void OnSettingButtonClicked()
        {
            _dialogManager.ShowDialog<UIWindow>(DialogName.SettingDialog, true, true);
        }

        #endregion

        #region 聊天

        public RectTransform TopChatContainer;

        public RectTransform BottomChatContainer;

        public RectTransform LeftChatContainer;

        public RectTransform RightChatContainer;

        #region TextMsg

        public float TextMsgRefreshTime { get; private set; }

        private void RefreshTextMsg()
        {
            if (TextMsgRefreshTime >= _textMsg.Timestamp)
            {
                return;
            }

            TextMsgRefreshTime = _textMsg.Timestamp;

            var msg = _textMsg.Read();
            if (msg == null)
            {
                return;
            }

            var myUser = _myUser.Read();
            var shieldChat = _shieldChat.Read();
            if (!shieldChat || (myUser != null && myUser.username == msg.from_username))
            {
                DoTextMsg(msg);
            }

            // 清空当前数据。
            _textMsg.ClearNotInvalidate();
        }

        private void DoTextMsg(BTextMsg msg)
        {
            var tableUser = _tableUser.Read();
            var seat = tableUser.GetSeatOfUser(msg.from_username);
            if (seat == -1)
            {
                // 不是当前桌子上的人发的消息，就直接忽略吧。
                return;
            }

            var pos = tableUser.PositionOfSeat(seat);
            if (pos == SeatPosition.Null)
            {
                return;
            }

            if (string.IsNullOrEmpty(msg.content))
            {
                // 聊天内容为空的时候，也直接忽略。
                return;
            }

            RectTransform container = null;
            switch (pos)
            {
                case SeatPosition.Top:
                    container = TopChatContainer;
                    break;

                case SeatPosition.Left:
                    container = LeftChatContainer;
                    break;

                case SeatPosition.Right:
                    container = RightChatContainer;
                    break;

                case SeatPosition.Bottom:
                    container = BottomChatContainer;
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

            if (Emoji.IsEmoji(msg.content))
            {
                bubble.SetEmoji(msg.content);
            }
            else
            {
                bubble.SetText(msg.content);
            }

            bubble.Show();
        }

        #endregion

        #region JianMeng

        public float JianMengRefreshTime { get; private set; }

        private void RefreshJianMeng()
        {
            if (JianMengRefreshTime >= _jianMeng.Timestamp)
            {
                return;
            }

            JianMengRefreshTime = _jianMeng.Timestamp;

            var msg = _jianMeng.Read();
            if (msg == null)
            {
                return;
            }

            var list = _jianMengList.Read();
            var data = FindJianMengItem(list, msg.cmd);
            if (data == null)
            {
                return;
            }

            var shieldChat = _shieldChat.Read();
            var myUser = _myUser.Read();
            if (!shieldChat || (myUser != null && myUser.username == msg.from_username))
            {
                DoJianMeng(msg, data);
            }

            // 清空当前数据。
            _jianMeng.ClearNotInvalidate();
        }

        private void DoJianMeng(BJianMeng msg, JianMengItem data)
        {
            var tableUser = _tableUser.Read();
            var seat = tableUser.GetSeatOfUser(msg.from_username);
            if (seat == -1)
            {
                // 不是当前桌子上的人发的消息，就直接忽略吧。
                return;
            }

            var pos = tableUser.PositionOfSeat(seat);
            if (pos == SeatPosition.Null)
            {
                return;
            }

            RectTransform container = null;
            switch (pos)
            {
                case SeatPosition.Top:
                    container = TopChatContainer;
                    break;

                case SeatPosition.Left:
                    container = LeftChatContainer;
                    break;

                case SeatPosition.Right:
                    container = RightChatContainer;
                    break;

                case SeatPosition.Bottom:
                    container = BottomChatContainer;
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

        #endregion

        public void ShowChatPanel()
        {
            // 进还贡的时候不能打开聊天面板。
            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return;
            }

            var tablePeriod = playingData.period;
            if (tablePeriod == TablePeriod.JinGong ||
                tablePeriod == TablePeriod.HuanGong)
            {
                _dialogManager.ShowToast("进还贡的时候不能聊天哦", 2);
                return;
            }

            _dialogManager.ShowDialog<ChatPanel>(DialogName.ChatPanel, true, true);
        }

        #endregion

        #region 顶部功能按钮

        #endregion

        #region 打牌效果

        #region 游戏震屏

        public RectTransform Content;

        /// <summary>
        /// 游戏窗口震屏。
        /// </summary>
        public void Shake()
        {
            Content.localPosition = new Vector3(0, 0, 0);
            Content.DOPunchPosition(new Vector3(0, 5, 0), 0.5f);
        }

        #endregion

        #region 玩家交互效果

        public RectTransform TableGroup;

        public RectTransform TopInteractionPos;

        public RectTransform BottomInteractionPos;

        public RectTransform LeftInteractionPos;

        public RectTransform RightInteractionPos;

        public float OtherInteractionScale = 0.8f;

        public float MyInteractionScale = 1f;

        /// <summary>
        /// 执行玩家之间的互动。
        /// 玩家之间的互动有可能是同时发生的，因此需要生成FlyItem，并且执行。
        /// 也就是需要一个缓存了。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromUsername"></param>
        /// <param name="toUsername"></param>
        public void DoInteraction(int code, string fromUsername, string toUsername)
        {
            var tableUser = _tableUser.Read();
            var fromSeat = tableUser.GetSeatOfUser(fromUsername);
            var toSeat = tableUser.GetSeatOfUser(toUsername);

            var fromSeatPos = tableUser.PositionOfSeat(fromSeat);
            var toSeatPos = tableUser.PositionOfSeat(toSeat);

            var fromScale = fromSeatPos == SeatPosition.Bottom ? MyInteractionScale : OtherInteractionScale;
            var toScale = toSeatPos == SeatPosition.Bottom ? MyInteractionScale : OtherInteractionScale;

            var fromPos = GetInteractionPos(fromSeatPos);
            var toPos = GetInteractionPos(toSeatPos);

            switch (code)
            {
                case InteractionCode.Egg:
                    FlyEgg(fromPos, toPos, fromScale, toScale);
                    break;

                case InteractionCode.Flower:
                    FlyFlower(fromPos, toPos, fromScale, toScale);
                    break;
            }
        }

        /// <summary>
        /// 刷新当前鸡蛋和鲜花的状态。
        /// </summary>
        private void RefreshInteractionItems()
        {
            RefreshEggState();
            RefreshFlowerState();
        }

        private Vector2 GetInteractionPos(SeatPosition seat)
        {
            var res = Vector3.one;

            switch (seat)
            {
                case SeatPosition.Top:
                    res = TopInteractionPos.position;
                    break;

                case SeatPosition.Left:
                    res = LeftInteractionPos.position;
                    break;

                case SeatPosition.Right:
                    res = RightInteractionPos.position;
                    break;

                case SeatPosition.Bottom:
                    res = BottomInteractionPos.position;
                    break;
            }

            return res;
        }

        private float _interactionRefreshTime;

        private void RefreshInteraction()
        {
            if (_interactionRefreshTime >= _interaction.Timestamp)
            {
                return;
            }

            _interactionRefreshTime = _interaction.Timestamp;
            var shieldChat = _shieldChat.Read();
            var myUser = _myUser.Read();
            var data = _interaction.Read();
            if (data == null)
            {
                return;
            }

            if (!shieldChat || (myUser != null && data.from_username == myUser.username))
            {
                DoInteraction(data.code, data.from_username, data.to_username);
            }

            _interaction.ClearNotInvalidate();
        }

        private void DisposeInteractions()
        {
            DisposeEggs();
            DisposeFlowers();
        }

        #region 鸡蛋

        public FlyItem EggPrefab;

        private readonly List<FlyItem> _curEgg = new List<FlyItem>();

        private readonly Queue<FlyItem> _eggCache = new Queue<FlyItem>();

        private void FlyEgg(Vector2 fromPos, Vector2 toPos, float fromScale, float toScale)
        {
            FlyItem egg;
            if (_eggCache.Count > 0)
            {
                egg = _eggCache.Dequeue();
            }
            else
            {
                egg = _flyItemFactory.Create(EggPrefab);
            }

            if (!egg.gameObject.activeSelf)
            {
                egg.gameObject.SetActive(true);
            }

            egg.transform.SetParent(transform, false);

            egg.StartFly(fromPos, toPos, fromScale, toScale);
            _curEgg.Add(egg);
        }

        private void RefreshEggState()
        {
            for (int i = 0; i < _curEgg.Count; i++)
            {
                var egg = _curEgg[i];
                if (!egg.IsRunning)
                {
                    if (egg.gameObject.activeSelf)
                    {
                        egg.gameObject.SetActive(false);
                    }

                    _eggCache.Enqueue(egg);
                    _curEgg.RemoveAt(i);
                    i--;
                }
            }
        }

        private void DisposeEggs()
        {
            foreach (var egg in _curEgg)
            {
                Destroy(egg.gameObject);
            }

            _curEgg.Clear();

            foreach (var egg in _eggCache)
            {
                Destroy(egg.gameObject);
            }

            _eggCache.Clear();
        }

        #endregion

        #region 鲜花

        public FlyItem FlowerPrefab;

        private readonly List<FlyItem> _curFlower = new List<FlyItem>();

        private readonly Queue<FlyItem> _flowerCache = new Queue<FlyItem>();

        private void FlyFlower(Vector2 fromPos, Vector2 toPos, float fromScale, float toScale)
        {
            FlyItem flower;

            if (_flowerCache.Count > 0)
            {
                flower = _flowerCache.Dequeue();
            }
            else
            {
                flower = _flyItemFactory.Create(FlowerPrefab);
            }

            if (!flower.gameObject.activeSelf)
            {
                flower.gameObject.SetActive(true);
            }

            flower.transform.SetParent(transform, false);

            flower.StartFly(fromPos, toPos, fromScale, toScale);
            _curFlower.Add(flower);
        }

        private void RefreshFlowerState()
        {
            for (int i = 0; i < _curFlower.Count; i++)
            {
                var flower = _curFlower[i];
                if (!flower.IsRunning)
                {
                    if (flower.gameObject.activeSelf)
                    {
                        flower.gameObject.SetActive(false);
                    }

                    _flowerCache.Enqueue(flower);
                    _curFlower.RemoveAt(i);
                    i--;
                }
            }
        }

        private void DisposeFlowers()
        {
            foreach (var flower in _curFlower)
            {
                Destroy(flower.gameObject);
            }

            _curFlower.Clear();

            foreach (var flower in _flowerCache)
            {
                Destroy(flower.gameObject);
            }

            _flowerCache.Clear();
        }

        #endregion

        #endregion

        #region 初始化和销毁效果

        public void InitEffects()
        {
            _daZhaDan = Instantiate(DaZhaDanPrefab, Vector3.zero, Quaternion.identity);
            _xiaoZhaDan = Instantiate(XiaoZhaDanPrefab, Vector3.zero, Quaternion.identity);
            _tongHuaShun = Instantiate(TongHuaShunPrefab, Vector3.zero, Quaternion.identity);
            _huoJian = Instantiate(HuoJianPrefab, Vector3.zero, Quaternion.identity);
            _feiJi = Instantiate(FeiJiPrefab, Vector3.zero, Quaternion.identity);

            if (_daZhaDan != null)
            {
                _daZhaDan.transform.SetParent(transform, false);
                _daZhaDan.gameObject.SetActive(false);
            }

            if (_xiaoZhaDan != null)
            {
                _xiaoZhaDan.transform.SetParent(transform, false);
                _xiaoZhaDan.gameObject.SetActive(false);
            }

            if (_tongHuaShun != null)
            {
                _tongHuaShun.transform.SetParent(transform, false);
                _tongHuaShun.gameObject.SetActive(false);
            }

            if (_huoJian != null)
            {
                _huoJian.transform.SetParent(transform, false);
                _huoJian.gameObject.SetActive(false);
            }

            if (_feiJi != null)
            {
                _feiJi.transform.SetParent(transform, false);
                _feiJi.gameObject.SetActive(false);
            }
        }

        public void DisposeEffects()
        {
            if (_daZhaDan)
            {
                Destroy(_daZhaDan.gameObject);
                _daZhaDan = null;
            }

            if (_xiaoZhaDan)
            {
                Destroy(_xiaoZhaDan.gameObject);
                _xiaoZhaDan = null;
            }

            if (_tongHuaShun)
            {
                Destroy(_tongHuaShun.gameObject);
                _tongHuaShun = null;
            }

            if (_huoJian)
            {
                Destroy(_huoJian.gameObject);
                _huoJian = null;
            }

            if (_feiJi)
            {
                Destroy(_feiJi.gameObject);
                _feiJi = null;
            }
        }

        #endregion

        public const string DisplayKey = "Display";

        public void ShowChuPaiEffect(PokerPattern chupai)
        {
            if (chupai == null) return;

            switch (chupai.Type)
            {
                case PatternType.AAABBB:
                    ShowFeiJi();
                    break;

                case PatternType.SuperABCDE:
                    ShowTongHuaShun();
                    break;

                case PatternType.XXDD:
                    ShowHuoJian();
                    break;

                case PatternType.XXXX:
                    Shake();
                    if (PatternType.IsBigXXXX(chupai))
                        ShowDaZhaDan();
                    else
                        ShowXiaoZhaDan();
                    break;
            }
        }

        private IEnumerator PlayEffect(Animator effect)
        {
            if (!effect)
            {
                yield break;
            }

            if (!effect.gameObject.activeSelf)
                effect.gameObject.SetActive(true);

            yield return null;

            effect.SetTrigger(DisplayKey);
        }

        #region 大炸弹

        /// <summary>
        /// 大炸弹效果的Prefab。
        /// </summary>
        public Animator DaZhaDanPrefab;

        private Animator _daZhaDan;

        public void ShowDaZhaDan()
        {
            StartCoroutine(PlayEffect(_daZhaDan));
        }

        #endregion

        #region 小炸弹

        /// <summary>
        /// 小炸弹效果的Prefab。
        /// </summary>
        public Animator XiaoZhaDanPrefab;

        private Animator _xiaoZhaDan;

        public void ShowXiaoZhaDan()
        {
            StartCoroutine(PlayEffect(_xiaoZhaDan));
        }

        #endregion

        #region 同花顺

        /// <summary>
        /// 同花顺效果的Prefab。
        /// </summary>
        public Animator TongHuaShunPrefab;

        private Animator _tongHuaShun;

        public void ShowTongHuaShun()
        {
            StartCoroutine(PlayEffect(_tongHuaShun));
        }

        #endregion

        #region 火箭

        /// <summary>
        /// 火箭效果的Prefab。
        /// </summary>
        public Animator HuoJianPrefab;

        private Animator _huoJian;

        public void ShowHuoJian()
        {
            StartCoroutine(PlayEffect(_huoJian));
        }

        #endregion

        #region 飞机

        /// <summary>
        /// 飞机效果的Prefab。
        /// </summary>
        public Animator FeiJiPrefab;

        private Animator _feiJi;

        public void ShowFeiJi()
        {
            StartCoroutine(PlayEffect(_feiJi));
        }

        #endregion

        #endregion

        #region 单机模式

        private void SetSingleMode(bool singleMode)
        {
            if (BackToOnlineBtn.gameObject.activeSelf != singleMode)
            {
                BackToOnlineBtn.gameObject.SetActive(singleMode);
            }

            if (ChatBtn.gameObject.activeSelf == singleMode)
            {
                ChatBtn.gameObject.SetActive(!singleMode);
            }
        }

        public Button BackToOnlineBtn;

        public Button ChatBtn;

        public void ExitSingleMode()
        {
            _quitDialog = _dialogManager.ShowConfirmBox("是否退出单机模式？",
                true, "退出", () => { _app.ExitSingleGame(); },
                true, "继续", () => { }, true, true, true);
        }

        private AlertBox _quitDialog;

        private void CheckSingleModeBackKey()
        {
            if (!_app.IsSingleGameMode())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_quitDialog != null)
                {
                    _quitDialog.Cancel();
                    return;
                }

                ExitSingleMode();
            }
        }

        #endregion

        #region 按钮功能

        public void ShowShopPanel()
        {
            if (_app.IsSingleGameMode())
            {
                _dialogManager.ShowToast("单机模式下，不能开启商店哦", 2);
                return;
            }

            _analyticManager.Event("game_shop_show");
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) =>
                {
                    var table = _currentTable.Read();
                    var gameType = table != null ? table.game_type : TableGameType.NORMAL;
                    if (gameType == TableGameType.RACE_TTZ)
                    {
                        shop.Show(ShopPanel.ShopType.Charge);
                    }
                    else
                    {
                        shop.Show(ShopPanel.ShopType.Charge, ShopPanel.ShopType.Commodity);
                    }
                }
            );
        }

        public Button WeChatShareBtn;

        public int Offset = 256;

        public void WxShare()
        {
            if (_app.IsSingleGameMode())
            {
                _dialogManager.ShowToast("单机模式无法截图分享", 2);
                return;
            }

            WeChatShareBtn.interactable = false;
            StartCoroutine(ShareCoroutine());
        }

        private IEnumerator ShareCoroutine()
        {
            _wechatManager.WechatShareScreenShot(Offset);
            yield return new WaitForSeconds(0.5f);
            WeChatShareBtn.interactable = true;
        }

        #endregion

        #region 翻倍

        public RectTransform FanbeiGroup;

        public CanvasGroup FanbeiCanvasGroup;

        public RectTransform FanbeiContent;

        public Image TensDigit;

        public Image UnitsDigit;

        public Sprite[] FanbeiNumGroup;

        public float FanbeiShowTime = 0.2f;

        public float FanbeiBigTime = 0.3f;

        public float FanbeiStayTime = 0.5f;

        public float FanbeiHideTime = 0.2f;

        private float _fanbeiRefreshTime;

        private Sequence _fanbeiTweener;

        private void RefreshFanbei()
        {
            if (_fanbeiRefreshTime >= _bFanBei.Timestamp)
            {
                return;
            }

            _fanbeiRefreshTime = _bFanBei.Timestamp;

            if (_fanbeiTweener != null)
            {
                _fanbeiTweener.Kill();
                _fanbeiTweener = null;
            }

            //一次性数据读取后清空
            var bFanbei = _bFanBei.Read(true);
            if (bFanbei == null)
            {
                FanbeiGroup.gameObject.SetActive(false);
                return;
            }

            var fanbeiNum = bFanbei.new_multiple;
            if (fanbeiNum <= 1 || fanbeiNum > 99)
            {
                if (FanbeiGroup.gameObject.activeSelf)
                {
                    FanbeiGroup.gameObject.SetActive(false);
                }

                return;
            }

            int tensDigit = fanbeiNum / 10;
            int unitsDigit = fanbeiNum % 10;

            if (tensDigit < 0 || tensDigit >= FanbeiNumGroup.Length
                || unitsDigit < 0 || unitsDigit >= FanbeiNumGroup.Length)
            {
                if (FanbeiGroup.gameObject.activeSelf)
                {
                    FanbeiGroup.gameObject.SetActive(false);
                }

                return;
            }

            TensDigit.sprite = FanbeiNumGroup[tensDigit];
            UnitsDigit.sprite = FanbeiNumGroup[unitsDigit];
            TensDigit.gameObject.SetActive(tensDigit > 0);

            if (!FanbeiGroup.gameObject.activeSelf)
            {
                FanbeiGroup.gameObject.SetActive(true);
            }

            FanbeiCanvasGroup.alpha = 1;
            FanbeiGroup.localScale = new Vector3(1, 0, 1);

            _fanbeiTweener = DOTween.Sequence();
            _fanbeiTweener
                .Append(FanbeiGroup.DOScaleY(1, FanbeiShowTime))
                .Append(FanbeiContent.DOPunchScale(new Vector3(0.5f, 0.5f, 0), FanbeiBigTime, 1, 0))
                .AppendInterval(FanbeiStayTime)
                .Append(FanbeiCanvasGroup.DOFade(0, FanbeiHideTime))
                .OnComplete(() => FanbeiGroup.gameObject.SetActive(false));
        }

        #endregion

        #region 小提示

        public Text GameTip;

        public float GameTipInterval = 30f;

        private float _gameTipTime;

        private readonly List<string> _tips = new List<string>();

        private int _tipIdx;

        private void RefreshGameTip()
        {
            if (_tips.Count <= 0)
            {
                return;
            }

            if (Time.time - _gameTipTime < GameTipInterval)
            {
                return;
            }

            _gameTipTime = Time.time;

            var tip = "";
            if (_tipIdx < _tips.Count)
            {
                tip = _tips[_tipIdx];
            }

            GameTip.text = tip;

            _tipIdx++;
            if (_tipIdx >= _tips.Count)
            {
                _tipIdx = 0;
            }
        }

        #endregion

        #region 开局

        public RectTransform DaJiTransform;

        public Text DaJiText;

        public float DaJiShowTime = 1f;

        public float DaJiDisplayTime = 2f;

        private Sequence _daJiTweener;

        private float _startRoundRefreshTime;

        private void RefreshDaDaoJi()
        {
            // 收到StartRound命令的时候，显示本局打到几。
            // 如果是断线重连进游戏界面的时候，也应该显示一下当前打几。

            if (_startRoundRefreshTime >= _startRound.Timestamp)
            {
                return;
            }

            _startRoundRefreshTime = _startRound.Timestamp;

            // 显示一下当前局打几的动画。
            if (_daJiTweener != null)
            {
                _daJiTweener.Kill();
                _daJiTweener = null;
            }

            var hostInfo = _hostInfo.Read();
            DaJiText.text = "本局打 " + hostInfo.GetCurrentHostLabel();

            if (!DaJiTransform.gameObject.activeSelf)
            {
                DaJiTransform.gameObject.SetActive(true);
            }

            DaJiTransform.localScale = new Vector3(0, 0, 1);
            _daJiTweener = DOTween.Sequence();
            _daJiTweener
                .Append(
                    DaJiTransform.DOScale(
                            new Vector3(1, 1, 1),
                            DaJiShowTime)
                        .SetEase(Ease.OutBack))
                .AppendInterval(DaJiDisplayTime)
                .Append(
                    DaJiTransform.DOScale(
                        new Vector3(0, 0, 1),
                        DaJiShowTime / 2))
                .OnComplete(() =>
                {
                    if (DaJiTransform.gameObject.activeSelf)
                    {
                        DaJiTransform.gameObject.SetActive(false);
                    }
                });
        }

        #endregion

        #region HostInfo

        public GameObject NormalHostGroup;

        public GameObject TTZHostGroup;

        public GameObject RaceTTZHostGroup;

        #endregion

        #region TTZ

        private float _ttzStartRefreshTime;

        private Sequence _leftPlayerTweener;

        private Sequence _rightPlayerTweener;

        private Sequence _topPlayerTweener;

        public void RefreshTTZStart()
        {
            var ttzStartTime = _ttzStart.Timestamp;
            if (_ttzStartRefreshTime >= ttzStartTime)
            {
                return;
            }

            _ttzStartRefreshTime = ttzStartTime;
            var msg = _ttzStart.Read();
            if (msg == null)
            {
                return;
            }

            // 显示换座的提示对话框。
            var dialogManager = _context.GetDialogManager();
            dialogManager.ShowDialog<TTZStartInfoDialog>(DialogName.TTZStartInfoDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(msg);
                    dialog.ShowAndAutoHide();
                }
            );


            // 只有在换座位的时候才显示换座位的动画。
            if (msg.change_seat)
            {
                ShowChangeSeatAnimation();
            }
        }

        public float ChangeSeatHideTime = 0.1f;

        public float ChangeSeatStayTimeLeft = 0.5f;

        public float ChangeSeatStayTimeTop = 1f;

        public float ChangeSeatStayTimeRight = 1.5f;

        public float ChangeSeatShowTime = 0.2f;

        public float ChangeSeatOverShoot = 2f;

        public void ShowChangeSeatAnimation()
        {
            // 显示刷新座位的动画。
            if (_leftPlayerTweener != null)
            {
                _leftPlayerTweener.Kill();
                _leftPlayerTweener = null;
            }

            if (_rightPlayerTweener != null)
            {
                _rightPlayerTweener.Kill();
                _rightPlayerTweener = null;
            }

            if (_topPlayerTweener != null)
            {
                _topPlayerTweener.Kill();
                _topPlayerTweener = null;
            }

            var leftTargetScale = new Vector3(-1, 1, 1);
            _leftPlayerTweener = DOTween.Sequence();
            _leftPlayerTweener
                .Append(PlayerLeft.transform.DOScale(Vector3.zero, ChangeSeatHideTime))
                .AppendInterval(ChangeSeatStayTimeLeft)
                .Append(PlayerLeft.transform.DOScale(leftTargetScale, ChangeSeatShowTime)
                    .SetEase(Ease.OutElastic, ChangeSeatOverShoot))
                .Play();

            _rightPlayerTweener = DOTween.Sequence();
            _rightPlayerTweener
                .Append(PlayerRight.transform.DOScale(Vector3.zero, ChangeSeatHideTime))
                .AppendInterval(ChangeSeatStayTimeRight)
                .Append(PlayerRight.transform.DOScale(Vector3.one, ChangeSeatShowTime)
                    .SetEase(Ease.OutElastic, ChangeSeatOverShoot))
                .Play();

            _topPlayerTweener = DOTween.Sequence();
            _topPlayerTweener
                .Append(PlayerTop.transform.DOScale(Vector3.zero, ChangeSeatHideTime))
                .AppendInterval(ChangeSeatStayTimeTop)
                .Append(PlayerTop.transform.DOScale(Vector3.one, ChangeSeatShowTime)
                    .SetEase(Ease.OutElastic, ChangeSeatOverShoot))
                .Play();
        }

        #endregion

        #region 匹配中

        public GameObject MatchingImage;

        public Text MatchingSeconds;

        public GameObject CancelMatching;

        public float CancelMatchingTime = 15f;

        private Tweener _tweener;

        public float ShowAnimationTime = 0.2f;

        private bool _isShowAnimation;

        public void ShowMatchingTxt(bool isShow)
        {
            if (MatchingImage.gameObject.activeSelf != isShow)
            {
                MatchingImage.gameObject.SetActive(isShow);
            }
        }

        public void RefreshMatchingSeconds(float startTime, float currentTime)
        {
            var waitSeconds = (int) (currentTime - startTime);
            if (waitSeconds <= 0)
            {
                MatchingImage.gameObject.SetActive(false);
            }
            else
            {
                MatchingImage.gameObject.SetActive(true);
                MatchingSeconds.text = waitSeconds + "秒";
            }

            CancelMatching.SetActive(waitSeconds >= CancelMatchingTime);

            if (waitSeconds >= CancelMatchingTime && !_isShowAnimation)
            {
                if (_tweener != null)
                {
                    _tweener.Kill();
                    _tweener = null;
                }

                CancelMatching.transform.localScale = new Vector3(0, 0, 1);

                _tweener = CancelMatching.transform
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack);
                _isShowAnimation = true;
            }
        }

        public void Cancelmatching()
        {
            _remoteAPI.LeaveRoom(false);
        }

        #endregion

        #region BottomBtnGroup

        public BottomBtnGroup BottomBtnGroup;

        private void EnableChatBtn(bool isShow)
        {
            BottomBtnGroup.EnableChatBtn(isShow);
        }

        #endregion

        #region HideRoundEndPanel

        public void HideRoundEndPanel()
        {
            RoundEndPanel.Hide();
        }

        #endregion

        #region 选牌引导

        public void CheckShowSelectPokerGuideDialog()
        {
            var hasShow = PrefsUtil.GetBool(PrefsKeys.HasGuideSelectPoker, false);
            if (!hasShow)
            {
                _dialogManager.ShowDialog<UIWindow>(DialogName.SelectPokerGuideDialog, true, true);
            }
        }

        #endregion
    }
}