namespace Dmm.Constant
{
    public class VipExchangeType
    {
        /// <summary>
        /// 无需购买。
        /// </summary>
        public const int CanNotBuy = 0;

        /// <summary>
        /// 可以购买。
        /// </summary>
        public const int BuyOk = 1;

        /// <summary>
        /// 可以续费。
        /// </summary>
        public const int Renew = 2;

        /// <summary>
        /// 可以升级。
        /// </summary>
        public const int Upgrade = 3;

        public static string LabelOf(int type)
        {
            switch (type)
            {
                case CanNotBuy:
                    return "无需购买";

                case BuyOk:
                    return "购买";

                case Renew:
                    return "续费";

                case Upgrade:
                    return "升级";

                default:
                    return "购买";
            }
        }
    }
}