using System.Collections.Generic;

namespace Dmm.Game
{
    public class SlotListMap
    {
        /// <summary>
        /// 缓存所有的卡槽。
        /// _slotMap与MyPokers保持一致，不管怎么操作，当前拥有的所有卡槽，以_slotMap为准。
        /// </summary>
        private readonly Dictionary<int, CardSlot> _slotMap = new Dictionary<int, CardSlot>();

        /// <summary>
        /// 保存所有卡槽的列表。
        /// </summary>
        private readonly List<List<CardSlot>> _slotList = new List<List<CardSlot>>();
    }
}