using System;

namespace Dmm.Util
{
    public class DateUtil
    {
        public const long DayDuration = 1000 * 3600 * 24;

        public static readonly DateTime Dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 解析Java格式的时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ParseJavaTime(long time)
        {
            var span = new TimeSpan(time * 10000);
            return Dt1970.Add(span);
        }

        public static long ToJavaTime(DateTime dt)
        {
            var span = dt.ToUniversalTime() - Dt1970;
            return (long) span.TotalMilliseconds;
        }

        public static DateTime GetMonthStartDay(DateTime time)
        {
            return new DateTime(
                time.Year,
                time.Month,
                1,
                0,
                0,
                0,
                time.Kind
            );
        }

        public static bool IsSameDay(long t1, long t2)
        {
            var d1 = ParseJavaTime(t1);
            var d2 = ParseJavaTime(t2);

            return IsSameDay(d1, d2);
        }

        public static bool IsSameDay(DateTime d1, DateTime d2)
        {
            // 应该使用本地时间进行对比，这样对是否在一天之内的判断才准确。
            // 如果使用UTC时间，则在相差的8个小时内，可能出现与本地不一致的情况。
            d1 = d1.ToLocalTime();
            d2 = d2.ToLocalTime();

            if (d1.Year != d2.Year)
                return false;

            if (d1.Month != d2.Month)
                return false;

            if (d1.Day != d2.Day)
                return false;

            return true;
        }

        public static bool IsYesterday(long now, long time)
        {
            var d1 = ParseJavaTime(now);
            var d2 = ParseJavaTime(time);

            return IsSameDay(d1.AddDays(-1), d2);
        }
    }
}