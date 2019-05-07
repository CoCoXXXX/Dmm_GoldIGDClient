using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.PU
{
    public class PToastHandler : MessageHandlerAdapter<Toast>
    {
        private readonly IDialogManager _dialogManager;

        public PToastHandler(IDialogManager dialogManager) :
            base(Server.PServer, Msg.CmdType.PU.TOAST)
        {
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(Toast msg)
        {
            var text = msg.content;
            if (string.IsNullOrEmpty(text))
                return;

            switch (msg.type)
            {
                case ToastType.Normal:
                    _dialogManager.ShowToast(text, 3);
                    break;

                case ToastType.Error:
                    _dialogManager.ShowToast(text, 3, true);
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