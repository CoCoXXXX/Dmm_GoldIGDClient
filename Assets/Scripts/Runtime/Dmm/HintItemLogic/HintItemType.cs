namespace Dmm.HintItemLogic
{
    public class HintItemType
    {
        public const int URL = 0;

        public const int CHARGE = 1;

        public const int COMMODITY = 2;

        public const int WX_INVITE = 3;

        public const int VIP = 4;

        public const int WX_SHARE = 5;

        public const int WX_CIRCLE = 6;

        public const int APP_PROMOTE = 10;

        public static string IdOf(int type)
        {
            switch (type)
            {
                case URL:
                    return "url";

                case CHARGE:
                    return "charge";

                case COMMODITY:
                    return "commodity";

                case VIP:
                    return "vip";

                case WX_SHARE:
                    return "wxshare";

                case WX_CIRCLE:
                    return "wxcircle";

                case APP_PROMOTE:
                    return "app";

                default:
                    return "default";
            }
        }
    }
}