using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Pay;

namespace Dmm.Util
{
    public class DataUtil
    {
        private const string UsernamePattern = "^[A-Za-z0-9]+$";

        public static bool ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return Regex.IsMatch(username, UsernamePattern);
        }

        private const string PasswordPattern = "^[A-Za-z0-9]+$";

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            return Regex.IsMatch(password, PasswordPattern);
        }

        public static bool ValidateNickname(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                return false;

            if (nickname.Contains("\r") || nickname.Contains("\n"))
                return false;

            if (nickname.Length > 16)
                return false;

            return true;
        }

        private const string EmailPattern = @"(?<email>[\da-zA-Z_]+@([-\dA-Za-z]+\.)+[a-zA-Z]{2,})";

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, EmailPattern);
        }

        public static bool ValidateCity(string city)
        {
            if (string.IsNullOrEmpty(city))
                return false;

            if (city.Length > 20)
                return false;

            return true;
        }

        public static bool ValidateSelfDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return false;

            if (description.Length > 30)
                return false;

            return true;
        }

        public static string ExtractFileNameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var idx = url.LastIndexOf('/');
            return url.Substring(idx + 1, url.Length - 1 - idx);
        }

        public static Dictionary<string, string> ParseParameter(string url)
        {
            var res = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(url))
                return res;

            try
            {
                var pairs = url.Split('&');
                if (pairs.Length <= 0)
                    return res;

                for (int i = 0; i < pairs.Length; i++)
                {
                    var p = pairs[i];
                    var kv = p.Split('=');

                    if (kv.Length >= 2)
                    {
                        if (res.ContainsKey(kv[0]))
                            res[kv[0]] = kv[1].Replace("\"", "");
                        else
                            res.Add(kv[0], kv[1].Replace("\"", ""));
                    }
                }
            }
            catch (Exception e)
            {
            }

            return res;
        }

        public static List<Currency> GetCurrencyList(CheckTradeResult res)
        {
            if (res == null)
                return null;

            var list = new List<Currency>();
            if (res.added_currency_count > 0)
            {
                var c = new Currency();
                c.type = res.currency_type;
                c.count = res.added_currency_count;
                list.Add(c);
            }

            if (res.gift != null && res.gift.curreny.Count > 0)
                list.AddRange(res.gift.curreny);

            return list;
        }

        /// <summary>
        /// 计算支付包等值的金蛋数量。
        /// </summary>
        /// <returns></returns>
        public static long CalculateGeValue(Prepayment prepayment)
        {
            if (prepayment == null)
                return 0;

            var geCount = 0L;
            var ypCount = 0L;

            if (prepayment.currency_type == CurrencyType.GOLDEN_EGG)
                geCount += prepayment.currency_count;
            else if (prepayment.currency_type == CurrencyType.YIN_PIAO)
                ypCount += prepayment.currency_count;

            var extraPack = prepayment.extra_pack;
            if (extraPack != null && extraPack.Count > 0)
            {
                for (int i = 0; i < extraPack.Count; i++)
                {
                    var cur = extraPack[i];
                    if (cur.type == CurrencyType.GOLDEN_EGG)
                        geCount += cur.count;
                    else if (cur.type == CurrencyType.YIN_PIAO)
                        ypCount += cur.count;
                }
            }

            return geCount + ypCount * 100;
        }

        public static long CalculateGeValue(int currencyType, long count)
        {
            if (currencyType == CurrencyType.GOLDEN_EGG)
                return count;
            else if (currencyType == CurrencyType.YIN_PIAO)
                return count * 100;
            else
                return 0;
        }

        public static long CalculateGeValue(Currency currency)
        {
            if (currency == null)
                return 0;

            return CalculateGeValue(currency.type, currency.count);
        }

        public static long CalculateGeValue(List<Currency> list)
        {
            if (list == null || list.Count <= 0)
                return 0;

            var count = 0L;
            for (int i = 0; i < list.Count; i++)
            {
                var cur = list[i];
                count += CalculateGeValue(cur.type, cur.count);
            }

            return count;
        }

        public static bool UpdateUserPublic(User from, User to)
        {
            if (from == null || to == null)
            {
                return false;
            }

            if (from == to)
            {
                return true;
            }

            to.nickname = from.nickname;
            to.sex = from.sex;
            to.description = from.description;
            to.city = from.city;

            to.level = from.level;
            to.title = from.title;
            to.money = from.money;
            to.second_money = from.second_money;

            to.vip = from.vip;

            to.round_count = from.round_count;
            to.win_count = from.win_count;
            to.escape_count = from.escape_count;

            to.hair = from.hair;
            to.body = from.body;
            to.item_show.Clear();

            if (from.item_show != null && from.item_show.Count > 0)
            {
                to.item_show.AddRange(from.item_show);
            }

            to.ready = from.ready;
            to.temp_leave = from.temp_leave;

            return true;
        }

        public static bool UpdateTable(Table from, Table to)
        {
            if (from == null || to == null)
                return false;

            to.room_id = from.room_id;
            to.table_id = from.table_id;
            to.type = from.type;
            to.player_count = from.player_count;
            to.target_host = from.target_host;
            to.team1_host = from.team1_host;
            to.team2_host = from.team2_host;
            to.host_team = from.host_team;

            UpdateUserPublic(from.user1, to.user1);
            UpdateUserPublic(from.user2, to.user2);
            UpdateUserPublic(from.user3, to.user3);
            UpdateUserPublic(from.user4, to.user4);

            to.state = from.state;
            to.round_count = from.round_count;

            return true;
        }

        public static User GetUser(Table table, int seat)
        {
            if (table == null)
                return null;

            switch (seat)
            {
                case 0:
                    return table.user1;

                case 1:
                    return table.user2;

                case 2:
                    return table.user3;

                case 3:
                    return table.user4;

                default:
                    return null;
            }
        }

        public static long GetCurrency(User data, int currencyType)
        {
            if (data == null)
                return 0;

            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    return data.money;

                case CurrencyType.YIN_PIAO:
                    return data.second_money;

                case CurrencyType.EXP:
                    return data.exp;

                case CurrencyType.VIP:
                    return data.vip;

                default:
                    return 0;
            }
        }

        public static void UseCommodity(User user, Commodity commodity, bool use)
        {
            if (user == null)
                return;

            if (commodity == null ||
                string.IsNullOrEmpty(commodity.name))
                return;

            switch (commodity.type)
            {
                case CommodityType.Hair:
                    user.hair = use ? commodity.name : null;
                    break;

                case CommodityType.Body:
                    user.body = use ? commodity.name : null;
                    break;

                case CommodityType.DeskItem:
                    if (use)
                    {
                        user.item_show.Clear();
                        user.item_show.Add(commodity.name);
                    }
                    else
                    {
                        user.item_show.Clear();
                    }
                    break;
            }
        }

        public static string FormatNickname(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                return nickname;

            // 删除换行符和空格。
            nickname = nickname.Replace('\n', ' ');
            nickname = nickname.Replace('\r', ' ');
            nickname = nickname.Trim();

            if (nickname.Length > 20)
                return nickname.Substring(0, 20) + "...";
            else
                return nickname;
        }

        public static string FormatWinRate(float winRate)
        {
            return string.Format("{0:p}", winRate);
        }

        private static readonly string[] PrimaryDiv = {"__;__"};
        private static readonly string[] EqualDiv = {"__:__"};

        public static AlipayResult ParseAlipayResult(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            var list = content.Split(PrimaryDiv, StringSplitOptions.None);
            if (list.Length <= 0)
                return null;

            var res = new AlipayResult();
            for (int i = 0; i < list.Length; i++)
            {
                var pair = list[i].Split(EqualDiv, StringSplitOptions.None);
                if (pair.Length >= 2)
                {
                    if (StringUtil.AreEqual(AlipayResult.KeyStatus, pair[0]))
                        res.status = pair[1];
                    else if (StringUtil.AreEqual(AlipayResult.KeyMemo, pair[0]))
                        res.memo = pair[1];
                    else if (StringUtil.AreEqual(AlipayResult.KeyResult, pair[0]))
                        res.result = pair[1];
                }
            }

            return res;
        }

        public static Currency NewCurrency(int type, long count)
        {
            var cur = new Currency();
            cur.type = type;
            cur.count = count;
            return cur;
        }

        public static string BuildAmountText(long amount)
        {
            // 按照单位来。
            // 大于100000000的时候，按照亿来计算。
            if (amount >= 100000000)
            {
                var res = amount / 100000000 + "亿";
                if (amount % 100000000 >= 10000)
                    res += amount % 100000000 / 10000 + "万";

                return res;
            }

            // 大于1000000的时候，按照万来计算。
            if (amount >= 1000000)
                return amount / 10000 + "万";

            return amount + "";
        }
    }
}