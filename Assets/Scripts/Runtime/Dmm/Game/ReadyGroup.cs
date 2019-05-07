using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Hall;
using Dmm.Network;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    public class ReadyGroup : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IAppController _appController;

        private IDialogManager _dialogManager;

        private IDataContainer<TableUserData> _tableUserData;

        private IDataContainer<User> _user;

        private IDataContainer<BKickOutCounter> _bKickOutCounter;

        private IDataContainer<Room> _room;

        [Inject]
        public void Initialize(
            IAppController appController,
            IDialogManager dialogManager,
            IDataRepository dataRepository,
            RemoteAPI remoteAPI)
        {
            _appController = appController;
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _bKickOutCounter = dataRepository.GetContainer<BKickOutCounter>(DataKey.BKickOutCounter);
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
        }

        #endregion

        public Image ReadyTop;

        public GameObject KickOutTop;

        public Text KickOutTopText;

        public Image ReadyBottom;

        public GameObject KickOutBottom;

        public Text KickOutBottomText;

        public Image ReadyLeft;

        public GameObject KickOutLeft;

        public Text KickOutLeftText;

        public Image ReadyRight;

        public GameObject KickOutRight;

        public Text KickOutRightText;

        /// <summary>
        /// 返回按钮。
        /// </summary>
        public Button BackBtn;

        public Button ReadyBtn;

        public void OnEnable()
        {
            ReadyBtn.interactable = false;

            if (ReadyTop.gameObject.activeSelf)
            {
                ReadyTop.gameObject.SetActive(false);
            }

            if (KickOutTop.activeSelf)
            {
                KickOutTop.SetActive(false);
            }

            if (ReadyBottom.gameObject.activeSelf)
            {
                ReadyBottom.gameObject.SetActive(false);
            }

            if (KickOutBottom.activeSelf)
            {
                KickOutBottom.SetActive(false);
            }

            if (ReadyLeft.gameObject.activeSelf)
            {
                ReadyLeft.gameObject.SetActive(false);
            }

            if (KickOutLeft.activeSelf)
            {
                KickOutLeft.SetActive(false);
            }

            if (ReadyRight.gameObject.activeSelf)
            {
                ReadyRight.gameObject.SetActive(false);
            }

            if (KickOutRight.activeSelf)
            {
                KickOutRight.SetActive(false);
            }
        }

        public void Update()
        {
            RefreshContent();
            RefreshKickOut();

            UpdateKickOutCounter();
        }

        public float ContentRefreshTime { get; private set; }

        private void RefreshContent()
        {
            if (ContentRefreshTime >= _tableUserData.Timestamp)
            {
                return;
            }

            ContentRefreshTime = _tableUserData.Timestamp;

            var myUser = _user.Read();
            var tableUser = _tableUserData.Read();
            var selfReady = myUser != null && myUser.ready == 1;

            // 未准备的情况，显示准备按钮。
            ReadyBtn.interactable = !selfReady;

            SetUserReady(ReadyTop, tableUser.UserTop(), KickOutTop);
            SetUserReady(ReadyBottom, tableUser.UserBottom(), KickOutBottom);
            SetUserReady(ReadyLeft, tableUser.UserLeft(), KickOutLeft);
            SetUserReady(ReadyRight, tableUser.UserRight(), KickOutRight);
        }

        private void SetUserReady(Image ready, User user, GameObject kickOut)
        {
            if (!ready) return;

            if (user != null)
            {
                var r = user.ready == 1;
                if (ready.gameObject.activeSelf != r)
                {
                    ready.gameObject.SetActive(r);
                }

                if (r && kickOut && kickOut.activeSelf)
                {
                    kickOut.SetActive(false);
                }
            }
            else
            {
                if (ready.gameObject.activeSelf)
                {
                    ready.gameObject.SetActive(false);
                }

                if (kickOut && kickOut.activeSelf)
                {
                    kickOut.SetActive(false);
                }
            }
        }

        public float KickOutRefreshTime { get; private set; }

        private bool _kickOut;

        private float _kickOutCounterEndTime;

        private GameObject _curKickOut;

        private Text _curKickOutText;

        private void RefreshKickOut()
        {
            if (KickOutRefreshTime >= _bKickOutCounter.Timestamp)
            {
                return;
            }

            KickOutRefreshTime = _bKickOutCounter.Timestamp;

            var data = _bKickOutCounter.Read();
            var tableUser = _tableUserData.Read();
            if (data == null)
            {
                return;
            }

            _kickOut = data.start_or_stop == 1;

            if (_kickOut)
            {
                _kickOutCounterEndTime = KickOutRefreshTime + data.left_time;
                var seat = tableUser.PositionOfSeat(data.seat);
                switch (seat)
                {
                    case SeatPosition.Bottom:
                        _curKickOut = KickOutBottom;
                        _curKickOutText = KickOutBottomText;
                        break;

                    case SeatPosition.Right:
                        _curKickOut = KickOutRight;
                        _curKickOutText = KickOutRightText;
                        break;

                    case SeatPosition.Top:
                        _curKickOut = KickOutTop;
                        _curKickOutText = KickOutTopText;
                        break;

                    case SeatPosition.Left:
                        _curKickOut = KickOutLeft;
                        _curKickOutText = KickOutLeftText;
                        break;
                }
            }
            else
            {
                if (_curKickOut && _curKickOut.activeSelf)
                {
                    _curKickOut.SetActive(false);
                }
            }
        }

        private void UpdateKickOutCounter()
        {
            if (!_kickOut || !_curKickOut || !_curKickOutText)
            {
                return;
            }

            if (!_curKickOut.activeSelf)
            {
                _curKickOut.SetActive(true);
            }

            var leftTime = _kickOutCounterEndTime - Time.time;
            if (leftTime <= 0)
            {
                _kickOut = false;
                _curKickOut.SetActive(false);
                return;
            }

            _curKickOutText.text = "倒计时: " + Mathf.RoundToInt(leftTime);
        }

        public void Ready()
        {
            _remoteAPI.Ready();
        }

        public void BackToHall()
        {
            _dialogManager.ShowConfirmBox(
                "真的要离开牌局吗？",
                true, "退出", () =>
                {
                    if (!_appController.IsSingleGameMode())
                    {
                        var currentRoom = _room.Read();
                        var currentRoomType = currentRoom == null ? RoomType.Match : currentRoom.type;
                        if (currentRoomType == RoomType.Normal)
                        {
                            _remoteAPI.LeaveTable(false);
                        }
                        else
                        {
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
    }
}