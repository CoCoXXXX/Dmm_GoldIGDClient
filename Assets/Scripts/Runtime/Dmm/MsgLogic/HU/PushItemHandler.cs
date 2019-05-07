using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.HU
{
    public class PushItemHandler : MessageHandlerAdapter<PushItem>
    {
        private readonly IDialogManager _dialogManager;

        public PushItemHandler(IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.PUSH_ITEM)
        {
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(PushItem msg)
        {
            _dialogManager.ShowDialog<PushItemDialog>(DialogName.PushItemDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(msg);
                    dialog.Show();
                });
        }
    }
}