namespace Dmm.PokerLogic
{
    public class PokerSuitType
    {
        /// <summary>
        /// 空。
        /// 大小王。
        /// </summary>
        public const int NULL = 0;

        /// <summary>
        /// 黑桃。 
        /// </summary>
        public const int SPADE = 1;

        /// <summary>
        /// 梅花。 
        /// </summary>
        public const int CLUB = 2;

        /// <summary>
        /// 方片。
        /// </summary>
        public const int DIAMOND = 3;

        /// <summary>
        /// 红心。
        /// </summary>
        public const int HEART = 4;

        public static int ValueOf(int type)
        {
            switch (type)
            {
                case NULL:
                case SPADE:
                case CLUB:
                case DIAMOND:
                case HEART:
                    return type;

                default:
                    return NULL;
            }
        }

        public static string LabelOf(int suitType)
        {
            switch (suitType)
            {
                case SPADE:
                    return "♠";

                case CLUB:
                    return "♣";

                case DIAMOND:
                    return "♦";

                case HEART:
                    return "♥";

                default:
                    return "NULL";
            }
        }
    }
}