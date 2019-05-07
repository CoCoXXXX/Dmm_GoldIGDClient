using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Msg;
using Dmm.PokerLogic;
using Dmm.Util;

namespace Dmm.Game
{
    public class GameUtil
    {
        #region Currency

        public static void SetMyCurrency(
            User user,
            int currencyType,
            long currencyCount)
        {
            if (user == null)
            {
                return;
            }

            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    user.money = currencyCount;
                    break;

                case CurrencyType.YIN_PIAO:
                    user.second_money = currencyCount;
                    break;

                case CurrencyType.YUAN_BAO:
                    user.yuan_bao = (int) currencyCount;
                    break;

                case CurrencyType.CARD_RECORDER:
                    user.card_recorder_expire_time = currencyCount;
                    break;

                case CurrencyType.RECHECKIN_CARD:
                    user.recheckin_card = (int) currencyCount;
                    break;
            }
        }

        #endregion

        public static Poker GetPokerFromPokerId(int poker)
        {
            if (poker == -1)
            {
                return null;
            }

            return new Poker(poker);
        }

        public static void SetTableUser(Table table, int seat, User data)
        {
            if (table == null)
            {
                return;
            }

            switch (seat)
            {
                case 0:
                    table.user1 = data;
                    break;

                case 1:
                    table.user2 = data;
                    break;

                case 2:
                    table.user3 = data;
                    break;

                case 3:
                    table.user4 = data;
                    break;
            }
        }

        #region Commodity

        public static Commodity GetCommodity(List<Commodity> commodityList, string cname)
        {
            if (string.IsNullOrEmpty(cname))
            {
                return null;
            }

            var list = commodityList;
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (StringUtil.AreEqual(list[i].name, cname))
                {
                    return list[i];
                }
            }

            return null;
        }

        public static bool HasCommodity(Bag bag, Commodity data)
        {
            if (data == null)
                return false;

            return HasCommodity(bag, data.name);
        }

        public static bool HasCommodity(Bag bag, string cname)
        {
            if (bag == null)
            {
                return false;
            }

            if (bag.item == null || bag.item.Count <= 0)
            {
                return false;
            }

            for (int i = 0; i < bag.item.Count; i++)
            {
                var item = bag.item[i];
                if (StringUtil.AreEqual(item.name, cname))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCommodityEquiped(Bag bag, User user, Commodity data)
        {
            if (data == null)
            {
                return false;
            }

            if (!HasCommodity(bag, data))
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            switch (data.type)
            {
                case CommodityType.Hair:
                    if (StringUtil.AreEqual(data.name, user.hair))
                    {
                        return true;
                    }

                    break;

                case CommodityType.Body:
                    if (StringUtil.AreEqual(data.name, user.body))
                    {
                        return true;
                    }

                    break;

                case CommodityType.DeskItem:
                    if (user.item_show != null &&
                        user.item_show.Count >= 1 &&
                        StringUtil.AreEqual(data.name, user.item_show[0]))
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        #endregion

        #region Friend

        public static List<FriendInfo> FriendList(SFriendListResult friendListResult)
        {
            var data = friendListResult;
            if (data == null)
            {
                return null;
            }

            if (data.result.code != ResultCode.OK)
            {
                return null;
            }

            return data.info;
        }

        public static bool IsFriend(SFriendListResult friendListResult, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var friendList = FriendList(friendListResult);
            if (friendList == null)
            {
                return false;
            }

            for (int i = 0; i < friendList.Count; i++)
            {
                if (friendList[i].username == username)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}