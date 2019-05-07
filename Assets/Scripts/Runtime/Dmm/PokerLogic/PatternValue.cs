using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public class PatternValue : IPatternValue
    {
        public readonly static Dictionary<int, int> NumValues = new Dictionary<int, int>();

        static PatternValue()
        {
            NumValues[PokerNumType.NULL] = 0;

            NumValues[PokerNumType.P2] = 2;
            NumValues[PokerNumType.P3] = 3;
            NumValues[PokerNumType.P4] = 4;
            NumValues[PokerNumType.P5] = 5;
            NumValues[PokerNumType.P6] = 6;
            NumValues[PokerNumType.P7] = 7;
            NumValues[PokerNumType.P8] = 8;
            NumValues[PokerNumType.P9] = 9;
            NumValues[PokerNumType.P10] = 10;
            NumValues[PokerNumType.PJ] = 11;
            NumValues[PokerNumType.PQ] = 12;
            NumValues[PokerNumType.PK] = 13;
            NumValues[PokerNumType.PA] = 14;
            NumValues[PokerNumType.PHost] = 15;

            NumValues[PokerNumType.PX] = 20;
            NumValues[PokerNumType.PD] = 21;
        }

        /// <summary>
        /// 获取CurrentHost的方法。
        /// </summary>
        /// <returns></returns>
        public delegate int CurrentHostGetter();

        private readonly CurrentHostGetter _currentHostGetter;

        public PatternValue(CurrentHostGetter currentHostGetter)
        {
            _currentHostGetter = currentHostGetter;
        }

        /// <summary>
        /// 当前的主牌。
        /// </summary>
        public int CurrentHost
        {
            get
            {
                if (_currentHostGetter != null)
                    return _currentHostGetter();

                return PokerNumType.P2;
            }
        }

        public bool IsHeartHost(Poker poker)
        {
            return poker.NumType == CurrentHost && poker.SuitType == PokerSuitType.HEART;
        }

        public int Compare(PokerPattern p1, PokerPattern p2)
        {
            if (p1 == null || p2 == null)
            {
                return CompareResult.PATTERN_NOT_MATCH;
            }

            if (p1.IsNull && !p2.IsNull)
            {
                return CompareResult.SMALLER;
            }

            if (!p1.IsNull && p2.IsNull)
            {
                return CompareResult.BIGGER;
            }

            if (p1.IsNull && p2.IsNull)
            {
                return CompareResult.EQUAL;
            }

            if (p1.Type == PatternType.XXDD)
            {
                return CompareResult.BIGGER;
            }

            if (p2.Type == PatternType.XXDD)
            {
                return CompareResult.SMALLER;
            }

            if (PatternType.IsBomb(p1.Type) && !PatternType.IsBomb(p2.Type))
            {
                return CompareResult.BIGGER;
            }

            if (!PatternType.IsBomb(p1.Type) && PatternType.IsBomb(p2.Type))
            {
                return CompareResult.SMALLER;
            }

            if (PatternType.IsBomb(p1.Type) && PatternType.IsBomb(p2.Type))
            {
                float c1 = 0;
                float c2 = 0;

                if (p1.Type == PatternType.XXXX)
                {
                    c1 = p1.PokerCount;
                }
                else if (p1.Type == PatternType.SuperABCDE)
                {
                    c1 = 5.5f;
                }

                if (p2.Type == PatternType.XXXX)
                {
                    c2 = p2.PokerCount;
                }
                else if (p2.Type == PatternType.SuperABCDE)
                {
                    c2 = 5.5f;
                }

                float res = c1 - c2;
                if (res > 0.4)
                {
                    return CompareResult.BIGGER;
                }
                else if (res < -0.4)
                {
                    return CompareResult.SMALLER;
                }
                else
                {
                    int v1 = ValueOf(p1);
                    int v2 = ValueOf(p2);

                    if (v1 == v2)
                    {
                        return CompareResult.EQUAL;
                    }

                    if (v1 > v2)
                    {
                        return CompareResult.BIGGER;
                    }
                    else
                    {
                        return CompareResult.SMALLER;
                    }
                }
            }

            if (p1.Type == p2.Type && p1.PokerCount == p2.PokerCount)
            {
                int v1 = ValueOf(p1);
                int v2 = ValueOf(p2);

                if (v1 == v2)
                {
                    return CompareResult.EQUAL;
                }

                if (v1 > v2)
                {
                    return CompareResult.BIGGER;
                }
                else
                {
                    return CompareResult.SMALLER;
                }
            }

            return CompareResult.PATTERN_NOT_MATCH;
        }

        public int Compare(int numType1, int numType2)
        {
            int v1 = ValueOf(numType1);
            int v2 = ValueOf(numType2);

            if (v1 > v2)
            {
                return CompareResult.BIGGER;
            }
            else if (v1 == v2)
            {
                return CompareResult.EQUAL;
            }
            else
            {
                return CompareResult.SMALLER;
            }
        }

        public int Compare(Poker p1, Poker p2)
        {
            if (p1 == null || p2 == null)
            {
                return CompareResult.INVALID_VALUE;
            }
            return Compare(p1.NumType, p2.NumType);
        }

        public int ValueOf(PokerPattern pattern)
        {
            if (PatternType.IsStraight(pattern.Type))
            {
                if (pattern.MajorNumType == PokerNumType.PA)
                {
                    return 1;
                }
                else
                {
                    return NumValues[pattern.MajorNumType];
                }
            }
            else
            {
                if (pattern.MajorNumType == CurrentHost)
                {
                    return PokerNumType.PHost;
                }
                else
                {
                    return NumValues[pattern.MajorNumType];
                }
            }
        }

        public int ValueOf(int numType)
        {
            if (numType == CurrentHost)
            {
                return PokerNumType.PHost;
            }
            else
            {
                return NumValues[numType];
            }
        }

        public int ValueInStraight(int numType)
        {
            return NumValues[numType];
        }
    }
}