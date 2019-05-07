namespace Dmm.Pay
{
    public class IapResult
    {
        public const int IapSuccess = 0;
        public const int IapFailProductInfo = -1;
        public const int IapFailPayment = -2;
        public const int IapFailTradeInvalid = -3;
        public const int IapFailCannotPay = -4;

        public int Result;

        public string OutTradeNo;

        public string Receipt;
    }
}