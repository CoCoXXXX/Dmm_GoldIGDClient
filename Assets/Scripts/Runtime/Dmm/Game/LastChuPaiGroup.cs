using System.Collections.Generic;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Hall;
using Dmm.PokerLogic;
using Dmm.Sound;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using PatternMatcher = Dmm.PokerLogic.PatternMatcher;
using PokerPattern = com.morln.game.gd.command.PokerPattern;

namespace Dmm.Game
{
    public class LastChuPaiGroup : MonoBehaviour
    {
        #region Inject

        private ISoundController _soundController;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<HostInfoResult> _hostInfo;

        private IDataContainer<PlayingData> _playingData;

        private IDataContainer<User> _myUser;

        private IDataContainer<BKangGong> _kangGong;

        [Inject]
        public void Initialize(ISoundController soundController, IDataRepository dataRepository)
        {
            _soundController = soundController;

            _kangGong = dataRepository.GetContainer<BKangGong>(DataKey.BKangGong);
            _hostInfo = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        #endregion

        #region Unity方法

        public void OnEnable()
        {
            ClearAllSeat();
        }

        public void Update()
        {
            RefreshTotal();

            RefreshKangGong();
        }

        #endregion

        #region 刷新整体

        // 发生断线重连的时候，会出现PlayingData整体数据的刷新，此时应该根据PlayingData的内容，对整体进行一次刷新。

        /// <summary>
        /// 整体刷新的时间。
        /// </summary>
        public float TotalRefreshTime { get; private set; }

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
            if (period != TablePeriod.ChuPai &&
                period != TablePeriod.EndRound &&
                period != TablePeriod.BetweenRound)
            {
                return;
            }

            var tableUser = _tableUser.Read();

            var curChuPaiSeat = playingData.chupai_key_owner_seat;
            for (int i = 0; i < 4; i++)
            {
                if (i == curChuPaiSeat)
                {
                    // 当前出牌座位不显示上一次出牌。
                    ClearSeat(tableUser.PositionOfSeat(curChuPaiSeat));
                    continue;
                }

                var pos = tableUser.PositionOfSeat(i);

                var lastChuPai = playingData.LastChuPaiOfSeat(i);
                if (lastChuPai == null)
                {
                    ClearSeat(pos);
                    continue;
                }

                var user = tableUser.GetUserAtSeat(i);
                var isWithEffect = playingData.last_chu_pai_seat == i;
                SetLastChuPai(pos, lastChuPai, user != null ? user.sex : 0, isWithEffect);
            }

            // 设置头游和二游
            var currentRank = playingData.current_rank;
            if (currentRank.Count > 0)
            {
                var touYouSeat = currentRank[0];
                var touYouPos = tableUser.PositionOfSeat(touYouSeat);
                var erYouSeat = currentRank.Count > 1 ? currentRank[1] : -1;
                var erYouPos = tableUser.PositionOfSeat(erYouSeat);

                SetTouYou(touYouPos, erYouPos);
            }
            else
            {
                HideAllTouYouImage();
            }
        }

        #endregion

        #region 头游

        public Sprite TouYouSprite;

        public Sprite ErYouSprite;

        public Image TopTouYou;

        public Image LeftTouYou;

        public Image RightTouYou;

        public Image BottomTouYou;

        public float TouYouAnimationTime = 1;

        private Tweener _touYouTweener;

        private Tweener _erYouTweener;

        public void HideAllTouYouImage()
        {
            if (LeftTouYou.gameObject.activeSelf)
            {
                LeftTouYou.gameObject.SetActive(false);
            }

            if (RightTouYou.gameObject.activeSelf)
            {
                RightTouYou.gameObject.SetActive(false);
            }

            if (TopTouYou.gameObject.activeSelf)
            {
                TopTouYou.gameObject.SetActive(false);
            }

            if (BottomTouYou.gameObject.activeSelf)
            {
                BottomTouYou.gameObject.SetActive(false);
            }
        }

        private void SetTouYou(SeatPosition touYouPos, SeatPosition erYouPos = SeatPosition.Null)
        {
            Image touYouImg = null;
            Image erYouImg = null;

            if (SeatPosition.Top == touYouPos)
            {
                touYouImg = TopTouYou;
            }
            else if (SeatPosition.Top == erYouPos)
            {
                erYouImg = TopTouYou;
            }
            else
            {
                if (TopTouYou.gameObject.activeSelf)
                {
                    TopTouYou.gameObject.SetActive(false);
                }
            }

            if (SeatPosition.Bottom == touYouPos)
            {
                touYouImg = BottomTouYou;
            }
            else if (SeatPosition.Bottom == erYouPos)
            {
                erYouImg = BottomTouYou;
            }
            else
            {
                if (BottomTouYou.gameObject.activeSelf)
                {
                    BottomTouYou.gameObject.SetActive(false);
                }
            }

            if (SeatPosition.Left == touYouPos)
            {
                touYouImg = LeftTouYou;
            }
            else if (SeatPosition.Left == erYouPos)
            {
                erYouImg = LeftTouYou;
            }
            else
            {
                if (LeftTouYou.gameObject.activeSelf)
                {
                    LeftTouYou.gameObject.SetActive(false);
                }
            }

            if (SeatPosition.Right == touYouPos)
            {
                touYouImg = RightTouYou;
            }
            else if (SeatPosition.Right == erYouPos)
            {
                erYouImg = RightTouYou;
            }
            else
            {
                if (RightTouYou.gameObject.activeSelf)
                {
                    RightTouYou.gameObject.SetActive(false);
                }
            }

            if (touYouImg != null)
            {
                touYouImg.sprite = TouYouSprite;
                if (!touYouImg.gameObject.activeSelf)
                {
                    if (_touYouTweener != null)
                    {
                        _touYouTweener.Kill();
                        _touYouTweener = null;
                    }

                    touYouImg.gameObject.SetActive(true);
                    touYouImg.rectTransform.localScale = Vector2.zero;

                    _touYouTweener = touYouImg.rectTransform
                        .DOScale(Vector2.one, TouYouAnimationTime)
                        .SetEase(Ease.OutBack);
                }
            }

            if (erYouImg != null)
            {
                erYouImg.sprite = ErYouSprite;
                if (!erYouImg.gameObject.activeSelf)
                {
                    if (_erYouTweener != null)
                    {
                        _erYouTweener.Kill();
                        _erYouTweener = null;
                    }

                    erYouImg.gameObject.SetActive(true);
                    erYouImg.rectTransform.localScale = Vector2.zero;
                    _erYouTweener = erYouImg.rectTransform
                        .DOScale(Vector2.one, TouYouAnimationTime)
                        .SetEase(Ease.OutBack);
                }
            }
        }

        #endregion

        #region 抗贡

        public float KangGongRefreshTime { get; private set; }

        public void RefreshKangGong()
        {
            if (KangGongRefreshTime >= _kangGong.Timestamp)
            {
                return;
            }

            KangGongRefreshTime = _kangGong.Timestamp;

            var data = _kangGong.Read(true);
            if (data != null)
            {
                ShowKangGong(data);
            }
        }

        private void ShowKangGong(BKangGong msg)
        {
            if (msg == null)
            {
                return;
            }

            var tableUser = _tableUser.Read();
            var seat1 = tableUser.PositionOfSeat(msg.seat1);
            var seat2 = tableUser.PositionOfSeat(msg.seat2);

            ClearAllBuChuImage(seat1, seat2);

            if (seat1 != SeatPosition.Null)
            {
                var tweener = GetBuChuTweener(seat1);

                if (tweener != null)
                {
                    tweener.Kill();
                }

                var image = GetBuChuImage(seat1);
                if (image)
                {
                    image.sprite = KangGongSprite;
                    tweener = StartBuChuAnimation(image.transform);
                    SetBuChuTweener(seat1, tweener);
                }
            }

            if (seat2 != SeatPosition.Null)
            {
                var tweener = GetBuChuTweener(seat2);

                if (tweener != null)
                {
                    tweener.Kill();
                }

                var image = GetBuChuImage(seat2);
                if (image)
                {
                    image.sprite = KangGongSprite;
                    tweener = StartBuChuAnimation(image.transform);
                    SetBuChuTweener(seat2, tweener);
                }
            }
        }

        /// <summary>
        /// 清空桌面所有的不出图片
        /// </summary>
        private void ClearAllBuChuImage(params SeatPosition[] excludes)
        {
            for (int i = 0; i < _allSeatPositions.Length; i++)
            {
                var pos = _allSeatPositions[i];
                var excluded = false;
                if (excludes != null && excludes.Length > 0)
                {
                    for (int e = 0; e < excludes.Length; e++)
                    {
                        if (excludes[e] != SeatPosition.Null &&
                            excludes[e] == pos)
                        {
                            excluded = true;
                            break;
                        }
                    }
                }

                if (!excluded)
                {
                    var buChu = GetBuChuImage(pos);
                    if (buChu && buChu.gameObject.activeSelf)
                    {
                        buChu.gameObject.SetActive(false);
                    }
                }
            }
        }

        #endregion

        #region 进还贡

        #endregion

        #region 出牌请求

        private readonly SeatPosition[] _allSeatPositions =
        {
            SeatPosition.Bottom,
            SeatPosition.Right,
            SeatPosition.Top,
            SeatPosition.Left
        };

        /// <summary>
        /// 清空所有的座位。
        /// </summary>
        /// <param name="excludes"></param>
        public void ClearAllSeat(params SeatPosition[] excludes)
        {
            for (int i = 0; i < _allSeatPositions.Length; i++)
            {
                var pos = _allSeatPositions[i];
                var excluded = false;
                if (excludes != null && excludes.Length > 0)
                {
                    for (int e = 0; e < excludes.Length; e++)
                    {
                        if (excludes[e] != SeatPosition.Null &&
                            excludes[e] == pos)
                        {
                            excluded = true;
                            break;
                        }
                    }
                }

                if (!excluded) ClearSeat(pos);
            }
        }

        public void ClearSeat(SeatPosition pos)
        {
            var buChu = GetBuChuImage(pos);
            if (buChu && buChu.gameObject.activeSelf)
            {
                buChu.gameObject.SetActive(false);
            }

            var lastChuPai = GetLastChuPaiSlots(pos);
            if (lastChuPai != null && lastChuPai.Count > 0)
            {
                for (int i = 0; i < lastChuPai.Count; i++)
                {
                    SetSlotIdle(lastChuPai[i]);
                }

                lastChuPai.Clear();
            }

            SetLastChuPai(pos, null);
        }

        #endregion

        #region Sprite

        public Sprite BuChuSprite;

        public Sprite KangGongSprite;

        #endregion

        #region 四个角的不出图片

        public Image BuChuTop;

        public Image BuChuLeft;

        public Image BuChuRight;

        public Image BuChuBottom;

        private readonly Dictionary<SeatPosition, Tweener> _buChuTweeners = new Dictionary<SeatPosition, Tweener>();

        #endregion

        #region 上一次出牌

        public float BuChuAnimationTime = 0.4f;

        public GameWindow GameWindow;

        public Vector2 MyLastChuPaiPos;

        public Vector2 TopLastChuPaiPos;

        public Vector2 LeftLastChuPaiPos;

        public Vector2 RightLastChuPaiPos;

        private readonly List<LastCardSlot> _bottomSlots = new List<LastCardSlot>();

        private List<Poker> _bottomeChuPai;

        private readonly List<LastCardSlot> _topSlots = new List<LastCardSlot>();

        private List<Poker> _topChuPai;

        private readonly List<LastCardSlot> _leftSlots = new List<LastCardSlot>();

        private List<Poker> _leftChuPai;

        private readonly List<LastCardSlot> _rightSlots = new List<LastCardSlot>();

        private List<Poker> _rightChuPai;

        private void SetLastChuPai(SeatPosition seatPos, PokerPattern lastChuPai, int sex, bool withEffect = true)
        {
            if (lastChuPai == null)
            {
                return;
            }

            var value = new PatternValue(GetCurrentHost);
            var matcher = new PatternMatcher(value);
            var pattern = matcher.Match(PokerLogicUtil.ToPokerList(lastChuPai.pokers));
            var pokers = PokerLogicUtil.ConvertToPokerList(pattern, value);
            SetLastChuPai(seatPos, pokers, sex, withEffect);
        }

        private void SetLastChuPai(SeatPosition seatPos, List<Poker> lastChuPai, int sex, bool withEffect = true)
        {
            if (seatPos == SeatPosition.Null)
            {
                return;
            }

            var chuPai = GetLastChuPai(seatPos);
            if (PokerLogicUtil.PokerListEqual(chuPai, lastChuPai))
            {
                // 如果现在要显示的牌，和当前的牌相等的话，就不需要再显示了。
                return;
            }

            SetLastChuPai(seatPos, lastChuPai);

            var slots = GetLastChuPaiSlots(seatPos);
            var buChu = GetBuChuImage(seatPos);
            var basePos = GetBasePos(seatPos);

            if (slots.Count > 0)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    SetSlotIdle(slots[i]);
                }

                slots.Clear();
            }

            if (lastChuPai == null || lastChuPai.Count <= 0)
            {
                var tweener = GetBuChuTweener(seatPos);

                if (tweener != null)
                {
                    tweener.Kill();
                }

                if (buChu)
                {
                    buChu.sprite = BuChuSprite;
                    if (!buChu.gameObject.activeSelf)
                    {
                        // 只有没显示的情况下，才显示动画。
                        tweener = StartBuChuAnimation(buChu.transform);
                        SetBuChuTweener(seatPos, tweener);
                    }
                }

                if (withEffect)
                {
                    _soundController.PlayBuChuSound();
                    _soundController.PlayChuPaiSound(PokerLogic.PokerPattern.BUCHU, sex);
                }

                return;
            }

            if (buChu && buChu.gameObject.activeSelf)
            {
                buChu.gameObject.SetActive(false);
            }

            foreach (var p in lastChuPai)
            {
                var slot = GetSlot();
                if (slot)
                {
                    slot.Poker = p;
                    slots.Add(slot);
                }
            }

            // SortSlot(slots);
            UpdateSlotPosition(basePos, slots);

            if (withEffect)
            {
                var value = new PatternValue(GetCurrentHost);
                var matcher = new PatternMatcher(value);

                var matched = matcher.Match(lastChuPai);
                if (matched != null && !matched.IsNull)
                {
                    _soundController.PlayChuPaiSound(matched, sex);
                    GameWindow.ShowChuPaiEffect(matched);
                }
            }
        }

        public void SetMyLastChuPai(PokerLogic.PokerPattern chuPai)
        {
            var value = new PatternValue(GetCurrentHost);
            var pokers = PokerLogicUtil.ConvertToPokerList(chuPai, value);

            var myUser = _myUser.Read();
            SetLastChuPai(SeatPosition.Bottom, pokers, myUser != null ? myUser.sex : 0);
        }

        private int GetCurrentHost()
        {
            var hostInfo = _hostInfo.Read();
            return hostInfo.GetCurrentHost();
        }

        public void SetMyLastChuPai(List<Poker> chuPai)
        {
            var myUser = _myUser.Read();
            SetLastChuPai(SeatPosition.Bottom, chuPai, myUser != null ? myUser.sex : 0);
        }

        private List<Poker> GetLastChuPai(SeatPosition pos)
        {
            switch (pos)
            {
                case SeatPosition.Bottom:
                    return _bottomeChuPai;

                case SeatPosition.Top:
                    return _topChuPai;

                case SeatPosition.Left:
                    return _leftChuPai;

                case SeatPosition.Right:
                    return _rightChuPai;

                default:
                    return null;
            }
        }

        private void SetLastChuPai(SeatPosition pos, List<Poker> lastChuPai)
        {
            switch (pos)
            {
                case SeatPosition.Bottom:
                    _bottomeChuPai = lastChuPai;
                    break;

                case SeatPosition.Top:
                    _topChuPai = lastChuPai;
                    break;

                case SeatPosition.Left:
                    _leftChuPai = lastChuPai;
                    break;

                case SeatPosition.Right:
                    _rightChuPai = lastChuPai;
                    break;
            }
        }

        private List<LastCardSlot> GetLastChuPaiSlots(SeatPosition pos)
        {
            switch (pos)
            {
                case SeatPosition.Bottom:
                    return _bottomSlots;

                case SeatPosition.Top:
                    return _topSlots;

                case SeatPosition.Left:
                    return _leftSlots;

                case SeatPosition.Right:
                    return _rightSlots;

                default:
                    return null;
            }
        }

        private Vector2 GetBasePos(SeatPosition pos)
        {
            switch (pos)
            {
                case SeatPosition.Bottom:
                    return MyLastChuPaiPos;

                case SeatPosition.Top:
                    return TopLastChuPaiPos;

                case SeatPosition.Left:
                    return LeftLastChuPaiPos;

                case SeatPosition.Right:
                    return RightLastChuPaiPos;

                default:
                    return Vector2.zero;
            }
        }

        private Image GetBuChuImage(SeatPosition pos)
        {
            switch (pos)
            {
                case SeatPosition.Top:
                    return BuChuTop;

                case SeatPosition.Bottom:
                    return BuChuBottom;

                case SeatPosition.Left:
                    return BuChuLeft;

                case SeatPosition.Right:
                    return BuChuRight;

                default:
                    return null;
            }
        }

        private Tweener GetBuChuTweener(SeatPosition pos)
        {
            if (pos == SeatPosition.Null)
            {
                return null;
            }

            Tweener tweener;
            _buChuTweeners.TryGetValue(pos, out tweener);
            return tweener;
        }

        private void SetBuChuTweener(SeatPosition pos, Tweener tweener)
        {
            if (pos == SeatPosition.Null)
            {
                return;
            }

            if (tweener != null)
            {
                if (!_buChuTweeners.ContainsKey(pos))
                {
                    _buChuTweeners.Add(pos, tweener);
                }
                else
                {
                    _buChuTweeners[pos] = tweener;
                }
            }
        }

        private Tweener StartBuChuAnimation(Transform trans)
        {
            if (!trans)
            {
                return null;
            }

            if (!trans.gameObject.activeSelf)
            {
                trans.gameObject.SetActive(true);
            }

            trans.localScale = new Vector3(0, 0, 1);
            return trans
                .DOScale(new Vector3(1, 1, 1), BuChuAnimationTime)
                .SetEase(Ease.OutBack, 1.5f);
        }

        #endregion

        #region 出牌动画配置

        public LastCardSlot CardSlotPrefab;

        public float LastChuPaiMoveTime = 0.1f;

        public float LastChuPaiMoveInterval = 0.01f;

        #endregion

        #region CardSlot

        /// <summary>
        /// 牌与牌之间的间隔。
        /// </summary>
        public float CardSlotWidth = 30;

        /// <summary>
        /// 缓存当前不用的卡槽。
        /// </summary>
        private readonly Queue<LastCardSlot> _idleSlots = new Queue<LastCardSlot>();

        /// <summary>
        /// 排序。
        /// </summary>
        /// <param name="slotList"></param>
        private void SortSlot(List<LastCardSlot> slotList)
        {
            if (slotList == null || slotList.Count <= 0)
            {
                return;
            }

            var flag = true;
            while (flag)
            {
                flag = false;
                for (int r = 1; r < slotList.Count; r++)
                {
                    var left = slotList[r - 1];
                    var right = slotList[r];
                    if (left.Poker != null && right.Poker != null)
                    {
                        int res = left.Poker.NumType - right.Poker.NumType;
                        if (res == 0)
                        {
                            if (left.Poker.SuitType > right.Poker.SuitType)
                                flag = true;
                        }
                        else if (res > 0)
                        {
                            flag = true;
                        }

                        if (flag)
                        {
                            slotList[r - 1] = right;
                            slotList[r] = left;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新牌的位置。
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="slotList"></param>
        private void UpdateSlotPosition(Vector2 pivot, List<LastCardSlot> slotList)
        {
            if (slotList == null || slotList.Count <= 0)
            {
                return;
            }

            var startX = pivot.x - (CardSlotWidth * slotList.Count) / 2;
            for (int i = 0; i < slotList.Count; i++)
            {
                var slot = slotList[i];

                if (slot.CurTweener != null)
                {
                    slot.CurTweener.Kill();
                    slot.CurTweener = null;
                }

                slot.RectTransform.pivot = new Vector2(0f, 0f);
                slot.RectTransform.anchoredPosition = pivot;
                slot.RectTransform.SetSiblingIndex(i);

                var delay = i * LastChuPaiMoveInterval;
                slot.CurTweener = slot.RectTransform
                    .DOAnchorPos(new Vector2(startX + i * CardSlotWidth, pivot.y), LastChuPaiMoveTime)
                    .SetDelay(delay);
            }
        }

        private void SetSlotIdle(LastCardSlot slot)
        {
            slot.Poker = null;
            if (slot.gameObject.activeSelf)
            {
                slot.gameObject.SetActive(false);
            }

            _idleSlots.Enqueue(slot);
        }

        private LastCardSlot GetSlot()
        {
            LastCardSlot slot;
            if (_idleSlots.Count > 0)
            {
                slot = _idleSlots.Dequeue();
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot = Instantiate(CardSlotPrefab);
            }

            if (slot)
            {
                slot.Parent = this;
                slot.RectTransform.SetParent(transform, false);
            }

            return slot;
        }

        #endregion

        #region 上次出牌Sprite

        public CardHelper CardHelper;

        public Sprite GetLastChuPaiSprite(int numType, int suitType)
        {
            return CardHelper.GetLastChuPaiCard(numType, suitType);
        }

        #endregion
    }
}