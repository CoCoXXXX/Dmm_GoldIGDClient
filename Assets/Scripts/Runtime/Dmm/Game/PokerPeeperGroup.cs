using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.PokerLogic;
using UnityEngine;
using Zenject;

namespace Dmm.Game
{
    public class PokerPeeperGroup : MonoBehaviour
    {
        #region Inject

        private PatternValue _patternValue;

        private IDataContainer<HostInfoResult> _hostInfo;

        private IDataContainer<PokerPeeperData> _pokerPeeperData;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _hostInfo = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);
            _pokerPeeperData = dataRepository.GetContainer<PokerPeeperData>(DataKey.PokerPeeperData);
        }

        #endregion

        public void OnDisable()
        {
            DisposeIdleSlots();
            DisposeSlots(_slotListLeft, _slotMapLeft);
            DisposeSlots(_slotListRight, _slotMapRight);
            DisposeSlots(_slotListTop, _slotMapTop);
            DisposeSlots(_slotListBottom, _slotMapBottom);
        }

        private void Awake()
        {
            _patternValue = new PatternValue(GetCurrentHost);
        }

        public void Update()
        {
            RefreshContent();
        }

        private float RefreshTime { get; set; }

        public void RefreshContent()
        {
            if (RefreshTime >= _pokerPeeperData.Timestamp)
            {
                return;
            }

            RefreshTime = _pokerPeeperData.Timestamp;

            RefreshPokerPeeperTop();
            RefreshPokerPeeperLeft();
            RefreshPokerPeeperRight();
            RefreshPokerPeeperBottom();
        }

        public float CardWidth = 37;

        public float CardHeight = 26;

        #region top

        public Vector2 TopPivot = new Vector2(0, 245);

        private readonly Dictionary<int, MiniCardSlot> _slotMapTop = new Dictionary<int, MiniCardSlot>();

        private readonly List<List<MiniCardSlot>> _slotListTop = new List<List<MiniCardSlot>>();

        private void RefreshPokerPeeperTop()
        {
            var pokerPeeper = _pokerPeeperData.Read();
            var data = pokerPeeper != null ? pokerPeeper.TopData : null;
            CheckAndClearEmptySlot(_slotMapTop, _slotListTop, data);
            UpdateSlotContent(_slotMapTop, _slotListTop, data);
            SortCardLayout(_slotListTop);
            UpdateSlotDimentionTop();
        }

        private void UpdateSlotDimentionTop()
        {
            if (_slotListTop.Count <= 0)
            {
                return;
            }

            float startX = 0;
            float startY = 0;

            // 顶部的牌的原点是(0.5, 1)
            if (_slotListTop.Count > 1)
            {
                startX = -(CardWidth * (_slotListTop.Count - 1) / 2);
            }
            else
            {
                startX = 0;
            }

            int siblingIndex = 0;
            for (int c = 0; c < _slotListTop.Count; c++)
            {
                var col = _slotListTop[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    slot.RectTransform.pivot = new Vector2(0.5f, 1);
                    ResizeSlot(slot, CardWidth, CardHeight);
                    MoveSlot(slot, TopPivot.x + startX + CardWidth * c, TopPivot.y + startY - CardHeight * r,
                        siblingIndex);
                    siblingIndex++;
                }
            }
        }

        #endregion

        #region left

        public Vector2 LeftPivot = new Vector2(-480, 30);

        private readonly Dictionary<int, MiniCardSlot> _slotMapLeft = new Dictionary<int, MiniCardSlot>();

        private readonly List<List<MiniCardSlot>> _slotListLeft = new List<List<MiniCardSlot>>();

        private void RefreshPokerPeeperLeft()
        {
            var pokerPeeper = _pokerPeeperData.Read();
            var data = pokerPeeper != null ? pokerPeeper.LeftData : null;
            CheckAndClearEmptySlot(_slotMapLeft, _slotListLeft, data);
            UpdateSlotContent(_slotMapLeft, _slotListLeft, data);
            SortCardLayout(_slotListLeft);
            UpdateSlotDimentionLeft();
        }

        private void UpdateSlotDimentionLeft()
        {
            if (_slotListLeft.Count <= 0)
            {
                return;
            }

            float startX = 0;
            float startY = 0;

            // 左边的牌的原点是(0, 0.5)
            if (_slotListLeft.Count > 1)
            {
                startY = CardHeight * (_slotListLeft.Count - 1) / 2;
            }
            else
            {
                startY = 0;
            }

            int siblingIndex = 0;
            for (int c = 0; c < _slotListLeft.Count; c++)
            {
                var col = _slotListLeft[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    slot.RectTransform.pivot = new Vector2(0, 0.5f);
                    ResizeSlot(slot, CardWidth, CardHeight);
                    MoveSlot(slot, LeftPivot.x + startX + CardWidth * r, LeftPivot.y + startY - CardHeight * c,
                        siblingIndex);
                    siblingIndex++;
                }
            }
        }

        #endregion

        #region right

        public Vector2 RightPivot = new Vector2(480, 30);

        private readonly Dictionary<int, MiniCardSlot> _slotMapRight = new Dictionary<int, MiniCardSlot>();

        private readonly List<List<MiniCardSlot>> _slotListRight = new List<List<MiniCardSlot>>();

        private void RefreshPokerPeeperRight()
        {
            var pokerPeeper = _pokerPeeperData.Read();
            var data = pokerPeeper != null ? pokerPeeper.RightData : null;
            CheckAndClearEmptySlot(_slotMapRight, _slotListRight, data);
            UpdateSlotContent(_slotMapRight, _slotListRight, data);
            SortCardLayout(_slotListRight);
            UpdateSlotDimentionRight();
        }

        private void UpdateSlotDimentionRight()
        {
            if (_slotListRight.Count <= 0)
            {
                return;
            }

            float startX = 0;
            float startY = 0;

            // 右边的牌的原点是(1, 0.5)
            if (_slotListRight.Count > 1)
            {
                startY = CardHeight * (_slotListRight.Count - 1) / 2;
            }
            else
            {
                startY = 0;
            }

            int siblingIndex = 0;
            for (int c = 0; c < _slotListRight.Count; c++)
            {
                var col = _slotListRight[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    slot.RectTransform.pivot = new Vector2(1, 0.5f);
                    ResizeSlot(slot, CardWidth, CardHeight);
                    MoveSlot(slot, RightPivot.x + startX - CardWidth * r, RightPivot.y + startY - CardHeight * c,
                        siblingIndex);
                    siblingIndex++;
                }
            }
        }

        #endregion

        #region bottom

        public Vector2 BottomPivot = new Vector2(0, -245);

        private readonly Dictionary<int, MiniCardSlot> _slotMapBottom = new Dictionary<int, MiniCardSlot>();

        private readonly List<List<MiniCardSlot>> _slotListBottom = new List<List<MiniCardSlot>>();

        private void RefreshPokerPeeperBottom()
        {
            var pokerPeeper = _pokerPeeperData.Read();
            var data = pokerPeeper != null ? pokerPeeper.BottomData : null;
            CheckAndClearEmptySlot(_slotMapBottom, _slotListBottom, data);
            UpdateSlotContent(_slotMapBottom, _slotListBottom, data);
            SortCardLayout(_slotListBottom);
            UpdateSlotDimentionBottom();
        }

        private void UpdateSlotDimentionBottom()
        {
            if (_slotListBottom.Count <= 0)
            {
                return;
            }

            float startX = 0;
            float startY = 0;

            // 底部的牌的原点是(0.5, 0)
            if (_slotListBottom.Count > 1)
            {
                startX = -(CardWidth * (_slotListBottom.Count - 1) / 2);
            }
            else
            {
                startX = 0;
            }

            int siblingIndex = 0;
            for (int c = 0; c < _slotListBottom.Count; c++)
            {
                var col = _slotListBottom[c];

                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    slot.RectTransform.pivot = new Vector2(0.5f, 0);
                    ResizeSlot(slot, CardWidth, CardHeight);
                    MoveSlot(slot, BottomPivot.x + startX + CardWidth * c, BottomPivot.y + startY + CardHeight * r,
                        siblingIndex);
                    siblingIndex++;
                }
            }
        }

        #endregion

        private void DisposeSlots(List<List<MiniCardSlot>> slotList, Dictionary<int, MiniCardSlot> slotMap)
        {
            if (slotMap != null)
            {
                foreach (var slot in slotMap.Values)
                {
                    Destroy(slot.gameObject);
                }

                slotMap.Clear();
            }

            if (slotList != null)
            {
                foreach (var list in slotList)
                {
                    if (list != null)
                        list.Clear();
                }

                slotList.Clear();
            }
        }

        private void CheckAndClearEmptySlot(Dictionary<int, MiniCardSlot> slotMap, List<List<MiniCardSlot>> slotList,
            byte[] pokers)
        {
            if (pokers == null)
            {
                ClearAllSlots(slotMap, slotList);
                return;
            }

            var removeFromMap = new List<int>();
            foreach (var p in slotMap.Keys)
            {
                if (!ContainsPoker(pokers, p))
                {
                    removeFromMap.Add(p);
                }
            }

            foreach (var r in removeFromMap)
            {
                if (slotMap.ContainsKey(r))
                {
                    var slot = slotMap[r];
                    RemoveSlotFromLayout(slotList, slot);
                }
            }

            removeFromMap.Clear();

            var removeFromLayout = new List<MiniCardSlot>();
            for (int c = 0; c < slotList.Count; c++)
            {
                var col = slotList[c];
                for (int r = 0; r < col.Count; r++)
                {
                    var slot = col[r];
                    if (slot.Poker == null ||
                        !ContainsPoker(pokers, slot.Poker.Number))
                    {
                        if (!removeFromLayout.Contains(slot))
                        {
                            removeFromLayout.Add(slot);
                        }
                    }
                }
            }

            foreach (var slot in removeFromLayout)
            {
                RemoveSlotFromLayout(slotList, slot);
            }

            removeFromLayout.Clear();
        }

        private void UpdateSlotContent(Dictionary<int, MiniCardSlot> slotMap, List<List<MiniCardSlot>> slotList,
            byte[] pokers)
        {
            if (pokers == null || pokers.Length <= 0)
            {
                ClearAllSlots(slotMap, slotList);
                return;
            }

            for (int p = 0; p < pokers.Length; p++)
            {
                var number = pokers[p];

                if (slotMap.ContainsKey(number)) continue;

                var poker = new Poker(number);
                var added = false;
                for (int i = 0; i < slotList.Count; i++)
                {
                    var col = slotList[i];
                    if (col.Count <= 0)
                    {
                        slotList.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if (col[0].NumType == poker.NumType)
                    {
                        var slot = GetMiniCardSlot();
                        slot.Poker = poker;

                        col.Add(slot);
                        slotMap[number] = slot;

                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    var col = new List<MiniCardSlot>();

                    var slot = GetMiniCardSlot();
                    slot.Poker = poker;
                    slotMap[number] = slot;

                    col.Add(slot);

                    slotList.Add(col);
                }
            }
        }

        private void SortCardLayout(List<List<MiniCardSlot>> slotList)
        {
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int c = 1; c < slotList.Count; c++)
                {
                    var left = slotList[c - 1];
                    var right = slotList[c];
                    if (left.Count > 0 && right.Count > 0)
                    {
                        int res = _patternValue.Compare(left[0].Poker, right[0].Poker);
                        if (res == CompareResult.SMALLER)
                        {
                            slotList[c - 1] = right;
                            slotList[c] = left;
                            flag = true;
                        }
                    }
                }
            }

            for (int c = 0; c < slotList.Count; c++)
            {
                SortCardColumn(slotList[c]);
            }
        }

        private void SortCardColumn(List<MiniCardSlot> column)
        {
            if (column == null || column.Count <= 0) return;

            bool flag = true;
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
                                flag = true;
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

        private void ClearAllSlots(Dictionary<int, MiniCardSlot> slotMap, List<List<MiniCardSlot>> slotList)
        {
            if (slotList != null)
            {
                foreach (var col in slotList)
                {
                    if (col.Count > 0)
                    {
                        foreach (var slot in col) SetSlotIdle(slot);
                        col.Clear();
                    }
                }

                slotList.Clear();
            }

            if (slotMap != null) slotMap.Clear();
        }

        private void RemoveSlotFromLayout(List<List<MiniCardSlot>> slotList, MiniCardSlot slot)
        {
            if (slot == null)
            {
                return;
            }

            SetSlotIdle(slot);

            if (slotList == null)
            {
                return;
            }

            for (int c = 0; c < slotList.Count; c++)
            {
                var col = slotList[c];
                col.Remove(slot);

                if (col.Count <= 0)
                {
                    slotList.RemoveAt(c);
                    c--;
                }
            }
        }

        private void MoveSlot(MiniCardSlot slot, float x, float y, int siblingIndex)
        {
            slot.RectTransform.SetSiblingIndex(siblingIndex);
            slot.RectTransform.anchoredPosition = new Vector2(x, y);
        }

        private void ResizeSlot(MiniCardSlot slot, float width, float height)
        {
            slot.RectTransform.sizeDelta = new Vector2(width, height);
            slot.RectTransform.localScale = new Vector3(1, 1, 1);
        }

        private readonly Queue<MiniCardSlot> _idleSlots = new Queue<MiniCardSlot>();

        private void SetSlotIdle(MiniCardSlot slot)
        {
            slot.Clear();
            slot.gameObject.SetActive(false);

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

        public MiniCardSlot MiniCardSlotPrefab;

        private MiniCardSlot GetMiniCardSlot()
        {
            MiniCardSlot slot;

            if (_idleSlots.Count > 0)
            {
                slot = _idleSlots.Dequeue();
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot = Instantiate(MiniCardSlotPrefab) as MiniCardSlot;
            }

            if (slot)
            {
                slot.CardHelper = CardHelper;
                slot.Clear();
                slot.transform.SetParent(transform, false);
                slot.transform.localPosition = new Vector3(0, 0, 0);
            }

            return slot;
        }

        private bool ContainsPoker(byte[] pokers, int p)
        {
            if (pokers == null || pokers.Length <= 0)
            {
                return false;
            }

            for (int i = 0; i < pokers.Length; i++)
            {
                if (pokers[i] == p)
                {
                    return true;
                }
            }

            return false;
        }

        #region 结束的时候显示的对方的牌

        public CardHelper CardHelper;

        public Sprite GetEndRoundSprite(int numType, int suitType)
        {
            return CardHelper.GetEndRoundCard(numType, suitType);
        }

        #endregion

        private int GetCurrentHost()
        {
            var hostInfo = _hostInfo.Read();
            if (hostInfo == null)
            {
                return PokerLogic.PokerNumType.P2;
            }

            return hostInfo.GetCurrentHost();
        }
    }
}