namespace Dmm.Constant
{
    public class PayChannelType
    {
        public const int ALIPAY_WEB = 1;
        public const int ALIPAY_CLIENT = 2;
        public const int IOS_IAP = 3;
        public const int SMS_WIYUN = 4;
        public const int SMS_LIANTONG = 5;
        public const int ALIPAY_IOS = 6;
        public const int TAOBAO = 7;
        public const int SMS_YIDONG = 8;
        public const int SMS_DIANXIN = 9;
        public const int BAO_RUAN = 10;
        public const int KUAIYONG = 12;
        public const int MLD_LIANTONG = 13;
        public const int XIAOMI = 14;
        public const int NDUO = 15;
        public const int UNI_PAY = 16;
        public const int NIBIRU = 17;
        public const int YIDONG_MM = 18;
        public const int MDO = 19;
        public const int MILI = 20;

        public const int WEI_XIN = 100;

        public const int TEST_PAY = 10000;

        public static string GetPayChannelName(int payChannel)
        {
            switch (payChannel)
            {
                case ALIPAY_CLIENT:
                case ALIPAY_IOS:
                    return "支付宝钱包";

                case IOS_IAP:
                    return "苹果支付";

                case WEI_XIN:
                    return "微信支付";

                case XIAOMI:
                    return "小米支付";

                case TEST_PAY:
                    return "测试支付";

                default:
                    return "其他支付方式";
            }
        }

        public static string GetPayChannelNameId(int payChannel)
        {
            switch (payChannel)
            {
                case ALIPAY_CLIENT:
                    return "alipay_android";

                case ALIPAY_IOS:
                    return "alipay_ios";

                case IOS_IAP:
                    return "iap";

                case WEI_XIN:
                    return "wxpay";

                case XIAOMI:
                    return "mipay";

                case TEST_PAY:
                    return "testpay";

                default:
                    return "other";
            }
        }
    }
}