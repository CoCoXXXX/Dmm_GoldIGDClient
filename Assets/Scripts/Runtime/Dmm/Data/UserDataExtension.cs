using System;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Util;
using UnityEngine;

namespace Dmm.Data
{
    public static class UserDataExtension
    {
        /// <summary>
        /// 当调用时 user是null时依然会调用进来，调用的地方不会报错
        /// </summary>
        /// <param name="user"></param>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        public static long MyCurrency(this User user, int currencyType)
        {
            if (user == null)
            {
                return 0;
            }

            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    return user.money;

                case CurrencyType.YIN_PIAO:
                    return user.second_money;

                case CurrencyType.EXP:
                    return user.exp;

                case CurrencyType.YUAN_BAO:
                    return user.yuan_bao;

                case CurrencyType.CARD_RECORDER:
                    return user.card_recorder_expire_time;

                case CurrencyType.RECHECKIN_CARD:
                    return user.recheckin_card;

                default:
                    return 0;
            }
        }

        public static int CardRecorderLeftDays(this User user)
        {
            if (user == null)
            {
                return 0;
            }

            var javaExpireTime = user.MyCurrency(CurrencyType.CARD_RECORDER);
            if (javaExpireTime <= 0)
            {
                return 0;
            }

            var javaNow = DateUtil.ToJavaTime(DateTime.Now);
            var delta = javaExpireTime - javaNow;
            if (delta <= 0)
            {
                return 0;
            }

            return (int) (delta / DateUtil.DayDuration + (delta % DateUtil.DayDuration > 0 ? 1 : 0));
        }

        public static bool HasCardRecorder(this User user)
        {
            if (user == null)
            {
                return false;
            }

            return user.vip > 0 || user.CardRecorderLeftDays() > 0;
        }

        public static string Username(this User user)
        {
            if (user == null)
            {
                return "";
            }

            return user.username;
        }

        public static string Nickname(this User user)
        {
            if (user == null)
            {
                return "爱掼蛋玩家";
            }

            return user.nickname;
        }

        public static string HeadIconPic(this User user)
        {
            if (user == null || string.IsNullOrEmpty(user.headicon_url))
            {
                return "default";
            }

            return string.Format("headicon-{0}", WWW.EscapeURL(user.headicon_url));
        }

        public static bool IsTempLeave(this User user)
        {
            if (user == null)
            {
                return false;
            }

            return user.temp_leave == 1;
        }
    }
}