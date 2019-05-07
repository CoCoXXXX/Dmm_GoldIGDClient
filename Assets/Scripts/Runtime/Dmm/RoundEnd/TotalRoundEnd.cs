using System.Collections.Generic;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.App;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Sound;
using Dmm.Task;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.RoundEnd
{
    public class UserNameAndNickName
    {
        public string Username;

        public string NickName;

        public int Vip = 0;
    }

    public class TotalRoundEnd : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IAppController _appController;

        private ISoundController _soundController;

        private IDialogManager _dialogManager;

        private ITaskManager _taskManager;

        private RoundEndRankMySelf.Factory _rankMySelfFactory;

        private RoundEndRankOther.Factory _rankOtherFactory;

        private IDataContainer<User> _user;

        private IDataContainer<Room> _room;

        private IDataContainer<TableUserData> _tableUserData;

        private IDataContainer<RaceConfig> _raceConfig;

        private IDataContainer<BKickOutCounter> _bKickOutCounter;

        private IDataContainer<ReadyResult> _readyResult;

        [Inject]
        public void Initialize(
            IAppController appController,
            ISoundController soundController,
            IDialogManager dialogManager,
            ITaskManager taskManager,
            RemoteAPI remoteAPI,
            RoundEndRankMySelf.Factory rankMySelfFactory,
            IDataRepository dataRepository,
            RoundEndRankOther.Factory rankOtherFactory)
        {
            _appController = appController;
            _soundController = soundController;
            _dialogManager = dialogManager;
            _taskManager = taskManager;
            _remoteAPI = remoteAPI;
            _rankMySelfFactory = rankMySelfFactory;
            _rankOtherFactory = rankOtherFactory;
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _raceConfig = dataRepository.GetContainer<RaceConfig>(DataKey.RaceConfig);
            _bKickOutCounter = dataRepository.GetContainer<BKickOutCounter>(DataKey.BKickOutCounter);
            _readyResult = dataRepository.GetContainer<ReadyResult>(DataKey.ReadyResult);
        }

        #endregion

        #region Unity方法

        public void Update()
        {
            RefreshKickOutCounter();
            UpdateKickOutCounterTime();
        }

        #endregion

        public RoundEndPanel RoundEndPanel;

        #region 结束图片

        public Sprite WinImg;

        public Sprite LoseImg;

        public Image EndImage;

        #endregion

        #region 刷新内容

        public RectTransform RankContent;

        public RectTransform ReadyGroup;

        public Text ReadyLeftTime;

        private RoundEndRankMySelf _mySelfRank;

        private readonly List<RoundEndRankOther> _otherRanks = new List<RoundEndRankOther>();

        private readonly string[] _ranks = new string[4];

        private readonly bool[] _win = new bool[4];

        public int RankOf(User user)
        {
            if (user == null)
            {
                return -1;
            }

            for (int i = 0; i < _ranks.Length; i++)
            {
                if (StringUtil.AreEqual(user.username, _ranks[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool IsWin(User user)
        {
            if (user == null)
            {
                return true;
            }

            var rank = RankOf(user);
            if (rank != -1)
            {
                return _win[rank];
            }

            return false;
        }

        /// <summary>
        /// 我自己是否是赢家。
        /// </summary>
        public bool SelfWin
        {
            get { return IsWin(_user.Read()); }
        }

        public void RefreshContent(BRoundEnd msg)
        {
            if (msg == null)
            {
                return;
            }

            if (!RankContent)
            {
                return;
            }

            var lost = msg.lost;
            var tableUser = _tableUserData.Read();
            // 只有在已经开局的情况下收到结算命令才有用。
            _ranks[0] = msg.username1;
            _ranks[1] = msg.username2;
            _ranks[2] = msg.username3;
            _ranks[3] = msg.username4;

            _win[0] = true;
            _win[1] = tableUser.IsSameTeam(msg.username2, msg.username1);
            _win[2] = tableUser.IsSameTeam(msg.username3, msg.username1);
            _win[3] = tableUser.IsSameTeam(msg.username4, msg.username1);

            if (lost || SelfWin)
            {
                // 掉线或者赢了，都播放赢的声音。
                _soundController.PlayEndRoundWinSound();
            }
            else
            {
                _soundController.PlayEndRoundLoseSound();
            }

            var user1 = tableUser.GetUserFormUserName(msg.username1);
            var user2 = tableUser.GetUserFormUserName(msg.username2);
            var user3 = tableUser.GetUserFormUserName(msg.username3);
            var user4 = tableUser.GetUserFormUserName(msg.username4);

            var users = new[] {user1, user2, user3, user4};
            int currentOtherRank = 0;

            for (int i = 0; i < users.Length; i++)
            {
                var u = users[i];
                if (u == null)
                {
                    continue;
                }

                var win = lost || IsWin(u);
                if (tableUser.IsMySelf(u))
                {
                    if (EndImage)
                    {
                        if (!EndImage.gameObject.activeSelf)
                        {
                            EndImage.gameObject.SetActive(true);
                        }

                        EndImage.sprite = win ? WinImg : LoseImg;
                    }

                    if (!_mySelfRank)
                    {
                        _mySelfRank = _rankMySelfFactory.Create();

                        if (_mySelfRank)
                        {
                            _mySelfRank.transform.SetParent(RankContent, false);
                        }
                    }

                    if (_mySelfRank)
                    {
                        if (!_mySelfRank.gameObject.activeSelf)
                        {
                            _mySelfRank.gameObject.SetActive(true);
                        }

                        var room = _room.Read();
                        var roomCurrencyType = room == null ? CurrencyType.GOLDEN_EGG : room.currency_type;
                        _mySelfRank.transform.SetSiblingIndex(i);
                        // TODO 完成连打房逻辑
                        _mySelfRank.ApplyData(
                            false,
                            win,
                            u.nickname,
                            u.vip,
                            GetMyExp(msg, i),
                            roomCurrencyType,
                            GetMyMoney(msg, i),
                            msg.race_id,
                            GetMyTotalScore(msg, i),
                            msg.total_multiple);
                    }
                }
                else
                {
                    if (currentOtherRank >= _otherRanks.Count)
                    {
                        var rank = _rankOtherFactory.Create();
                        if (rank)
                        {
                            rank.transform.SetParent(RankContent, false);
                            _otherRanks.Add(rank);
                        }
                    }

                    if (currentOtherRank < _otherRanks.Count)
                    {
                        var rank = _otherRanks[currentOtherRank];
                        if (!rank.gameObject.activeSelf)
                        {
                            rank.gameObject.SetActive(true);
                        }

                        currentOtherRank++;

                        rank.ApplyData(win, u.username, u.nickname, u.vip);
                        rank.transform.SetSiblingIndex(i);
                    }
                }
            }
        }

        public void RaceRefreshContent(com.morln.game.gd.command.RoundEnd msg)
        {
            if (msg == null)
            {
                return;
            }

            if (!RankContent)
            {
                return;
            }

            var raceResultItem = msg.item;
            if (raceResultItem == null)
            {
                return;
            }

            raceResultItem.Sort((x, y) => x.rank.CompareTo(y.rank));

            var myUser = _user.Read();
            var userName = myUser == null ? null : myUser.username;
            var selfIsWin = false;
            var selfScore = 0;
            var users = new List<UserNameAndNickName>();

            for (var i = 0; i < raceResultItem.Count; i++)
            {
                if (userName == raceResultItem[i].username)
                {
                    selfIsWin = raceResultItem[i].win;
                    var myCurrency = raceResultItem[i].currency;
                    if (myCurrency == null)
                    {
                        return;
                    }

                    //比赛房默认货币类型为分数
                    var currentRoomCurrencyType = CurrencyType.SCORE;
                    for (var j = 0; j < myCurrency.Count; j++)
                    {
                        if (myCurrency[j].type == currentRoomCurrencyType)
                        {
                            selfScore = (int) myCurrency[j].count;
                            break;
                        }
                    }
                }

                var user = new UserNameAndNickName();
                user.NickName = raceResultItem[i].nickname;
                user.Username = raceResultItem[i].username;
                users.Add(user);
            }

            if (selfIsWin)
            {
                // 掉线或者赢了，都播放赢的声音。
                _soundController.PlayEndRoundWinSound();
            }
            else
            {
                _soundController.PlayEndRoundLoseSound();
            }

            int currentOtherRank = 0;

            for (int i = 0; i < users.Count; i++)
            {
                var u = users[i];
                if (u == null)
                {
                    continue;
                }

                var win = false;
                for (var j = 0; j < raceResultItem.Count; j++)
                {
                    if (u.Username == raceResultItem[j].username)
                    {
                        win = raceResultItem[j].win;
                        break;
                    }
                }

                if (u.Username == userName)
                {
                    if (EndImage)
                    {
                        if (!EndImage.gameObject.activeSelf)
                        {
                            EndImage.gameObject.SetActive(true);
                        }

                        EndImage.sprite = win ? WinImg : LoseImg;
                    }

                    if (!_mySelfRank)
                    {
                        _mySelfRank = _rankMySelfFactory.Create();

                        if (_mySelfRank)
                        {
                            _mySelfRank.transform.SetParent(RankContent, false);
                        }
                    }

                    if (_mySelfRank)
                    {
                        if (!_mySelfRank.gameObject.activeSelf)
                        {
                            _mySelfRank.gameObject.SetActive(true);
                        }

                        var room = _room.Read();
                        var roomCurrencyType = room == null ? CurrencyType.GOLDEN_EGG : room.currency_type;
                        _mySelfRank.transform.SetSiblingIndex(i);
                        _mySelfRank.ApplyData(
                            false,
                            win,
                            u.NickName,
                            u.Vip,
                            0,
                            roomCurrencyType,
                            selfScore,
                            1,
                            GetMyRaceTotalScore(),
                            msg.total_multiple);
                    }
                }
                else
                {
                    if (currentOtherRank >= _otherRanks.Count)
                    {
                        var rank = _rankOtherFactory.Create();
                        if (rank)
                        {
                            rank.transform.SetParent(RankContent, false);
                            _otherRanks.Add(rank);
                        }
                    }

                    if (currentOtherRank < _otherRanks.Count)
                    {
                        var rank = _otherRanks[currentOtherRank];
                        if (!rank.gameObject.activeSelf)
                        {
                            rank.gameObject.SetActive(true);
                        }

                        currentOtherRank++;

                        rank.ApplyData(win, u.Username, u.NickName, u.Vip);
                        rank.transform.SetSiblingIndex(i);
                    }
                }
            }
        }

        private int GetMyExp(BRoundEnd msg, int rank)
        {
            if (msg == null)
            {
                return 0;
            }

            if (rank == 0)
            {
                return msg.final_exp1;
            }

            if (rank == 1)
            {
                return msg.final_exp2;
            }

            if (rank == 2)
            {
                return msg.final_exp3;
            }

            if (rank == 3)
            {
                return msg.final_exp4;
            }

            return 0;
        }

        private int GetMyMoney(BRoundEnd msg, int rank)
        {
            if (msg == null)
            {
                return 0;
            }

            if (rank == 0)
            {
                return msg.final_money1;
            }

            if (rank == 1)
            {
                return msg.final_money2;
            }

            if (rank == 2)
            {
                return msg.final_money3;
            }

            if (rank == 3)
            {
                return msg.final_money4;
            }

            return 0;
        }

        private int GetMyTotalScore(BRoundEnd msg, int rank)
        {
            if (msg == null)
            {
                return 0;
            }

            if (rank == 0)
            {
                return msg.total_score1;
            }

            if (rank == 1)
            {
                return msg.total_score2;
            }

            if (rank == 2)
            {
                return msg.total_score3;
            }

            if (rank == 3)
            {
                return msg.total_score4;
            }

            return 0;
        }

        private int GetMyRaceTotalScore()
        {
            var raceConfig = _raceConfig.Read();
            if (raceConfig == null)
            {
                return 0;
            }

            if (raceConfig.score == null)
            {
                return 0;
            }

            var totalScore = raceConfig.score.count;

            return (int) totalScore;
        }

        public void Reset()
        {
            if (EndImage.gameObject.activeSelf)
            {
                EndImage.gameObject.SetActive(false);
            }

            if (ReadyGroup.gameObject.activeSelf)
            {
                ReadyGroup.gameObject.SetActive(false);
            }

            if (_mySelfRank && _mySelfRank.gameObject.activeSelf)
            {
                _mySelfRank.gameObject.SetActive(false);
            }

            foreach (var rank in _otherRanks)
            {
                if (rank.gameObject.activeSelf)
                    rank.gameObject.SetActive(false);
            }

            if (!Panel.gameObject.activeSelf)
            {
                Panel.gameObject.SetActive(true);
            }

            Panel.alpha = 1;
        }

        #endregion

        #region 刷新KickOuterCounter

        public float KickOutCounterRefreshTime { get; private set; }

        private void RefreshKickOutCounter()
        {
            if (KickOutCounterRefreshTime >= _bKickOutCounter.Timestamp)
            {
                return;
            }

            KickOutCounterRefreshTime = _bKickOutCounter.Timestamp;

            var tableUser = _tableUserData.Read();
            var msg = _bKickOutCounter.Read();
            if (msg == null)
            {
                return;
            }

            if (msg.seat == tableUser.MySeat && msg.start_or_stop == 1)
            {
                // 是我自己需要准备了。

                _showKickOutCounter = true;

                if (!ReadyGroup.gameObject.activeSelf)
                {
                    ReadyGroup.gameObject.SetActive(true);
                }

                _kickOutCounterEndTime = _bKickOutCounter.Timestamp + msg.left_time;
            }
            else
            {
                _showKickOutCounter = false;

                if (ReadyGroup.gameObject.activeSelf)
                {
                    ReadyGroup.gameObject.SetActive(false);
                }
            }
        }

        private bool _showKickOutCounter;

        private float _kickOutCounterEndTime;

        private void UpdateKickOutCounterTime()
        {
            if (!_showKickOutCounter || !ReadyGroup.gameObject.activeSelf)
            {
                return;
            }

            if (_kickOutCounterEndTime < Time.time)
            {
                if (ReadyGroup.gameObject.activeSelf)
                {
                    ReadyGroup.gameObject.SetActive(false);
                }

                _showKickOutCounter = false;
                return;
            }

            var leftTime = Mathf.RoundToInt(_kickOutCounterEndTime - Time.time);
            ReadyLeftTime.text = "还剩" + leftTime + "秒";
        }

        #endregion

        /// <summary>
        /// 再来一局。
        /// </summary>
        public void Ready()
        {
            // 向服务器发送准备命令。
            _dialogManager.ShowWaitingDialog(true);
            _readyResult.ClearNotInvalidate();

            _remoteAPI.Ready();
            _taskManager.ExecuteTask(
                CheckReadyState,
                () => { _dialogManager.ShowWaitingDialog(false); });
        }

        private bool CheckReadyState()
        {
            if (_readyResult.Read() == null)
            {
                return false;
            }

            _dialogManager.ShowWaitingDialog(false);
            var myUser = _user.Read();
            bool selfready = myUser == null ? false : (myUser.ready == 1);
            if (selfready)
            {
                // 如果玩家已经准备了，则隐藏结算面板。
                RoundEndPanel.Hide();
            }

            return true;
        }

        /// <summary>
        /// 返回大厅的功能。
        /// </summary>
        public void BackToHall()
        {
            _dialogManager.ShowConfirmBox(
                "真的要离开牌局吗？",
                true, "退出", () =>
                {
                    if (!_appController.IsSingleGameMode())
                    {
                        var room = _room.Read();
                        var currentRoomType = room == null ? RoomType.Match : room.type;
                        if (currentRoomType == RoomType.Normal)
                        {
                            // 选桌房，离桌。
                            _remoteAPI.LeaveTable(false);
                        }
                        else
                        {
                            // 其他房间直接离开房间。
                            _remoteAPI.LeaveRoom(false);
                        }
                    }
                    else
                    {
                        _appController.ExitSingleGame();
                    }
                },
                true, "继续游戏", null,
                true, true, true);
        }

        #region 隐藏结算面板

        public CanvasGroup Panel;

        public void ClosePanel()
        {
            RoundEndPanel.Hide();
        }

        #endregion
    }
}