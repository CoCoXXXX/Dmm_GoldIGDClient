using System.Collections.Generic;
using System.Text;
using Dmm.PokerLogic;

namespace Dmm.Util
{
    public class PokerLogicUtil
    {
        public static com.morln.game.gd.command.PokerPattern ConvertToSessionPattern(PokerPattern pattern)
        {
            if (pattern == null)
                return null;

            var res = new com.morln.game.gd.command.PokerPattern();
            res.type = pattern.Type;

            var pokers = pattern.Pokers;
            if (pokers != null && pokers.Count > 0)
            {
                res.pokers = new byte[pokers.Count];
                for (int i = 0; i < pokers.Count; i++)
                    res.pokers[i] = (byte) pokers[i].Number;
            }

            return res;
        }

        public static int PokerNumTypeOf(com.morln.game.gd.command.PokerNumType numType)
        {
            switch (numType)
            {
                case com.morln.game.gd.command.PokerNumType.p2:
                    return 2;

                case com.morln.game.gd.command.PokerNumType.p3:
                    return 3;

                case com.morln.game.gd.command.PokerNumType.p4:
                    return 4;

                case com.morln.game.gd.command.PokerNumType.p5:
                    return 5;

                case com.morln.game.gd.command.PokerNumType.p6:
                    return 6;

                case com.morln.game.gd.command.PokerNumType.p7:
                    return 7;

                case com.morln.game.gd.command.PokerNumType.p8:
                    return 8;

                case com.morln.game.gd.command.PokerNumType.p9:
                    return 9;

                case com.morln.game.gd.command.PokerNumType.p10:
                    return 10;

                case com.morln.game.gd.command.PokerNumType.pJ:
                    return 11;

                case com.morln.game.gd.command.PokerNumType.pQ:
                    return 12;

                case com.morln.game.gd.command.PokerNumType.pK:
                    return 13;

                case com.morln.game.gd.command.PokerNumType.pA:
                    return 14;

                case com.morln.game.gd.command.PokerNumType.pHost:
                    return 15;

                case com.morln.game.gd.command.PokerNumType.pX:
                    return 20;

                case com.morln.game.gd.command.PokerNumType.pD:
                    return 21;

                default:
                    return 0;
            }
        }

        public static com.morln.game.gd.command.PokerNumType ToSessionNumType(int type)
        {
            switch (type)
            {
                case PokerNumType.P2:
                    return com.morln.game.gd.command.PokerNumType.p2;

                case PokerNumType.P3:
                    return com.morln.game.gd.command.PokerNumType.p3;

                case PokerNumType.P4:
                    return com.morln.game.gd.command.PokerNumType.p4;

                case PokerNumType.P5:
                    return com.morln.game.gd.command.PokerNumType.p5;

                case PokerNumType.P6:
                    return com.morln.game.gd.command.PokerNumType.p6;

                case PokerNumType.P7:
                    return com.morln.game.gd.command.PokerNumType.p7;

                case PokerNumType.P8:
                    return com.morln.game.gd.command.PokerNumType.p8;

                case PokerNumType.P9:
                    return com.morln.game.gd.command.PokerNumType.p9;

                case PokerNumType.P10:
                    return com.morln.game.gd.command.PokerNumType.p10;

                case PokerNumType.PJ:
                    return com.morln.game.gd.command.PokerNumType.pJ;

                case PokerNumType.PQ:
                    return com.morln.game.gd.command.PokerNumType.pQ;

                case PokerNumType.PK:
                    return com.morln.game.gd.command.PokerNumType.pK;

                case PokerNumType.PA:
                    return com.morln.game.gd.command.PokerNumType.pA;

                case PokerNumType.PX:
                    return com.morln.game.gd.command.PokerNumType.pX;

                case PokerNumType.PD:
                    return com.morln.game.gd.command.PokerNumType.pD;

                default:
                    return com.morln.game.gd.command.PokerNumType.NULL_NUM;
            }
        }

        public static string LabelOfSessionNumType(com.morln.game.gd.command.PokerNumType numType)
        {
            var value = PokerNumTypeOf(numType);
            return PokerNumType.LabelOf(value);
        }

        public static List<Poker> ToPokerList(byte[] pokers)
        {
            if (pokers == null || pokers.Length <= 0)
            {
                return null;
            }

            var list = new List<Poker>();
            for (int i = 0; i < pokers.Length; i++)
            {
                var p = pokers[i];
                if (p != Poker.NullPoker)
                {
                    list.Add(new Poker(p));
                }
            }

            return list;
        }

        public static string FormatPokers(List<Poker> pokers)
        {
            if (pokers == null || pokers.Count <= 0)
                return "BUCHU";

            var buf = new StringBuilder();
            for (int i = 0; i < pokers.Count; i++)
            {
                buf.Append(pokers[i]);
            }

            return buf.ToString();
        }

        public static bool PokerListEqual(List<Poker> pokers1, List<Poker> pokers2)
        {
            if (pokers1 == null || pokers2 == null)
            {
                if (pokers1 == null && pokers2 == null)
                    return true;
                else
                    return false;
            }
            else
            {
                if (pokers1.Count != pokers2.Count)
                    return false;

                for (int i = 0; i < pokers1.Count; i++)
                {
                    var p = pokers1[i];
                    if (!pokers2.Contains(p))
                        return false;
                }

                for (int i = 0; i < pokers2.Count; i++)
                {
                    var p = pokers2[i];
                    if (!pokers1.Contains(p))
                        return false;
                }

                return true;
            }
        }

        public static List<Poker> ConvertToPokerList(PokerPattern pattern, PatternValue value)
        {
            if (pattern == null)
                return null;

            var pokers = new List<Poker>();
            if (pattern.Type == PatternType.BUCHU)
                return pokers;

            var pool = new List<Poker>();
            if (pattern.Pokers != null)
                pool.AddRange(pattern.Pokers);

            var pile = pattern.HeadPile;
            while (pile != null)
            {
                var list = new List<Poker>();
                for (int i = 0; i < pool.Count; i++)
                {
                    var p = pool[i];
                    if (p.NumType == pile.NumType)
                    {
                        list.Add(p);
                        pool.RemoveAt(i);
                        i--;
                    }
                }

                if (list.Count < pile.Count)
                {
                    for (int i = 0, n = pile.Count - list.Count; i < n; i++)
                    {
                        for (int c = 0; c < pool.Count; c++)
                        {
                            if (value.IsHeartHost(pool[c]))
                            {
                                list.Add(pool[c]);
                                pool.RemoveAt(c);
                                c--;
                            }
                        }
                    }
                }

                pokers.AddRange(list);

                pile = pile.Next;
            }

            return pokers;
        }
    }
}