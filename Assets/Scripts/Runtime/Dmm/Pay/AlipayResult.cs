namespace Dmm.Pay
{
    /// <summary>
    /// 支付宝支付的结果。
    /// </summary>
    public class AlipayResult
    {
        /// <summary>
        /// 订单支付成功。
        /// </summary>
        public const string StatusOk = "9000";

        /// <summary>
        /// 订单正在处理中。
        /// </summary>
        public const string StatusProcessing = "8000";

        /// <summary>
        /// 订单支付失败。
        /// </summary>
        public const string StatusFail = "4000";

        /// <summary>
        /// 用户中途取消。
        /// </summary>
        public const string StatusCanceled = "6001";

        /// <summary>
        /// 网络出错。
        /// </summary>
        public const string StatusNetworkError = "6002";

        public const string KeyStatus = "status";

        public string status;

        public const string KeyResult = "result";

        public string result;

        public const string KeyMemo = "memo";

        public string memo;

        public AlipayResult()
        {
        }

        public AlipayResult(string status, string result, string memo)
        {
            this.status = status;
            this.result = result;
            this.memo = memo;
        }
    }
}