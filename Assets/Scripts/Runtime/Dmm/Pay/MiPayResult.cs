namespace Dmm.Pay
{
    public class MiPayResult
    {
        public const int OK = 0;

        public const int CANCEL = -18004;

        public const int FAIL = -18003;

        public const int EXECUTING = -18006;

        public int result;

        public string outTradeNo;

        public MiPayResult()
        {
        }

        public MiPayResult(int result, string outTradeNo)
        {
            this.result = result;
            this.outTradeNo = outTradeNo;
        }
    }
}