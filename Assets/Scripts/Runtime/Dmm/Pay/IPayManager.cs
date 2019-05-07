using com.morln.game.gd.command;

namespace Dmm.Pay
{
    public interface IPayManager
    {
        void Pay(TradeNoResult trade, int payChannel);
        float GetPayResultTime(int payChannel);
        AlipayResult GetAlipayResultData();
        IapResult GetIapPayResultData();
        WxPayResult GetWxPayResultData();
        MiPayResult GetMiPayResultData();
    }
}