using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.HU
{
    public class HToastHandler : MessageHandlerAdapter<Toast>
    {
        private readonly IDialogManager _dialogManager;

        public HToastHandler(IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.TOAST)
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