using Dmm.Constant;

namespace Dmm.Data
{
    public class CheckinDayData
    {
        /// <summary>
        /// 日期。
        /// </summary>
        public int Day;

        /// <summary>
        /// 签到的状态。
        /// </summary>
        public int Status;

        /// <summary>
        /// 是否是今天。
        /// </summary>
        public bool IsToday;

        /// <summary>
        /// 是否开启。
        /// 如果日期不在本月之内则不开启。
        /// </summary>
        public bool Enabled;

        public bool Passed
        {
            get { return Status == CheckinStatus.Passed; }
        }

        public bool Checked
        {
            get { return Status == CheckinStatus.Checked; }
        }

        public bool UnChecked
        {
            get { return Status == CheckinStatus.UnChecked; }
        }

        public override string ToString()
        {
            string statusStr;
            switch (Status)
            {
                case CheckinStatus.Checked:
                    statusStr = "Checked";
                    break;

                case CheckinStatus.UnChecked:
                    statusStr = "UnChecked";
                    break;

                case CheckinStatus.Passed:
                    statusStr = "Passed";
                    break;

                default:
                    statusStr = "NULL";
                    break;
            }

            return "[" + Day + "," + statusStr + "," + (Enabled ? "Enabled" : "Disabled") + "]";
        }
    }
}