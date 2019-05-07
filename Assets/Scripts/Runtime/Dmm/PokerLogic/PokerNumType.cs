namespace Dmm.PokerLogic
{
    public class PokerNumType
    {
        public const int NULL = 0;

        public const int P2 = 2;
        public const int P3 = 3;
        public const int P4 = 4;
        public const int P5 = 5;
        public const int P6 = 6;
        public const int P7 = 7;
        public const int P8 = 8;
        public const int P9 = 9;
        public const int P10 = 10;
        public const int PJ = 11;
        public const int PQ = 12;
        public const int PK = 13;
        public const int PA = 14;

        /// <summary>
        /// 主牌的代码。
        /// </summary>
        public const int PHost = 15;

        /// <summary>
        /// 小王。
        /// </summary>
        public const int PX = 20;

        /// <summary>
        /// 大王。
        /// </summary>
        public const int PD = 21;

        public static string LabelOf(int numType)
        {
            switch (numType)
            {
                case P2:
                    return "2";

                case P3:
                    return "3";

                case P4:
                    return "4";

                case P5:
                    return "5";

                case P6:
                    return "6";

                case P7:
                    return "7";

                case P8:
                    return "8";

                case P9:
                    return "9";

                case P10:
                    return "10";

                case PJ:
                    return "J";

                case PQ:
                    return "Q";

                case PK:
                    return "K";

                case PA:
                    return "A";

                case PX:
                    return "X";

                case PD:
                    return "D";

                case PHost:
                    return "Host";

                default:
                    return "NULL";
            }
        }
    }
}