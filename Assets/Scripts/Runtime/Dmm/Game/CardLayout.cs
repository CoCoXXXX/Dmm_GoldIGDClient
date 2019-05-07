using System.Collections;
using System.Collections.Generic;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Analytic;
using Dmm.Base;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Network;
using Dmm.PokerLogic;
using Dmm.Sound;
using Dmm.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using PokerNumType = Dmm.PokerLogic.PokerNumType;
using PokerPattern = Dmm.PokerLogic.PokerPattern;
using PokerSuitType = Dmm.PokerLogic.PokerSuitType;

namespace Dmm.Game
{
    /// <summary>
    /// 牌池。
    /// 要实现根据发的牌来排列所有的牌。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardLayout : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private IAnalyticManager _analyticManager;

        private ISoundController _soundController;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<PlayingData> _playingData;

        private IDataContainer<BeenJinGong> _beenJinGong;

        private IDataContainer<BeenHuanGong> _beenHuanGong;

        private IDataContainer<ChuPaiKey> _chuPaiKey;

        private IDataContainer<HostInfoResult> _hostInfo;

        private IDataContainer<StartRound> _startRound;

        [Inject]
        public void Initialize(
            ISoundController soundController,
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            IDataRepository dataRepository,
            RemoteAPI remoteAPI)
        {
            _soundController = soundController;
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
            _remoteAPI = remoteAPI;

            _startRound = dataRepository.GetContainer<StartRound>(DataKey.StartRound);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _beenJinGong = dataRepository.GetContainer<BeenJinGong>(DataKey.BeenJinGong);
            _beenHuanGong = dataRepository.GetContainer<BeenHuanGong>(DataKey.BeenHuanGong);
            _hostInfo = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);
            _chuPaiKey = dataRepository.GetContainer<ChuPaiKey>(DataKey.ChuPaiKey);
        }

        #endregion

        #region 单张模式

        public bool DanZhangMode
        {
            get { return PrefsUtil.GetBool(PrefsKeys.XuanDanZhangKey, false); }
        }

        #endregion

        #region 配置属性

        /// <summary>
        /// 卡槽Prefab。
        /// </summary>
        public CardSlot CardSlotPrefab;

        /// <summary>
        /// 插入指示。
        /// </summary>
        public Sprite InsertIndicator;

        /// <summary>
        /// 卡槽的Parent。
        /// </summary>
        public RectTransform CardContentParent;

        /// <summary>
        /// 控制整个CardLayout的Alpha组。
        /// </summary>
        public CanvasGroup CardContentGroup;

        /// <summary>
        /// 撤销理牌按钮。
        /// </summary>
        public Button CheXiaoLiPaiBtn;

        /// <summary>
        /// 最多多少列牌。
        /// </summary>
        public int MaxColumnCount = 15;

        /// <summary>
        /// 计算牌高度的时候，参考桌子上要放多少行牌。
        /// </summary>
        public int ReferenceRowCount = 10;

        /// <summary>
        /// 牌的宽高比。
        /// </summary>
        public float CardAspect = 1.1f;

        #endregion

        #region 私有属性

        /// <summary>
        /// 牌的宽度。
        /// </summary>
        private float _cardWidth = 100;

        /// <summary>
        /// 牌的高度。
        /// </summary>
        private float _cardHeight = 110;

        /// <summary>
        /// 牌被右边的牌盖住的时候，需要至少预留的左边的宽度比例。
        /// 0 ~ 1
        /// </summary>
        public float ReservedLeftSpace = 0.3f;

        /// <summary>
        /// 牌被下面的牌盖住的时候，需要至少预留的上边的高度比例。
        /// 0 ~ 1
        /// </summary>
        public float ReservedTopSpace = 0.3f;

        /// <summary>
        /// 展开的时候，牌顶部预留的空间。
        /// 0 ~ 1
        /// </summary>
        public float ExpandTopSpace = 0.5f;

        private float CurrentTopSpace
        {
            get
            {
                if (_expand)
                {
                    return ExpandTopSpace;
                }
                else
                {
                    return ReservedTopSpace;
                }
            }
        }

        private bool _expand;

        /// <summary>
        /// 当前开始的坐标。
        /// </summary>
        private float _curStartX;

        /// <summary>
        /// 当前配置的卡槽宽度。
        /// </summary>
        private float _curSlotWidth;

        /// <summary>
        /// 当前的卡槽上部空间。
        /// </summary>
        private float _curTopHeight;

        #region 卡槽

        /// <summary>
        /// 玩家所有的牌。
        /// _slotMap与MyPokers保持一致，不管怎么操作，当前拥有的所有卡槽，以_slotMap为准。
        /// _slotLayout + _floatingSlots = _slotMap
        /// </summary>
        private readonly Dictionary<int, CardSlot> _slotMap = new Dictionary<int, CardSlot>();

        /// <summary>
        /// 当前牌池中的牌。
        /// </summary>
        private readonly List<List<CardSlot>> _slotLayout = new List<List<CardSlot>>();

        /// <summary>
        /// 当前漂浮移动的卡槽。
        /// </summary>
        private readonly List<CardSlot> _floatingSlots = new List<CardSlot>();

        private void DisposeCardSlots()
        {
            foreach (var slot in _slotMap.Values)
            {
                Destroy(slot.gameObject);
            }

            _slotMap.Clear();

            foreach (var list in _slotLayout)
            {
                if (list != null)
                    list.Clear();
            }

            _slotLayout.Clear();
            _floatingSlots.Clear();
        }

        #endregion

        #region 卡槽的缓存

        /// <summary>
        /// 缓存的空闲卡槽。
        /// </summary>
        private readonly Queue<CardSlot> _idleSlots = new Queue<CardSlot>();

        /// <summary>
        /// 获取一个卡槽对象。
        /// </summary>
        /// <returns></returns>
        private CardSlot GetCardSlot(Poker poker)
        {
            CardSlot slot;

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
                slot.Clear();

                slot.Poker = poker;
                slot.HeartHost = slot.SuitType == PokerSuitType.HEART && slot.NumType == GetCurrentHost();

                slot.transform.SetParent(CardContentParent, false);
                slot.transform.localPosition = new Vector3(0, 0, 0);
            }

            return slot;
        }

        private void SetSlotIdle(CardSlot slot)
        {
            if (!slot)
            {
                return;
            }

            slot.Clear();
            if (slot.gameObject.activeSelf)
            {
                slot.gameObject.SetActive(false);
            }

            if (!_idleSlots.Contains(slot))
            {
                _idleSlots.Enqueue(slot);
            }
        }

        private void DisposeIdleSlots()
        {
            foreach (var slot in _idleSlots)
            {
                Destroy(slot.gameObject);
            }

            _idleSlots.Clear();
        }

        #endregion

        #region 拖动占位符

        /// <summary>
        /// 占位用的空slot。
        /// </summary>
        private readonly List<CardSlot> _placeHolder = new List<CardSlot>();

        private void InitPlaceHolder()
        {
            EnablePlaceHolder(false);
        }

        private void EnablePlaceHolder(bool enable, int insertColumn = -1)
        {
            if (_placeHolder.Count <= 0)
            {
                _placeHolder.Add(GetCardSlot(null));
            }

            var slot = _placeHolder[0];
            if (slot && slot.gameObject.activeSelf != enable)
            {
                slot.CardImage.sprite = InsertIndicator;
                slot.gameObject.SetActive(enable);
            }

            if (enable)
            {
                if (insertColumn < 0)
                {
                    insertColumn = 0;
                }

                if (_slotLayout.Contains(_placeHolder) &&
                    _slotLayout.IndexOf(_placeHolder) == insertColumn)
                {
                    return;
                }

                // 将_placeHolder移动到指定的列。
                _slotLayout.Remove(_placeHolder);

                if (insertColumn <= _slotLayout.Count)
                {
                    _slotLayout.Insert(insertColumn, _placeHolder);
                }
                else
                {
                    _slotLayout.Add(_placeHolder);
                }
            }
            else
            {
                _slotLayout.Remove(_placeHolder);
            }
        }

        #endregion

        #region 销毁卡槽

        private void DisposeAllCardSlot()
        {
            DisposeCardSlots();
            DisposeIdleSlots();
        }

        #endregion

        /// <summary>
        /// 扑克的AI。
        /// </summary>
        private PokerAI _pokerAI;

        private PatternValue _value;

        private PatternMatcher _matcher;

        #endregion

        #region Unity方法

        private void OnEnable()
        {
            Init();

            _hasRefresh = false;
            _expand = false;
        }

        private void Init()
        {
            _value = new PatternValue(GetCurrentHost);
            _matcher = new PatternMatcher(_value);
            // 在Enable的时候初始化。
            _pokerAI = new PokerAI(_matcher, _value);
            InitPlaceHolder();
        }

        private void OnDisable()
        {
            DisposeAllCardSlot();
        }

        private void Update()
        {
            CheckLongPressState();
            CheckHotKey();
        }

        #endregion

        #region RefreshTotal

        private float _totalRefreshTime;

        public void RefreshTotal()
        {
            if (_totalRefreshTime >= _playingData.Timestamp)
            {
                return;
            }

            _totalRefreshTime = _playingData.Timestamp;
            // 这里不对牌池进行排序。
            // 是因为如果玩家断线重连了，
            // 不应当将玩家理的牌刷新掉。
            RefreshMyPokers(!_hasRefresh);
        }

        private float _jinHuanGongPokerRefreshTime;

        public void RefreshJinHuanGongPoker()
        {
            var max = Mathf.Max(_beenJinGong.Timestamp, _beenHuanGong.Timestamp);
            if (_jinHuanGongPokerRefreshTime >= max)
            {
                return;
            }

            _jinHuanGongPokerRefreshTime = max;

            var beenJinGong = _beenJinGong.Read(true);
            if (beenJinGong != null)
            {
                if (_slotMap.ContainsKey(beenJinGong.poker_id))
                {
                    var slot = _slotMap[beenJinGong.poker_id];
                    if (slot)
                    {
                        slot.JinHuanGong = true;
                    }
                }
            }

            var beenHuanGong = _beenHuanGong.Read(true);
            if (beenHuanGong != null)
            {
                if (_slotMap.ContainsKey(beenHuanGong.poker_id))
                {
                    var slot = _slotMap[beenHuanGong.poker_id];
                    if (slot)
                    {
                        slot.JinHuanGong = true;
                    }
                }
            }
        }

        #endregion

        #region 刷新内容

        /// <summary>
        /// 卡槽位置发生变化的时候，移动动画的执行时间。
        /// </summary>
        public float AnimationTime = 0.5f;

        /// <summary>
        /// 之前是否刷新过内容。
        /// 给RefreshTotal一个是否排序做参考。
        /// </summary>
        private bool _hasRefresh;

        private void RefreshMyPokers(bool sortCardLayout)
        {
            _hasRefresh = true;

            var playingData = _playingData.Read();

            var myPokers = playingData == null ? null : playingData.my_pokers;

            if (myPokers == null || myPokers.Length <= 0)
            {
                ClearAllSlots();
                return;
            }

            _pokerAI.SetMyPokers(myPokers);
            _pokerAI.BuildPokerPool();

            if (_superABCDE != null)
            {
                _superABCDE.Clear();
            }

            _superABCDE = _pokerAI.FindAllSuperABCDE();
            _currentSelectedSuperABCDE = -1;

            CheckAndClearEmptySlot();
            UpdateSlotContent();

            if (sortCardLayout)
            {
                SortCardLayout();
            }

            UpdateSlotDimention();
        }

        private void CheckAndClearEmptySlot()
        {
            // 扫描浮动的牌组。
            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                var slot = _floatingSlots[i];
                if (ContainsPoker(slot.PokerNumber))
                {
                    continue;
                }

                _slotMap.Remove(slot.PokerNumber);
                SetSlotIdle(slot);

                _floatingSlots.RemoveAt(i);
                i--;
            }

            // 扫描slotList
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];

                // 不能删除placeHolder。
                if (col == _placeHolder)
                {
                    continue;
                }

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    if (ContainsPoker(slot.PokerNumber))
                    {
                        continue;
                    }

                    _slotMap.Remove(slot.PokerNumber);
                    SetSlotIdle(slot);

                    col.RemoveAt(r);
                    r--;
                }

                if (col.Count <= 0)
                {
                    _slotLayout.RemoveAt(c);
                    c--;
                }
            }

            // 扫描slotMap
            var removeFromMap = new List<int>();
            foreach (var number in _slotMap.Keys)
            {
                if (!ContainsPoker(number))
                {
                    removeFromMap.Add(number);
                }
            }

            for (int i = 0; i < removeFromMap.Count; i++)
            {
                var number = removeFromMap[i];
                if (_slotMap.ContainsKey(number))
                {
                    var slot = _slotMap[number];
                    _slotMap.Remove(number);
                    SetSlotIdle(slot);
                }
            }

            removeFromMap.Clear();
        }

        /// <summary>
        // 扫描我所有的牌，如果之前的牌存在，则保留卡槽位置。
        // 如果之前的牌不存在，则添加新牌。
        /// </summary>
        private void UpdateSlotContent()
        {
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (myPokers == null || myPokers.Length <= 0)
            {
                return;
            }

            var currentHost = GetCurrentHost();
            for (int p = 0; p < myPokers.Length; p++)
            {
                var number = myPokers[p];

                // 是空的牌，就不用操作了。
                if (number == Poker.NullPoker)
                {
                    continue;
                }

                // 牌已经存在了，就不进行任何操作。
                if (_slotMap.ContainsKey(number))
                {
                    var slot = _slotMap[number];
                    if (slot)
                    {
                        slot.HeartHost = slot.SuitType == PokerSuitType.HEART && slot.NumType == currentHost;
                    }

                    continue;
                }

                // 将牌添加到卡槽中。
                var poker = new Poker(number);
                var added = AddPokerToLayout(poker);

                // 没有发现NumType相等的牌列，在最后添加一个牌列。
                if (!added)
                {
                    var col = new List<CardSlot>();

                    var slot = GetCardSlot(poker);

                    _slotMap[number] = slot;

                    col.Add(slot);

                    var inserted = false;
                    for (int i = 0; i < _slotLayout.Count; i++)
                    {
                        var c = _slotLayout[i];
                        if (c == null || c.Count <= 0)
                        {
                            // 当前列没有卡槽，应当删除当前列。
                            _slotLayout.RemoveAt(i);
                            i--;
                            continue;
                        }

                        var res = _value.Compare(poker, c[0].Poker);
                        if (res == CompareResult.BIGGER)
                        {
                            _slotLayout.Insert(i, col);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        _slotLayout.Add(col);
                    }
                }
            }
        }

        private CardSlot AddPokerToLayout(Poker poker)
        {
            // 将牌添加到卡槽中。
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];

                if (col == null || col.Count <= 0)
                {
                    // 当前列没有卡槽，应当删除当前列。
                    _slotLayout.RemoveAt(c);
                    c--;
                    continue;
                }

                // 如果发现NumType相等的牌列，则将牌添加到该列的最顶端。
                if (col[0].NumType == poker.NumType)
                {
                    var slot = GetCardSlot(poker);

                    col.Add(slot);
                    _slotMap[poker.Number] = slot;

                    return slot;
                }
            }

            return null;
        }

        private void SortCardLayout()
        {
            if (_slotLayout.Count <= 0)
            {
                return;
            }

            var flag = true;
            while (flag)
            {
                flag = false;
                for (int c = 1; c < _slotLayout.Count; c++)
                {
                    var left = _slotLayout[c - 1];
                    var right = _slotLayout[c];
                    if (left.Count > 0 && right.Count > 0)
                    {
                        int res = _value.Compare(left[0].Poker, right[0].Poker);
                        if (res == CompareResult.SMALLER)
                        {
                            _slotLayout[c - 1] = right;
                            _slotLayout[c] = left;
                            flag = true;
                        }
                    }
                }
            }

            for (int c = 0; c < _slotLayout.Count; c++)
            {
                SortCardColumn(_slotLayout[c]);
            }
        }

        private void SortCardColumn(List<CardSlot> column)
        {
            if (column == null || column.Count <= 0)
            {
                return;
            }

            var flag = true;
            while (flag)
            {
                flag = false;
                for (int r = 1; r < column.Count; r++)
                {
                    var left = column[r - 1];
                    var right = column[r];
                    if (left.Poker != null && right.Poker != null)
                    {
                        int res = left.Poker.NumType - right.Poker.NumType;
                        if (res == 0)
                        {
                            if (left.Poker.SuitType > right.Poker.SuitType)
                            {
                                flag = true;
                            }
                        }
                        else if (res > 0)
                        {
                            flag = true;
                        }

                        if (flag)
                        {
                            column[r - 1] = right;
                            column[r] = left;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新卡槽的大小和位置。
        /// </summary>
        private void UpdateSlotDimention()
        {
            if (_slotLayout.Count <= 0)
            {
                return;
            }

            // 刷新每个卡槽的位置和宽高。
            var height = CardContentParent.rect.height;
            var width = CardContentParent.rect.width;

            _cardHeight = height / (ReservedTopSpace * (ReferenceRowCount - 1) + 1);
            _cardWidth = _cardHeight / CardAspect;

            if (_slotLayout.Count > 1)
            {
                _curSlotWidth = (width - _cardWidth) / (_slotLayout.Count - 1);

                _curStartX = 0;
                // 卡槽宽度超过牌宽度的时候，使用牌宽度，保持牌与牌之间紧贴在一起。
                if (_curSlotWidth > _cardWidth)
                {
                    _curSlotWidth = _cardWidth;
                    _curStartX = (width - _cardWidth * _slotLayout.Count) / 2;
                }
                else
                {
                    _curStartX = 0;
                }
            }
            else
            {
                _curSlotWidth = _cardWidth;
                _curStartX = width / 2 - _cardWidth / 2;
            }

            int siblingIndex = 0;
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];

                for (int r = col.Count - 1; r >= 0; r--)
                {
                    var slot = col[r];

                    _curTopHeight = _cardHeight * CurrentTopSpace;
                    MoveSlot(slot, _curStartX + _curSlotWidth * c, _curTopHeight * r, siblingIndex, true);
                    siblingIndex++;

                    ResizeSlot(slot, _cardWidth, _cardHeight);
                }
            }
        }

        private void ResizeSlot(CardSlot slot, float width, float height)
        {
            if (!slot)
            {
                return;
            }

            slot.RectTransform.sizeDelta = new Vector2(width, height);
            slot.RectTransform.localScale = new Vector3(1, 1, 1);
        }

        private void MoveSlot(CardSlot slot, float x, float y, int siblingIndex, bool withAnim)
        {
            if (!slot)
            {
                return;
            }

            if (slot.CurTweener != null)
            {
                slot.CurTweener.Kill();
                slot.CurTweener = null;
            }

            slot.RectTransform.SetSiblingIndex(siblingIndex);

            if (slot.RectTransform.anchoredPosition == new Vector2(x, y))
            {
                return;
            }

            if (withAnim)
            {
                slot.CurTweener = slot.RectTransform.DOAnchorPos(new Vector2(x, y), AnimationTime);
            }
            else
            {
                slot.RectTransform.anchoredPosition = new Vector2(x, y);
            }
        }

        /// <summary>
        /// 清空所有的卡槽。
        /// 将闲置的卡槽和牌列都缓存起来。
        /// </summary>
        public void ClearAllSlots()
        {
            // 清空浮动的牌。
            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                var slot = _floatingSlots[i];
                SetSlotIdle(slot);
            }

            _floatingSlots.Clear();

            // 关闭_placeHolder。
            EnablePlaceHolder(false);

            // 清空当前牌池中的牌。
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];
                if (col != null && col.Count > 0)
                {
                    for (int r = 0; r < col.Count; r++)
                    {
                        var slot = col[r];
                        SetSlotIdle(slot);
                    }

                    col.Clear();
                }
            }

            _slotLayout.Clear();

            // slotList + floatingSlots = slotMap
            _slotMap.Clear();
        }

        /// <summary>
        /// 取消牌池中所有牌的选择状态。
        /// </summary>
        public void ResetAllSelectState(bool updateDimention)
        {
            // 清空牌池中选中的牌。
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];
                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    slot.Selected = false;
                }
            }

            // 清空浮动牌中选中的牌。
            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                _floatingSlots[i].Selected = false;
            }

            if (_expand)
            {
                _expand = false;
                if (updateDimention)
                {
                    UpdateSlotDimention();
                }
            }
        }

        private void UpdateFloatingSlots(Vector2 touchPos)
        {
            if (_floatingSlots.Count <= 0)
            {
                return;
            }

            var topHeight = _cardHeight * ReservedTopSpace;
            var pos = ToLocalPosition(touchPos);
            int siblingIndex = 500;
            for (int i = _floatingSlots.Count - 1; i >= 0; i--)
            {
                var slot = _floatingSlots[i];

                MoveSlot(
                    slot,
                    pos.x - _cardWidth / 2,
                    _cardHeight + topHeight * i,
                    siblingIndex,
                    false);

                siblingIndex++;
            }

            var insertColumn = (int) ((pos.x - _curStartX) / _curSlotWidth);
            EnablePlaceHolder(true, insertColumn);
        }

        private Vector2 ToLocalPosition(Vector2 touchPos)
        {
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CardContentParent, touchPos, MainCamera.Current, out localPos))
            {
                return localPos;
            }

            return touchPos;
        }

        #endregion

        #region 撤销理牌

        /// <summary>
        /// 重置到之前的状态。
        /// </summary>
        public void RestoreLayoutRecord()
        {
            _analyticManager.Event("game_chexiaolipai");
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (myPokers == null || myPokers.Length <= 0)
            {
                ClearAllSlots();
                return;
            }

            // 缓存当前牌池中的牌，以便之后产生牌从当前位置移动到目标位置的效果。
            var cache = new Dictionary<int, CardSlot>();

            // 扫描浮动牌。
            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                var slot = _floatingSlots[i];
                if (!slot)
                {
                    continue;
                }

                if (ContainsPoker(slot.PokerNumber))
                {
                    cache[slot.PokerNumber] = slot;
                }
                else
                {
                    SetSlotIdle(slot);
                    _slotMap.Remove(slot.PokerNumber);
                }
            }

            _floatingSlots.Clear();

            EnablePlaceHolder(false);

            // 扫描_slotList。
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];
                if (col != null && col.Count > 0)
                {
                    for (int r = 0; r < col.Count; r++)
                    {
                        var slot = col[r];
                        if (!slot)
                        {
                            continue;
                        }

                        if (ContainsPoker(slot.PokerNumber))
                        {
                            cache[slot.PokerNumber] = slot;
                        }
                        else
                        {
                            SetSlotIdle(slot);
                            _slotMap.Remove(slot.PokerNumber);
                        }
                    }

                    col.Clear();
                }
            }

            _slotLayout.Clear();

            for (int i = 0; i < myPokers.Length; i++)
            {
                var number = myPokers[i];

                if (number == Poker.NullPoker)
                {
                    continue;
                }

                if (!cache.ContainsKey(number))
                {
                    var slot = GetCardSlot(new Poker(number));
                    cache[number] = slot;
                }
            }

            // 扫描一下_slotMap确保没有多出的牌漏掉。
            foreach (var slot in _slotMap.Values)
            {
                if (!cache.ContainsValue(slot))
                {
                    SetSlotIdle(slot);
                }
            }

            _slotMap.Clear();

            // 将cache中的牌添加到_slotList和_slotMap中。
            foreach (var key in cache.Keys)
            {
                var slot = cache[key];
                var added = false;
                for (int c = 0; c < _slotLayout.Count; c++)
                {
                    var col = _slotLayout[c];

                    if (col == null || col.Count <= 0)
                    {
                        // 当前列没有卡槽，应当删除当前列。
                        _slotLayout.RemoveAt(c);
                        c--;
                        continue;
                    }

                    // 如果发现NumType相等的牌列，则将牌添加到该列的最顶端。
                    if (col[0].NumType == slot.NumType)
                    {
                        col.Add(slot);
                        _slotMap[slot.PokerNumber] = slot;

                        added = true;
                        break;
                    }
                }

                // 没有发现NumType相等的牌列，在最后添加一个牌列。
                if (!added)
                {
                    var col = new List<CardSlot>();

                    _slotMap[slot.PokerNumber] = slot;
                    col.Add(slot);

                    _slotLayout.Add(col);
                }
            }

            cache.Clear();

            ResetAllSelectState(false);
            SortCardLayout();
            UpdateSlotDimention();
        }

        #endregion

        #region 理成一列

        public void LiChengYiLie()
        {
            // 扫描已经选中的牌，将牌按照匹配的牌型放置到牌的左边或者右边。
            var selected = new List<CardSlot>();

            // 删除牌池中已经选中用的牌。
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    if (slot.Selected)
                    {
                        selected.Add(slot);

                        col.RemoveAt(r);
                        r--;
                    }
                }

                if (col.Count <= 0)
                {
                    _slotLayout.RemoveAt(c);
                    c--;
                }
            }

            // 删除浮动牌中选中的部分，保持其他部分不变。
            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                var slot = _floatingSlots[i];
                if (slot.Selected && !selected.Contains(slot))
                {
                    selected.Add(slot);

                    _floatingSlots.RemoveAt(i);
                    i--;
                }
            }

            // 更新浮动牌池。
            EnablePlaceHolder(_floatingSlots.Count > 0, _slotLayout.IndexOf(_placeHolder));

            // 牌池和浮动牌中间都没有选中的牌。
            if (selected.Count <= 0)
            {
                return;
            }

            // 先排个序。
            SortCardColumn(selected);

            // 匹配选中的牌型。
            var pokers = new List<Poker>();
            for (int i = 0; i < selected.Count; i++)
            {
                pokers.Add(selected[i].Poker);
            }

            var matched = _matcher.Match(pokers);
            if (matched != null && PatternType.IsBomb(matched.Type))
            {
                // 是炸弹，将牌放到左边。
                _slotLayout.Insert(0, selected);
            }
            else
            {
                // 非炸弹，放到右边。
                _slotLayout.Add(selected);
            }

            ResetAllSelectState(false);
            UpdateFloatingSlots(_lastHoldPos);
            UpdateSlotDimention();

            _analyticManager.Event("game_lichengyilie");
        }

        #endregion

        #region 看同花顺

        private List<PokerPattern> _superABCDE;

        private int _currentSelectedSuperABCDE = -1;

        public void KanTongHuaShun()
        {
            ResetAllSelectState(false);

            if (_superABCDE != null && _superABCDE.Count > 0)
            {
                _currentSelectedSuperABCDE = (_currentSelectedSuperABCDE + 1) % _superABCDE.Count;
                var selected = _superABCDE[_currentSelectedSuperABCDE];
                foreach (var p in selected.Pokers)
                {
                    if (_slotMap.ContainsKey(p.Number))
                    {
                        var slot = _slotMap[p.Number];
                        slot.Selected = true;
                    }
                }
            }
            else
            {
                Toast("没有同花顺");
            }

            _analyticManager.Event("game_kantonghuashun");
        }

        #endregion

        #region 触摸监听

        #region 长按检测

        /// <summary>
        /// 长按的时间。
        /// </summary>
        public float LongPressTime = 2;

        public float StableRange = 10;

        private float _pointerHoldTime;

        private bool _longPressing;

        private Vector2 _lastHoldPos;

        private void CheckLongPressState()
        {
            if (!_longPressing)
            {
                return;
            }

            _pointerHoldTime += Time.deltaTime;

            if (_pointerHoldTime >= LongPressTime)
            {
                _longPressing = false;
                _pointerHoldTime = 0;

                OnLongPress();
            }
        }

        #endregion

        private CardSlot _lastHittedSlot;

        /// <summary>
        /// 通过是否为-1确定是否当前已经有点触摸了。
        /// </summary>
        private int _lastPointerId = -1;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (_lastPointerId != -1)
            {
                return;
            }

            _lastPointerId = eventData.pointerId;

            _longPressing = true;
            _pointerHoldTime = 0;

            _lastHoldPos = eventData.position;

            _lastHittedSlot = null;

            _expand = true;
            UpdateSlotDimention();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (eventData.pointerId != _lastPointerId)
            {
                return;
            }

            _lastPointerId = -1;

            _lastHoldPos = eventData.position;
            _longPressing = false;
            _pointerHoldTime = 0;

            if (_floatingSlots.Count > 0)
            {
                var list = new List<CardSlot>();
                list.AddRange(_floatingSlots);
                _floatingSlots.Clear();

                var insertColumn = _slotLayout.IndexOf(_placeHolder);
                if (insertColumn < 0)
                {
                    insertColumn = 0;
                }

                if (insertColumn <= _slotLayout.Count)
                {
                    _slotLayout.Insert(insertColumn, list);
                }
                else
                {
                    _slotLayout.Add(list);
                }

                EnablePlaceHolder(false);

                ResetAllSelectState(false);
                UpdateSlotDimention();
                _lastHittedSlot = null;
                return;
            }

            var hitted = HitCardSlot(eventData);
            if (!hitted)
            {
                _lastHittedSlot = null;
                return;
            }

            if (!_lastHittedSlot)
            {
                if (DanZhangMode)
                {
                    // 选单张模式。
                    hitted.Selected = !hitted.Selected;
                }
                else
                {
                    // 选整列模式。
                    var col = GetColumnOfSlot(hitted);
                    if (col != null)
                    {
                        if (col.Count <= 1)
                        {
                            // 本列只有一张的情况：
                            hitted.Selected = !hitted.Selected;
                        }
                        else
                        {
                            // 本列多于1张的情况：
                            if (SameSelectStateCount(col, false) == col.Count)
                            {
                                // 都没有选中的情况下，直接选中全部。
                                for (int i = 0; i < col.Count; i++)
                                {
                                    col[i].Selected = true;
                                }
                            }
                            else if (SameSelectStateCount(col, true) == col.Count)
                            {
                                // 全部都选中的情况下，只选中当前的卡槽，清空其他的卡槽。
                                for (int i = 0; i < col.Count; i++)
                                {
                                    if (col[i] != hitted)
                                    {
                                        col[i].Selected = false;
                                    }
                                }

                                hitted.Selected = true;
                            }
                            else
                            {
                                // 其他情况，就保持正常的选择模式。
                                hitted.Selected = !hitted.Selected;
                            }
                        }
                    }
                }
            }

            _lastHittedSlot = null;
        }

        private int SameSelectStateCount(List<CardSlot> list, bool selected)
        {
            if (list == null)
            {
                return 0;
            }

            var count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Selected == selected)
                {
                    count++;
                }
            }

            return count;
        }

        private List<CardSlot> GetColumnOfSlot(CardSlot slot)
        {
            if (!slot)
            {
                return null;
            }

            for (int i = 0; i < _slotLayout.Count; i++)
            {
                var col = _slotLayout[i];
                if (col.Contains(slot))
                {
                    return col;
                }
            }

            return null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (eventData.pointerId != _lastPointerId)
            {
                return;
            }

            if (_floatingSlots.Count > 0)
            {
                // 浮动卡槽存在的时候，拖动操作，只能移动浮动的牌组。
                UpdateFloatingSlots(eventData.position);
                UpdateSlotDimention();
                return;
            }

            if (_longPressing &&
                Vector2.Distance(_lastHoldPos, eventData.position) >= StableRange)
            {
                _lastHoldPos = eventData.position;
                _pointerHoldTime = 0;
            }

            var hitted = HitCardSlot(eventData);
            if (!hitted)
            {
                return;
            }

            if (!_lastHittedSlot || hitted != _lastHittedSlot)
            {
                _lastHittedSlot = hitted;
                OnSlotEnter(hitted);
            }
        }

        #region 自定义触摸事件

        private void OnSlotEnter(CardSlot slot)
        {
            slot.Selected = !slot.Selected;
        }

        private void OnLongPress()
        {
            // 如果出现_floatingSlots有牌的情况，就应该检查一遍。
            // 虽然我不知道什么情况下才会出现这种情况，不过还是检查一遍，保证牌池的正确性。
            if (_floatingSlots.Count > 0)
            {
                for (int i = 0; i < _floatingSlots.Count; i++)
                {
                    SetSlotIdle(_floatingSlots[i]);
                }

                _floatingSlots.Clear();

                CheckAndClearEmptySlot();
                UpdateSlotContent();
            }

            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    if (slot.Selected && !_floatingSlots.Contains(slot))
                    {
                        _floatingSlots.Add(slot);

                        col.RemoveAt(r);
                        r--;
                    }
                }

                if (col.Count <= 0)
                {
                    _slotLayout.RemoveAt(c);
                    c--;
                }
            }

            SortCardColumn(_floatingSlots);
            UpdateFloatingSlots(_lastHoldPos);

            ResetAllSelectState(false);
            UpdateSlotDimention();
        }

        #endregion

        private CardSlot HitCardSlot(PointerEventData data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.pointerEnter)
            {
                return data.pointerEnter.GetComponent<CardSlot>();
            }

            return null;
        }

        #endregion

        #region 发牌

        public void RandomFaPai()
        {
            /*var list = new List<int>();
            int i = 0;
            while (i < 27)
            {
                int num = Random.Range(1, 108);
                if (!list.Contains(num))
                {
                    i++;
                    list.Add(num);
                }
            }

            TableData.Current.MyPokers.Clear();
            TableData.Current.MyPokers.AddRange(list);

            TableData.Current.FaPaiTime = Time.time;*/
        }

        private float _lastFaPaiTime;

        public void RefreshFaPai()
        {
            var max = Mathf.Max(_startRound.Timestamp, _playingData.Timestamp);
            if (_lastFaPaiTime >= max)
            {
                return;
            }

            _lastFaPaiTime = max;

            // 只有有牌的时候才播放发牌声。
            if (GetMyPokerCount() > 0)
            {
                _soundController.PlayFaPaiSound();
            }

            RefreshMyPokers(true);
        }

        #endregion

        #region 显示桌面

        public float Alpha
        {
            get
            {
                if (CardContentGroup)
                {
                    return CardContentGroup.alpha;
                }

                return 1;
            }
            set
            {
                if (CardContentGroup)
                {
                    CardContentGroup.alpha = value;
                }
            }
        }

        #endregion

        #region 提示

        /// <summary>
        /// 上一次刷新出牌记录的时间。
        /// </summary>
        private float _tiShiRefreshTime;

        /// <summary>
        /// 出牌记录。
        /// </summary>
        private PokerPattern _lastTiShiPattern;

        /// <summary>
        /// 刷新提示的记录。
        /// 因为玩家在点击提示的时候，会跟上一次提示的牌进行对比。
        /// 所以当收到新的出牌请求的时候，需要将之前的提示记录清空。
        /// </summary>
        public void RefreshTiShiPattern()
        {
            if (_tiShiRefreshTime >= _chuPaiKey.Timestamp)
            {
                return;
            }

            _tiShiRefreshTime = _chuPaiKey.Timestamp;

            var playingData = _playingData.Read();
            var currentChuPaiSeat = playingData == null ? -1 : playingData.chupai_key_owner_seat;
            var tableUser = _tableUser.Read();
            if (currentChuPaiSeat != tableUser.MySeat)
            {
                return;
            }

            // 只有收到出牌请求的人是自己的时候，才清空提示的记录。
            _lastTiShiPattern = null;

            var otherChuPai = LastMatchedValueChuPai();

            if (otherChuPai != null)
            {
                var myPokers = playingData == null ? null : playingData.my_pokers;
                _pokerAI.SetMyPokers(myPokers);
                _pokerAI.BuildPokerPool();
                var tishi = _pokerAI.GetSmallestPatternBiggerThan(
                    otherChuPai,
                    true,
                    true);

                if (tishi == null || tishi.IsNull)
                {
                    Toast("没有牌能大过上家", true);
                }
            }
        }

        /// <summary>
        /// 提示。
        /// </summary>
        public void TiShi()
        {
            var lastChuPai = LastMatchedValueChuPai();
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (lastChuPai == null || lastChuPai.IsNull)
            {
                // 之前不存在出牌了，则随机选择一个牌型来出。
                // MyLog.InfoWithFrame(name, "SelectChuPai");
                _pokerAI.SetMyPokers(myPokers);
                _lastTiShiPattern = _pokerAI.SelectChuPai();
            }
            else
            {
                // 存在出牌，则选择一个比较大的牌来出。
                _pokerAI.SetMyPokers(myPokers);

                if (_lastTiShiPattern == null || _lastTiShiPattern.IsNull)
                {
                    // 不存在之前的提示。
                    // MyLog.InfoWithFrame(name, string.Format("SelectChuPai from lastChuPai {0}", lastChuPai));
                    _lastTiShiPattern = _pokerAI.GetSmallestPatternBiggerThan(
                        lastChuPai,
                        true,
                        true,
                        10);
                }
                else
                {
                    // 存在上一次提示。
                    MyLog.InfoWithFrame(name, string.Format("SelectChuPai from lastTiShi {0}", _lastTiShiPattern));
                    _lastTiShiPattern = _pokerAI.GetSmallestPatternBiggerThan(
                        _lastTiShiPattern,
                        true,
                        true,
                        10);

                    // 如果上一次的提示已经存在，则很有可能是上次选择了最大的牌。
                    // 因此重新用原始的数据选择一遍。
                    if (_lastTiShiPattern == null || _lastTiShiPattern.IsNull)
                    {
                        _lastTiShiPattern = _pokerAI.GetSmallestPatternBiggerThan(
                            lastChuPai,
                            true,
                            true,
                            10);
                    }
                }
            }

            if (_lastTiShiPattern != null && !_lastTiShiPattern.IsNull)
            {
                ResetAllSelectState(false);

                // 选中所有提示的牌。
                var pokers = _lastTiShiPattern.Pokers;
                if (pokers != null && pokers.Count > 0)
                {
                    for (int i = 0; i < pokers.Count; i++)
                    {
                        var p = pokers[i];
                        if (_slotMap.ContainsKey(p.Number))
                        {
                            var slot = _slotMap[p.Number];
                            if (slot)
                            {
                                slot.Selected = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (lastChuPai != null && !lastChuPai.IsNull)
                {
                    // 没有大过的牌，就直接不出了。
                    ResetAllSelectState(false);
                    // Toast("没有牌能大过上家", true);
                    BuChu();
                }
            }
        }

        #endregion

        #region 出牌

        public ChuPaiButtonGroup ChuPaiBtnGroup;

        public LastChuPaiGroup LastChuPaiGroup;

        public void BuChu()
        {
            var pattern = new com.morln.game.gd.command.PokerPattern();
            pattern.type = PatternType.BUCHU;
            _remoteAPI.ChuPai(pattern);

            if (ChuPaiBtnGroup)
            {
                ChuPaiBtnGroup.ShowChuPaiGroup(false);
            }
        }

        /// <summary>
        /// 将当前选中的牌，出出去。
        /// </summary>
        public void ChuPai()
        {
            // 挑出当前选中的牌。
            var selected = new List<Poker>();

            var chuPaiList = GetSelectedPokerList();

            if (chuPaiList != null && chuPaiList.Count > 0)
            {
                selected.AddRange(chuPaiList);
            }

            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                var slot = _floatingSlots[i];
                if (!slot.Selected)
                {
                    continue;
                }

                var poker = slot.Poker;
                if (poker != null && !selected.Contains(poker))
                {
                    selected.Add(poker);
                }
            }

            if (selected.Count <= 0)
            {
                Toast("请选择要出的牌！", true);
                return;
            }

            // 检查当前选中的牌，是否符合规则。
            var matched = _matcher.Match(selected);
            if (matched == null || matched.IsNull)
            {
                // 提示玩家选中的牌不符合牌型。
                Toast("出牌不符合规则！", true);
                return;
            }

            // 和上一次出牌进行比较。
            var lastChuPai = LastMatchedValueChuPai();
            if (lastChuPai != null)
            {
                var res = _value.Compare(matched, lastChuPai);
                switch (res)
                {
                    case CompareResult.PATTERN_NOT_MATCH:
                        Toast("出牌不符合规则！", true);
                        return;

                    case CompareResult.SMALLER:
                    case CompareResult.EQUAL:
                        Toast("出牌不够大！", true);
                        return;
                }
            }

            // 符合牌型，出牌。
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    if (slot.Selected)
                    {
                        _slotMap.Remove(slot.PokerNumber);

                        SetSlotIdle(slot);
                        col.RemoveAt(r);

                        r--;
                    }
                }

                if (col.Count <= 0)
                {
                    _slotLayout.RemoveAt(c);
                    c--;
                }
            }

            for (int i = 0; i < _floatingSlots.Count; i++)
            {
                var slot = _floatingSlots[i];
                if (slot.Selected)
                {
                    SetSlotIdle(slot);

                    _floatingSlots.RemoveAt(i);
                    i--;
                }
            }

            // 出完牌之后，将浮动的牌落下。
            if (_floatingSlots.Count > 0)
            {
                var list = new List<CardSlot>();
                list.AddRange(_floatingSlots);

                var insertColumn = _slotLayout.IndexOf(_placeHolder);
                if (insertColumn < 0)
                {
                    insertColumn = 0;
                }

                if (insertColumn <= _slotLayout.Count)
                {
                    _slotLayout.Insert(insertColumn, list);
                }
                else
                {
                    _slotLayout.Add(list);
                }

                _floatingSlots.Clear();
            }

            EnablePlaceHolder(false);

            // 删除要出的牌。
            RemoveCardLayoutPoker(selected);
            ResetAllSelectState(true);
            UpdateSlotDimention();

            // 不再直接显示出的牌。
            // LastChuPaiGroup.SetMyLastChuPai(matched);

            // 出牌的时候清空提示的牌。
            _lastTiShiPattern = null;

            // 发出出牌的命令。
            _remoteAPI.ChuPai(PokerLogicUtil.ConvertToSessionPattern(matched));

            if (ChuPaiBtnGroup)
            {
                ChuPaiBtnGroup.ShowChuPaiGroup(false);
            }
        }

        /// <summary>
        /// 获取当前选中的牌。
        /// </summary>
        /// <returns></returns>
        private List<Poker> GetSelectedPokerList()
        {
            var selected = new List<Poker>();
            for (int c = 0; c < _slotLayout.Count; c++)
            {
                var col = _slotLayout[c];
                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    if (slot.Selected && slot.Poker != null)
                    {
                        selected.Add(slot.Poker);
                    }
                }
            }

            return selected;
        }

        public JinGongButtonGroup JinGongButtonGroup;

        /// <summary>
        /// 选中当前可以进贡的牌。
        /// </summary>
        public void SelectJinGongPoker()
        {
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            var bytes = myPokers;
            if (bytes == null || bytes.Length <= 0)
            {
                return;
            }

            ResetAllSelectState(false);

            var found = AutoSelectJinGong(GetMyPokerList());

            if (found != null)
            {
                if (_slotMap.ContainsKey(found.Number))
                {
                    _slotMap[found.Number].Selected = true;
                }
            }
        }

        private Poker AutoSelectJinGong(List<Poker> pokers)
        {
            if (pokers == null || pokers.Count <= 0)
            {
                return null;
            }

            Poker found = null;
            pokers.Sort(_value.Compare);
            for (int i = pokers.Count - 1; i >= 0; i--)
            {
                var p = pokers[i];
                if (!_value.IsHeartHost(p))
                {
                    found = p;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// 进贡。
        /// </summary>
        public void JinGong()
        {
            var selected = GetSelectedPokerList();
            if (selected == null || selected.Count <= 0)
            {
                Toast("请选择进贡的牌");
                SelectJinGongPoker();
                return;
            }

            // 不能进贡大于一张的牌。
            if (selected.Count > 1)
            {
                Toast("只能进贡 1 张牌");
                SelectJinGongPoker();
                return;
            }

            var sel = selected[0];

            // 不能够进贡红心主牌。
            if (_value.IsHeartHost(sel))
            {
                Toast("不能进贡逢人配");
                SelectJinGongPoker();
                return;
            }

            // 验证进贡牌的合法性。
            var poker = AutoSelectJinGong(GetMyPokerList());
            if (poker != null && sel.NumType != poker.NumType)
            {
                Toast("请选择正确的进贡牌");
                SelectJinGongPoker();
                return;
            }

            // 发出进贡命令。
            _remoteAPI.JinGong(sel.Number);

            // 隐藏进贡按钮。
            if (JinGongButtonGroup)
            {
                JinGongButtonGroup.ShowJinGongBtn(false);
            }
        }

        /// <summary>
        /// 还贡。
        /// </summary>
        public void HuanGong()
        {
            var selected = GetSelectedPokerList();
            if (selected == null || selected.Count <= 0)
            {
                Toast("请选择还贡的牌");
                return;
            }

            if (selected.Count > 1)
            {
                Toast("只能还贡 1 张牌");
                return;
            }

            var poker = selected[0];
            /*
            if (poker.NumType > PokerNumType.P10)
            {
                Toast("不能还10以上的牌");
                return;
            }
            */

            if (poker.NumType == PokerNumType.PX || poker.NumType == PokerNumType.PD)
            {
                Toast("不能还大小王");
                return;
            }

            if (poker.NumType == GetCurrentHost())
            {
                Toast("不能还主牌");
                return;
            }

            // 发送还贡命令。
            _remoteAPI.HuanGong(selected[0].Number);

            // 隐藏还贡按钮。
            if (JinGongButtonGroup)
            {
                JinGongButtonGroup.ShowHuanGongBtn(false);
            }
        }

        public void ResetCardLayout()
        {
            ResetAllSelectState(true);
        }

        #endregion

        #region 快捷方法

        private void Toast(string content, bool error = false)
        {
            _dialogManager.ShowToast(content, 1, error);
        }

        #endregion

        #region 牌的Sprite

        public CardHelper CardHelper;

        public Sprite GetCardSprite(int numType, int suitType)
        {
            return CardHelper.GetCard(numType, suitType);
        }

        #endregion

        #region 键盘快捷键

        private void CheckHotKey()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TiShi();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChuPai();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                LiChengYiLie();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                RestoreLayoutRecord();
            }
        }

        #endregion

        #region 工具方法

        private int GetCurrentHost()
        {
            var host = _hostInfo.Read();
            return host.GetCurrentHost();
        }

        private bool ContainsPoker(int poker)
        {
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (myPokers == null || myPokers.Length <= 0)
            {
                return false;
            }

            for (int i = 0; i < myPokers.Length; i++)
            {
                if (myPokers[i] != Poker.NullPoker &&
                    myPokers[i] == poker)
                {
                    return true;
                }
            }

            return false;
        }

        private int GetMyPokerCount()
        {
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (myPokers == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < myPokers.Length; i++)
            {
                if (myPokers[i] != Poker.NullPoker)
                {
                    count++;
                }
            }

            return count;
        }

        private List<Poker> GetMyPokerList()
        {
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (myPokers == null || myPokers.Length <= 0)
            {
                return null;
            }

            var list = new List<Poker>();
            for (int i = 0; i < myPokers.Length; i++)
            {
                var number = myPokers[i];
                if (number == Poker.NullPoker)
                {
                    continue;
                }

                list.Add(new Poker(number));
            }

            return list;
        }

        private void RemoveCardLayoutPoker(List<Poker> list)
        {
            if (list == null || list.Count <= 0)
            {
                return;
            }

            foreach (var poker in list)
            {
                if (poker == null)
                {
                    continue;
                }

                RemoveCardLayoutPoker(poker.Number);
            }
        }

        private void RemoveCardLayoutPoker(int poker)
        {
            var playingData = _playingData.Read();
            var myPokers = playingData == null ? null : playingData.my_pokers;
            if (myPokers == null || myPokers.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < myPokers.Length; i++)
            {
                if (myPokers[i] != Poker.NullPoker &&
                    myPokers[i] == poker)
                {
                    myPokers[i] = Poker.NullPoker;
                }
            }
        }

        /// <summary>
        /// 上一次有值的出牌。
        /// 用于检验当前出牌是否合法。
        /// </summary>
        private com.morln.game.gd.command.PokerPattern LastValueChuPai()
        {
            var tableUser = _tableUser.Read();
            var playingData = _playingData.Read();
            // 当前自己的座位，向之前找三个，第一个非不出的牌型，就是。
            var seat = tableUser.MySeat;
            for (int i = 1; i <= 3; i++)
            {
                var chuPai = playingData.LastChuPaiOfSeat((seat + 4 - i) % 4);
                if (chuPai != null &&
                    chuPai.type != PatternType.NULL &&
                    chuPai.type != PatternType.BUCHU)
                {
                    return chuPai;
                }
            }

            return null;
        }

        /// <summary>
        /// 已经转换成逻辑PokerPattern对象。
        /// </summary>
        private PokerPattern LastMatchedValueChuPai()
        {
            var sessionPattern = LastValueChuPai();
            if (sessionPattern == null)
            {
                return null;
            }

            if (sessionPattern.pokers == null)
            {
                return null;
            }

            var list = new List<Poker>();
            for (int i = 0; i < sessionPattern.pokers.Length; i++)
            {
                list.Add(new Poker(sessionPattern.pokers[i]));
            }

            var matched = _matcher.Match(list);
            return matched;
        }

        #endregion

        public float HideCardJinHuanGongColorDelayTime = 3;

        public void HideCardJinHuanGongColor(bool immediately)
        {
            if (immediately)
            {
                foreach (var k in _slotMap)
                {
                    var slot = k.Value;
                    if (slot == null)
                    {
                        continue;
                    }

                    slot.JinHuanGong = false;
                }
            }
            else
            {
                StartCoroutine(HideCardjinHuanGongColor());
            }
        }

        private IEnumerator HideCardjinHuanGongColor()
        {
            yield return new WaitForSeconds(HideCardJinHuanGongColorDelayTime);

            if (_slotMap == null)
            {
                yield break;
            }

            foreach (var k in _slotMap)
            {
                var slot = k.Value;
                if (slot == null)
                {
                    continue;
                }

                slot.JinHuanGong = false;
            }
        }
    }
}