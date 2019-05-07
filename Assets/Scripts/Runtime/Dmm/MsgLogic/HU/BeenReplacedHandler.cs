using Dmm.App;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.HU
{
    public class BeenReplacedHandler : IMessageHandler
    {
        public Server Server
        {
            get { return Server.HServer; }
        }

        public int CmdType
        {
            get { return Msg.CmdType.HU.BEEN_REPLACED; }
        }

        private readonly IAppController _appController;

        private readonly IDialogManager _dialogManager;

        public BeenReplacedHandler(
            IAppController appController,
            IDialogManager dialogManager
        )
        {
            _appController = appController;
            _dialogManager = dialogManager;
        }

        public void Handle(ProtoMessage msg)
        {
            _appController.ClearAppStateData();
            _dialogManager.ShowDialog<BeenReplacedDialog>(DialogName.BeenReplacedDialog, true, true);
        }
    }
}