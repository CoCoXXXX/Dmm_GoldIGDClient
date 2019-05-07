using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Pay;
using UnityEditor;
using UnityEngine;

namespace Dmm.Game
{
    public class ChargeTestWindow : EditorWindow
    {
        public delegate void OnClickDelegate();

        // 充值的测试流程如下：
        // 显示具有两个支付渠道的支付对话框。
        // 点击支付渠道按钮，发送请求订单命令。
        // 回复测试订单结果命令。
        // 设置测试的客户端支付结果。
        // 手动回复测试的检查订单结果。

        private Prepayment _prepayment;

        private Trade _trade;

        public IMsgRepo MsgRepo;

        public DialogManager DialogManager;

        public PayManager PayManager;

        public void OnEnable()
        {
            MsgRepo = new MsgRepo();

            title = "支付测试";

            _prepayment = new Prepayment();
            _prepayment.name = "TestPrepayment";
            _prepayment.display_name = "测试支付包";
            _prepayment.description = "充值即可获得大礼哦";
            _prepayment.currency_type = CurrencyType.GOLDEN_EGG;
            _prepayment.currency_count = 20000;
            _prepayment.price = 6;
            _prepayment.pay_channel.Add(PayChannelType.IOS_IAP);
            _prepayment.pay_channel.Add(PayChannelType.ALIPAY_IOS);
            _prepayment.pay_channel.Add(PayChannelType.ALIPAY_CLIENT);

            _trade = new Trade();
            _trade.pay_channel = PayChannelType.ALIPAY_IOS;
            _trade.out_trade_no = "KSLDHGKL21934";
        }

        public void OnGUI()
        {
            if (GUILayout.Button("显示支付面板"))
            {
                DialogManager.ShowDialog<PayChannelPanel>(DialogName.PayChannelPanel, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData("test", _prepayment);
                        dialog.Show();
                    });
            }

            if (GUILayout.Button("发送成功的TradeNoResult"))
            {
                var msg = CmdUtil.HU.TradeNoResult(
                    _prepayment.name,
                    1, _prepayment.currency_type, _prepayment.currency_count,
                    _trade, ResultCode.OK, null, null);
                SendResponse(msg);
            }

            if (GUILayout.Button("发送失败的TradeNoResult"))
            {
                var msg = CmdUtil.HU.TradeNoResult(
                    _prepayment.name,
                    1, _prepayment.currency_type, _prepayment.currency_count,
                    _trade, ResultCode.H_TRADE_PREPAYMENT_NOT_FOUND, null, null);
                SendResponse(msg);
            }

            if (GUILayout.Button("设置客户端支付成功的结果"))
            {
                PayManager.AlipayResult("status__:__9000__;__result__:__success=\"true\"");
            }

            if (GUILayout.Button("设置客户端支付失败的结果"))
            {
                PayManager.AlipayResult("status__:__6001__;__memo__:__玩家取消支付");
            }

            if (GUILayout.Button("发送成功的CheckTradeResult"))
            {
                var msg = CmdUtil.HU.CheckTradeResult(
                    ResultCode.OK,
                    _trade,
                    _prepayment.currency_type, _prepayment.currency_count,
                    300000, null);
                SendResponse(msg);
            }

            if (GUILayout.Button("发送等待通知的CheckTradeResult"))
            {
                var msg = CmdUtil.HU.CheckTradeResult(
                    ResultCode.H_TRADE_NOT_PAID,
                    _trade,
                    _prepayment.currency_type, _prepayment.currency_count,
                    300000, null);
                SendResponse(msg);
            }

            if (GUILayout.Button("发送失败的CheckTradeResult"))
            {
                var msg = CmdUtil.HU.CheckTradeResult(
                    ResultCode.H_TRADE_CLOSED,
                    _trade,
                    _prepayment.currency_type, _prepayment.currency_count,
                    300000, null);
                SendResponse(msg);
            }
        }

        private void SendResponse(ProtoMessage msg)
        {
            MsgRepo.ReceiveMsg(msg);
        }
    }
}