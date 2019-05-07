namespace Dmm.Constant
{
    public class GuanDanType
    {
        /**
         * 普通掼蛋。
         */
        public const int NORMAL = 0;

        /**
         * 团团转。
         */
        public const int TTZ = 1;

        public static string LabelOf(int type)
        {
            switch (type)
            {
                case NORMAL:
                    return "经典玩法";

                case TTZ:
                    return "团团转";

                default:
                    return "";
            }
        }
    }
}