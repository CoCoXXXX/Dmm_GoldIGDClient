using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Hall;
using Dmm.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    /// <summary>
    /// 桌面钟用于显示倒计时。
    /// </summary>
    public class TableClock : MonoBehaviour
    {
        #region Inject

        private IAppController _appController;

        private ISoundController _soundController;

        private IDataContainer<PlayingData> _playingData;

        private IDataContainer<TableUserData> _tableUserData;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IAppController appController,
            ISoundController soundController)
        {
            _appController = appController;
            _soundController = soundController;
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        #endregion

        #region Unity方法

        public void Update()
        {
            CounterDownTime();
        }

        #endregion

        #region 刷新整体

        private float TotalRefreshTime { get; set; }

        public void RefreshTotal()
        {
            if (TotalRefreshTime >= _playingData.Timestamp)
            {
                return;
            }

            TotalRefreshTime = _playingData.Timestamp;

            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return;
            }

            var period = playingData.period;
            var tableUser = _tableUserData.Read();
            if (period == TablePeriod.Waiting)
            {
                Hide(_pos);
                Hide(_pos1);
            }
            else if (period == TablePeriod.JinGong)
            {
                // 如果存在seat != -1，但是poker == -1的情况，则显示箭头。

                var jgSeat1 = playingData.jg_seat1;
                var jgPoker1 = GameUtil.GetPokerFromPokerId(playingData.jg_poker1);
                var jgPos1 = jgSeat1 != -1 && jgPoker1 == null ? tableUser.PositionOfSeat(jgSeat1) : SeatPosition.Null;

                var jgSeat2 = playingData.jg_seat2;
                var jgPoker2 = GameUtil.GetPokerFromPokerId(playingData.jg_poker2);
                var jgPos2 = jgSeat2 != -1 && jgPoker2 == null ? tableUser.PositionOfSeat(jgSeat2) : SeatPosition.Null;

                Show(playingData.left_time, jgPos1, jgPos2);
            }
            else if (period == TablePeriod.HuanGong)
            {
                // 如果存在seat != -1，但是poker == -1的情况，则显示箭头。
                var hgSeat1 = playingData.hg_seat1;
                var hgPoker1 = GameUtil.GetPokerFromPokerId(playingData.hg_poker1);
                var hgPos1 = hgSeat1 != -1 && hgPoker1 == null ? tableUser.PositionOfSeat(hgSeat1) : SeatPosition.Null;

                var hgSeat2 = playingData.hg_seat2;
                var hgPoker2 = GameUtil.GetPokerFromPokerId(playingData.hg_poker2);
                var hgPos2 = hgSeat2 != -1 && hgPoker2 == null ? tableUser.PositionOfSeat(hgSeat2) : SeatPosition.Null;

                Show(playingData.left_time, hgPos1, hgPos2);
            }
            else if (period == TablePeriod.ChuPai)
            {
                var curChuPaiSeat = playingData.chupai_key_owner_seat;
                if (curChuPaiSeat != -1)
                {
                    Show(playingData.left_time, tableUser.PositionOfSeat(curChuPaiSeat));
                }
            }
        }

        #endregion

        #region 显示和隐藏

        public RectTransform Content;

        public Text TimeCounter;

        public RectTransform Arrow;

        private SeatPosition _pos;

        public RectTransform Arrow1;

        private SeatPosition _pos1;

        public float AngleOffset = 90;

        private float _endTime;

        public void Show(
            int leftTime,
            SeatPosition seat0,
            SeatPosition seat1 = SeatPosition.Null)
        {
            if (Content && !Content.gameObject.activeSelf)
            {
                Content.gameObject.SetActive(true);
            }

            _endTime = Time.time + leftTime;

            if (seat0 != SeatPosition.Null)
            {
                _pos = seat0;
                float angle = SeatAngle(seat0);
                if (Arrow)
                {
                    if (!Arrow.gameObject.activeSelf)
                    {
                        Arrow.gameObject.SetActive(true);
                    }

                    Arrow.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
            else
            {
                if (Arrow && Arrow.gameObject.activeSelf)
                {
                    Arrow.gameObject.SetActive(false);
                }

                _pos = SeatPosition.Null;
            }

            if (seat1 != SeatPosition.Null)
            {
                _pos1 = seat1;
                float angle = SeatAngle(seat1);
                if (Arrow1)
                {
                    if (!Arrow1.gameObject.activeSelf)
                    {
                        Arrow1.gameObject.SetActive(true);
                    }

                    Arrow1.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
            else
            {
                if (Arrow1 && Arrow1.gameObject.activeSelf)
                {
                    Arrow1.gameObject.SetActive(false);
                }

                _pos1 = SeatPosition.Null;
            }

            // 单机模式下不显示时间计数。
            if (TimeCounter &&
                TimeCounter.gameObject.activeSelf == _appController.IsSingleGameMode())
            {
                TimeCounter.gameObject.SetActive(!_appController.IsSingleGameMode());
            }
        }

        private float SeatAngle(SeatPosition pos)
        {
            float angle = 0;
            switch (pos)
            {
                case SeatPosition.Top:
                    angle = (0 + AngleOffset) % 360;
                    break;

                case SeatPosition.Left:
                    angle = (90 + AngleOffset) % 360;
                    break;

                case SeatPosition.Bottom:
                    angle = (180 + AngleOffset) % 360;
                    break;

                case SeatPosition.Right:
                    angle = (270 + AngleOffset) % 360;
                    break;
            }

            return angle;
        }

        public void Hide(SeatPosition pos)
        {
            if (pos == SeatPosition.Null)
            {
                return;
            }

            if (pos == _pos)
            {
                if (Arrow && Arrow.gameObject.activeSelf)
                {
                    Arrow.gameObject.SetActive(false);
                }

                _pos = SeatPosition.Null;
            }
            else if (pos == _pos1)
            {
                if (Arrow1 && Arrow1.gameObject.activeSelf)
                {
                    Arrow1.gameObject.SetActive(false);
                }

                _pos1 = SeatPosition.Null;
            }

            if (_pos == SeatPosition.Null && _pos1 == SeatPosition.Null)
            {
                // 如果两个箭头都被隐藏了，则隐藏整体时钟。
                if (Content && Content.gameObject.activeSelf)
                {
                    Content.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region 快到时间的提示音

        private float _lastHurrySountTime;

        private void CounterDownTime()
        {
            if (_appController.IsSingleGameMode())
                // 单机模式下不计时。
            {
                return;
            }

            if (Content && Content.gameObject.activeSelf)
            {
                var leftTime = Mathf.RoundToInt(_endTime - Time.time);
                if (leftTime < 0)
                {
                    leftTime = 0;
                }

                if (TimeCounter)
                {
                    TimeCounter.text = "" + leftTime;
                }

                if (leftTime < 10)
                {
                    if (Time.time - _lastHurrySountTime >= 1)
                    {
                        _soundController.PlayHurrySound();
                        _lastHurrySountTime = Time.time;
                    }
                }
            }
        }

        #endregion
    }
}