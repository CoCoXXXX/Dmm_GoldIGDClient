using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class CheckTradeResultHandler : MessageHandlerAdapter<CheckTradeResult>
    {
        private readonly IDataContainer<CheckTradeResult> _checkTradeResult;

        private readonly IDialogManager _dialogManager;

        public CheckTradeResultHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.CHECK_TRADE_RESULT)
        {
            _checkTradeResult = dataRepository.GetContainer<CheckTradeResult>(DataKey.CheckTradeResult);
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(CheckTradeResult msg)
        {
            _checkTradeResult.Write(msg, Time.time);

            if (msg.result == ResultCode.OK)
            {
                // 在这里弹出，是因为登陆的时候也会主动向玩家发送CheckTradeResult的命令。
                _dialogManager.ShowDialog<GetAwardDialog>(DialogName.GetAwardDialog, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(
                            "恭喜您成功充值",
                            AwardType.Charge,
                            DataUtil.GetCurrencyList(msg));
                        dialog.Show();
                    });
            }
        }
    }
}