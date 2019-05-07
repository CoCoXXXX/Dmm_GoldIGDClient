namespace Dmm.Pay
{
    public class WxPayResult
    {
        public const int Ok = 0;

        public const int Error = -1;

        public const int Cancel = -2;

        public int Result;

        public string ErrMsg;

        public WxPayResult()
        {
        }

        public WxPayResult(int result, string errMsg)
        {
            this.Result = result;
            this.ErrMsg = errMsg;
        }
    }
}