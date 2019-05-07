using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Network;
using Dmm.Report;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    /// <summary>
    /// 玩家座位。
    /// </summary>
    public class PlayerSeat : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private IDataContainer<TableUserData> _tableUserData;

        private IDataContainer<Room> _room;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
        }

        #endregion

        public void Update()
        {
            RefreshKickOutCounter();
        }

        /// <summary>
        /// Parent Seat Panel.
        /// </summary>
        public SeatPanel SeatPanel;

        /// <summary>
        /// Seat的编号。
        /// </summary>
        public int SeatNumber;

        public GameObject KickOutCounterGroup;

        public Text KickOutCounterTxt;

        /// <summary>
        /// 玩家自身指示器。
        /// </summary>
        public Image SelfIndicator;

        /// <summary>
        /// 已经准备。
        /// </summary>
        public GameObject ReadyIndicator;

        /// <summary>
        /// 玩家的等级。
        /// </summary>
        public Text Level;

        /// <summary>
        /// 玩家的等级Title。
        /// </summary>
        public Text LevelTitle;

        /// <summary>
        /// 玩家的胜率。
        /// </summary>
        public Text WinRate;

        /// <summary>
        /// 玩家的财富。
        /// </summary>
        public CurrencyValue Money;

        /// <summary>
        /// 昵称。
        /// </summary>
        public NicknameGroup Nickname;

        /// <summary>
        /// 玩家对象。
        /// </summary>
        public PlayerFigure Player;

        /// <summary>
        /// Seat function btn group.
        /// </summary>
        public RectTransform FuncGroup;

        /// <summary>
        /// 玩家的数据。
        /// </summary>
        public User Data { get; private set; }

        /// <summary>
        /// 按钮。
        /// </summary>
        public Button Button;

        /// <summary>
        /// 清空座位。
        /// </summary>
        public void Clear()
        {
            Data = null;

            ShowFuncGroup(false);

            if (SelfIndicator && SelfIndicator.gameObject.activeSelf)
            {
                SelfIndicator.gameObject.SetActive(false);
            }

            if (ReadyIndicator && ReadyIndicator.gameObject.activeSelf)
            {
                ReadyIndicator.gameObject.SetActive(false);
            }

            if (Level)
            {
                Level.text = null;
            }
            if (LevelTitle)
            {
                LevelTitle.text = null;
            }
            if (WinRate)
            {
                WinRate.text = null;
            }
            if (Money)
            {
                Money.Clear();
            }
            if (Nickname)
            {
                Nickname.Clear();
            }
            if (Player)
            {
                Player.Clear();
            }
        }

        /// <summary>
        /// 应用玩家数据。
        /// </summary>
        /// <param name="data"></param>
        public void ApplyData(User data)
        {
            if (data == null)
            {
                Data = null;

                // Close function group when clear data.
                ShowFuncGroup(false);
                ShowKickOutCounter(false);

                if (SelfIndicator && SelfIndicator.gameObject.activeSelf)
                {
                    SelfIndicator.gameObject.SetActive(false);
                }

                if (ReadyIndicator && ReadyIndicator.gameObject.activeSelf)
                {
                    ReadyIndicator.gameObject.SetActive(false);
                }

                if (Level)
                {
                    Level.text = "";
                }
                if (LevelTitle)
                {
                    LevelTitle.text = "";
                }
                if (WinRate)
                {
                    WinRate.text = "";
                }
                if (Money)
                {
                    Money.Clear();
                }
                if (Nickname)
                {
                    Nickname.Clear();
                }
                if (Player)
                {
                    Player.Clear();
                }
            }
            else
            {
                // 之前的数据是空，或者数据发生了变化，则关闭倒计时和功能按钮。
                if (Data == null || !StringUtil.AreEqual(data.username, Data.username))
                {
                    // FIXME bug就出在这里了。
                    ShowFuncGroup(false);
                    ShowKickOutCounter(false);
                }

                Data = data;
                var tableUser = _tableUserData.Read();
                var self = tableUser.IsMySelf(data);

                if (SelfIndicator && SelfIndicator.gameObject.activeSelf != self)
                {
                    SelfIndicator.gameObject.SetActive(self);
                }

                var ready = data.ready == 1;
                if (ReadyIndicator && ReadyIndicator.gameObject.activeSelf != ready)
                {
                    ReadyIndicator.gameObject.SetActive(ready);
                }

                if (Level)
                {
                    Level.text = "" + data.level;
                }
                if (LevelTitle)
                {
                    LevelTitle.text = "" + data.title;
                }
                if (WinRate)
                {
                    float rate = data.round_count != 0 ? ((float) data.win_count) / data.round_count : 0;
                    WinRate.text = DataUtil.FormatWinRate(rate);
                }
                if (Money)
                {
                    var room = _room.Read();
                    var currencyType = room == null ? CurrencyType.GOLDEN_EGG : room.currency_type;
                    Money.SetCurrency(DataUtil.GetCurrency(data, currencyType), currencyType);
                }

                if (Nickname)
                {
                    Nickname.SetData(data);
                }
                if (Player)
                {
                    Player.ApplyData(data);
                }
            }
        }

        public void ShowFuncGroup(bool show)
        {
            if (FuncGroup && FuncGroup.gameObject.activeSelf != show)
            {
                FuncGroup.gameObject.SetActive(show);
                if (show && SeatPanel)
                {
                    SeatPanel.OnSeatShowFuncGroup(this);
                }
            }
        }

        private bool _showKickOutCounter;

        private float _kickOutEndTime;

        public void ShowKickOutCounter(bool show, float startTime = 0, float time = 0)
        {
            if (!KickOutCounterGroup)
            {
                return;
            }

            _showKickOutCounter = show;

            if (show)
            {
                if (!KickOutCounterGroup.activeSelf)
                {
                    KickOutCounterGroup.SetActive(true);
                }

                _kickOutEndTime = startTime + time;
            }
            else
            {
                if (KickOutCounterGroup.activeSelf)
                {
                    KickOutCounterGroup.SetActive(false);
                }
            }
        }

        private void RefreshKickOutCounter()
        {
            if (!_showKickOutCounter || !KickOutCounterGroup || !KickOutCounterGroup.activeSelf)
            {
                return;
            }

            if (_kickOutEndTime < Time.time)
            {
                ShowKickOutCounter(false);
                return;
            }

            if (KickOutCounterTxt)
            {
                var leftTime = Mathf.RoundToInt(_kickOutEndTime - Time.time);
                KickOutCounterTxt.text = "倒计时: " + leftTime;
            }
        }

        public void OnClick()
        {
            var tableUser = _tableUserData.Read();

            if (Data != null)
            {
                if (!tableUser.IsMySelf(Data))
                {
                    ShowFuncGroup(true);
                }
            }
            else
            {
                // 座位是空的时候，就选择座位。
                if (!tableUser.SelfReady())
                {
                    _remoteAPI.ChooseSeat(SeatNumber);
                }
                else
                {
                    _dialogManager.ShowToast("准备之后就不能换座位了", 1);
                }
            }
        }

        public void AddFriend()
        {
            if (Data == null)
            {
                return;
            }

            _remoteAPI.AddFriend(Data.username);
            _dialogManager.ShowToast("已向对方发送了好友请求\n等待对方接受", 3, false);
            ShowFuncGroup(false);
        }

        public void ShowPlayerInfo()
        {
            if (Data == null)
            {
                return;
            }

            _dialogManager.ShowDialog<OtherInfoPanel>(DialogName.OtherInfoPanel, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(Data, false, false);
                    dialog.Show();
                }
            );
        }

        public void KickOut()
        {
            _remoteAPI.KickOut(SeatNumber);
            ShowFuncGroup(false);
        }

        public void Report()
        {
            if (Data == null)
            {
                _dialogManager.ShowToast("举报玩家失败", 2, true);
                return;
            }

            _dialogManager.ShowDialog<ReportDialog>(DialogName.ReportDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(Data);
                    dialog.Show();
                });
        }
    }
}