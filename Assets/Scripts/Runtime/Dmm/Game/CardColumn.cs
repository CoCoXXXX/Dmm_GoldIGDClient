using System.Collections.Generic;
using Dmm.PokerLogic;
using UnityEngine;

namespace Dmm.Game
{
    /// <summary>
    /// 牌列。
    /// </summary>
    public class CardColumn : MonoBehaviour
    {
        /// <summary>
        /// 当前牌列中的所有卡槽。
        /// </summary>
        public readonly List<CardSlot> Cards = new List<CardSlot>();

        /// <summary>
        /// 排列的MajorNumType。
        /// </summary>
        public int NumType
        {
            get
            {
                if (Cards.Count > 0)
                {
                    return Cards[0].NumType;
                }

                return PokerNumType.NULL;
            }
        }

        /// <summary>
        /// 如果牌列符合牌型，则缓存这个牌型。
        /// </summary>
        public int PatternType { get; private set; }
    }
}