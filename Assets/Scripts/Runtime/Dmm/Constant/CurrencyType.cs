namespace Dmm.Constant
{
    public class CurrencyType
    {
        public const int GOLDEN_EGG = 0;
        public const int YIN_PIAO = 1;

        public const int YUAN_BAO = 100;
        public const int EXP = 200;

        public const int SCORE = 1001;

        public const int VIP = 10001;

        public const int CARD_RECORDER = 10002;

        public const int RECHECKIN_CARD = 10003;

        public const int COMPOUND = 20001;

        public static string LabelOf(int type)
        {
            switch (type)
            {
                case GOLDEN_EGG:
                    return "金蛋";
                case YIN_PIAO:
                    return "银票";
                case YUAN_BAO:
                    return "兑奖券";
                case VIP:
                    return "VIP";
                case EXP:
                    return "经验";
                case SCORE:
                    return "分";
                case CARD_RECORDER:
                    return "记牌器";
                case RECHECKIN_CARD:
                    return "补签卡";
                case COMPOUND:
                    return "大礼包";
                default:
                    return "NULL";
            }
        }

        public static string IdOf(int type)
        {
            switch (type)
            {
                case GOLDEN_EGG:
                    return "ge";

                case YIN_PIAO:
                    return "yp";

                case YUAN_BAO:
                    return "yuanbao";

                case VIP:
                    return "vip";

                case EXP:
                    return "exp";

                case SCORE:
                    return "score";

                case COMPOUND:
                    return "compound";

                default:
                    return "NULL";
            }
        }
    }
}