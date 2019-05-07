namespace Dmm.Constant
{
    public class CheckinStatus
    {
        /// <summary>
        /// 日期已过，却没有签到。
        /// </summary>
        public const int Passed = 0;

        /// <summary>
        /// 该日期已经签到过了。
        /// </summary>
        public const int Checked = 1;

        /// <summary>
        /// 比服务器端多一个状态，用以表示本月还没有到的日期，尚未签到。
        /// </summary>
        public const int UnChecked = 2;
    }
}