using System.Collections.Generic;
using com.morln.game.gd.command;

namespace Dmm.Checkin
{
    public class CheckinConditionComparator : IComparer<CheckinCondition>
    {
        public int Compare(CheckinCondition x, CheckinCondition y)
        {
            var v1 = x != null ? x.day_count : 0;
            var v2 = y != null ? y.day_count : 0;
            return v1 - v2;
        }
    }
}