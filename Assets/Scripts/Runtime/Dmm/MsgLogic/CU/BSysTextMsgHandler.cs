using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.CU
{
    public class BSysTextMsgHandler : MessageHandlerAdapter<BSysTextMsg>
    {
        private readonly IDialogManager _dialogManager;

        private readonly ISystemMsgController _systemMsgController;

        public BSysTextMsgHandler(
            IDialogManager dialogManager,
            ISystemMsgController systemMsgController)
            : base(Server.GServer, Msg.CmdType.CU.B_SYS_TEXT_MSG_V6)
        {
            _dialogManager = dialogManager;
            _systemMsgController = systemMsgController;
        }

        protected override void DoHandle(BSysTextMsg msg)
        {
            var text = msg.msg;
            if (string.IsNullOrEmpty(text))
                return;

            switch (msg.type)
            {
                case ToastType.Normal:
                    _systemMsgController.Show(text);
                    break;

                case ToastType.Error:
                    _systemMsgController.Show(text, true);
                    break;

                case ToastType.MessageBox:
                    _dialogManager.ShowMessageBox(text);
                    break;

                case ToastType.ConfirmBox:
                    _dialogManager.ShowConfirmBox(text);
                    break;
            }
        }
    }
}