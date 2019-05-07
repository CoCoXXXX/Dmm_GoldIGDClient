using System.Collections.Generic;
using com.morln.game.gd.command;

namespace Dmm.Checkin
{
    /// <summary>
    /// 对CheckinItem依照时间逆序排列。
    /// </summary>
    public class CheckinItemComparator : IComparer<CheckinItem>
    {
        public int Compare(CheckinItem item1, CheckinItem item2)
        {
            var v1 = item1 != null ? item1.day : 0;
            var v2 = item2 != null ? item2.day : 0;
            return v2 - v1;
        }
    }
}