using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class RequestAwardResultHandler : MessageHandlerAdapter<RequestAwardResult>
    {
        private readonly IDataContainer<RequestAwardResult> _requestAwardResult;

        private readonly IDialogManager _dialog;

        public RequestAwardResultHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.REQUEST_AWARD_RESULT)
        {
            _requestAwardResult = dataRepository.GetContainer<RequestAwardResult>(DataKey.RequestAwardResult);
            _dialog = dialogManager;
        }

        protected override void DoHandle(RequestAwardResult msg)
        {
            _requestAwardResult.Write(msg, Time.time);

            if (msg.result == ResultCode.OK)
            {
                var award = msg.award;
                if (award != null)
                {
                    _dialog.ShowDialog<GetAwardDialog>(DialogName.GetAwardDialog, false, false,
                        (dialog) =>
                        {
                            dialog.ApplyData(award);
                            dialog.Show();
                        });
                }
                else
                {
                    _dialog.ShowToast("领取奖励失败", 3, true);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(msg.err_msg))
                {
                    _dialog.ShowToast(msg.err_msg, 3, true);
                }
            }
        }
    }
}