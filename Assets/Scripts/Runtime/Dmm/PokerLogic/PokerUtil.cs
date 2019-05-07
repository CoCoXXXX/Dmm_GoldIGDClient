using System.Collections.Generic;

namespace Dmm.PokerLogic
{
    public class PokerUtil
    {
        /// <summary>
        /// 计算牌的number。
        /// </summary>
        /// <param name="numType">牌面</param>
        /// <param name="suitType">花色</param>
        /// <param name="deckNumber">第几副牌：从1开始计算</param>
        /// <returns></returns>
        public static int CalculateNumber(int numType, int suitType, int deckNumber)
        {
            int c = 0;

            if (numType == PokerNumType.PX)
            {
                c = 53;
            }
            else if (numType == PokerNumType.PD)
            {
                c = 54;
            }
            else if (numType == PokerNumType.PA)
            {
                c = 13 * (suitType - 1) + 1;
            }
            else if (numType == PokerNumType.P2)
            {
                c = 13 * (suitType - 1) + 2;
            }
            else
            {
                c = 13 * (suitType - 1) + numType;
            }

            return c + 54 * (deckNumber - 1);
        }

        public static int NumTypeOf(int number)
        {
            int numCode = (number + 53) % 54 + 1;
            if (numCode > 52)
                return numCode == 53 ? PokerNumType.PX : PokerNumType.PD;
            else
                return (numCode + 11) % 13 + 2;
        }

        public static int SuitTypeOf(int number)
        {
            int suitCode = number % 54;
            if (suitCode > 52 || suitCode == 0)
                return PokerSuitType.NULL;
            else
                return (suitCode - 1) / 13 + 1;
        }

        public static Poker ParsePoker(string pokerName)
        {
            var suit = pokerName[0];
            var num = pokerName[1];

            int suitType = PokerSuitType.NULL;
            switch (suit)
            {
                case 'H':
                    suitType = PokerSuitType.HEART;
                    break;

                case 'S':
                    suitType = PokerSuitType.SPADE;
                    break;

                case 'C':
                    suitType = PokerSuitType.CLUB;
                    break;

                case 'D':
                    suitType = PokerSuitType.DIAMOND;
                    break;
            }

            int numType = PokerNumType.NULL;
            switch (num)
            {
                case '2':
                    numType = PokerNumType.P2;
                    break;

                case '3':
                    numType = PokerNumType.P3;
                    break;

                case '4':
                    numType = PokerNumType.P4;
                    break;

                case '5':
                    numType = PokerNumType.P5;
                    break;

                case '6':
                    numType = PokerNumType.P6;
                    break;

                case '7':
                    numType = PokerNumType.P7;
                    break;

                case '8':
                    numType = PokerNumType.P8;
                    break;

                case '9':
                    numType = PokerNumType.P9;
                    break;

                case '0':
                    numType = PokerNumType.P10;
                    break;

                case 'J':
                    numType = PokerNumType.PJ;
                    break;

                case 'Q':
                    numType = PokerNumType.PQ;
                    break;

                case 'K':
                    numType = PokerNumType.PK;
                    break;

                case 'A':
                    numType = PokerNumType.PA;
                    break;

                case 'X':
                    numType = PokerNumType.PX;
                    break;

                case 'D':
                    numType = PokerNumType.PD;
                    break;
            }

            return new Poker(numType, suitType);
        }

        public static List<Poker> ParsePokerList(string pokerStr)
        {
            var pokers = new List<Poker>();

            if (!string.IsNullOrEmpty(pokerStr))
            {
                string[] list = pokerStr.Split(',');

                if (list.Length > 0)
                {
                    foreach (var s in list)
                    {
                        var poker = ParsePoker(s.Trim().ToUpper());
                        pokers.Add(poker);
                    }
                }
            }

            return pokers;
        }

        public static byte[] ParseToPokerBytes(string pokerStr)
        {
            var list = ParsePokerList(pokerStr);
            if (list == null)
                return null;

            var bytes = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
                bytes[i] = (byte) list[i].Number;

            return bytes;
        }

        /// <summary>
        /// 测试用，快速构建牌堆。
        /// </summary>
        /// <param name="numType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static PokerPile Pile(int numType, int count)
        {
            var pile = new PokerPile(numType);
            pile.AddPoker(count);
            return pile;
        }

        public static PokerPattern Pattern(
            int type,
            int heartHostCount,
            int majorNumType,
            List<Poker> pokers,
            params PokerPile[] piles)
        {
            var pileList = new List<PokerPile>();
            for (int i = 0; i < piles.Length; i++)
                pileList.Add(piles[i]);

            for (int i = 0; i < pileList.Count - 1; i++)
                pileList[i].Next = pileList[i + 1];

            return new PokerPattern(type, piles[0], majorNumType, pokers);
        }
    }
}