namespace Dmm.PokerLogic
{
    public class PatternType
    {
        public const int NULL = -1000;

        public const int BUCHU = -1;

        public const int A = 1;

        public const int AA = 2;

        public const int AAA = 3;

        public const int XXXX = 4;

        public const int AAABBB = 5;

        public const int AAAXX = 6;

        public const int AABBCC = 7;

        public const int ABCDE = 8;

        public const int SuperABCDE = 9;

        public const int XXDD = 10;

        public static int GetMajorPileCount(int type)
        {
            switch (type)
            {
                case BUCHU:
                    return 0;

                case A:
                    return 1;

                case AA:
                    return 1;

                case AAA:
                    return 1;

                case XXXX:
                    return 1;

                case AAABBB:
                    return 2;

                case AAAXX:
                    return 1;

                case AABBCC:
                    return 3;

                case ABCDE:
                    return 5;

                case SuperABCDE:
                    return 5;

                case XXDD:
                    return 2;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 是不是顺子。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStraight(int type)
        {
            return type == ABCDE || type == SuperABCDE || type == AABBCC || type == AAABBB;
        }

        /// <summary>
        /// 是不是炸弹。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBomb(int type)
        {
            return type == XXXX || type == SuperABCDE || type == XXDD;
        }

        /// <summary>
        /// 是不是大炸弹。
        /// </summary>
        /// <param name="bomb"></param>
        /// <returns></returns>
        public static bool IsBigXXXX(PokerPattern bomb)
        {
            if (bomb == null)
                return false;

            if (bomb.Type != XXXX)
                return false;

            return bomb.PokerCount > 5;
        }

        public static string LabelOf(int type)
        {
            switch (type)
            {
                case BUCHU:
                    return "BUCHU";

                case A:
                    return "A";

                case AA:
                    return "AA";

                case AAA:
                    return "AAA";

                case AAAXX:
                    return "AAAXX";

                case AABBCC:
                    return "AABBCC";

                case AAABBB:
                    return "AAABBB";

                case ABCDE:
                    return "ABCDE";

                case SuperABCDE:
                    return "SuperABCDE";

                case XXXX:
                    return "XXXX";

                case XXDD:
                    return "XXDD";

                default:
                    return "NULL";
            }
        }
    }
}