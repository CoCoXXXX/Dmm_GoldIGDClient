using com.morln.game.gd.command;
using Dmm.Constant;

namespace Dmm.Shop
{
    public class CommodityHelper
    {
        public static int GetPrice(Commodity data)
        {
            if (data == null)
                return 0;

            var priceList = data.level_price;
            if (priceList == null || priceList.Count <= 0)
                return 0;

            return priceList[0];
        }

        public static int GetCurrencyType(Commodity data)
        {
            if (data == null)
                return CurrencyType.GOLDEN_EGG;

            var typeList = data.level_currency_type;
            if (typeList == null || typeList.Count <= 0)
                return CurrencyType.GOLDEN_EGG;

            return typeList[0];
        }
    }
}