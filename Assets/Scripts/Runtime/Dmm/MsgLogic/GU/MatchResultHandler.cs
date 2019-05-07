using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;

namespace Dmm.MsgLogic.GU
{
    public class MatchResultHandler : MessageHandlerAdapter<MatchResult>
    {
        private readonly IDataContainer<Room> _room;

        private readonly RemoteAPI _remoteApi;

        private readonly IDialogManager _dialogManager;

        public MatchResultHandler(RemoteAPI remoteApi, IDialogManager dialogManager, IDataRepository dataRepository)
            : base(Server.GServer, Msg.CmdType.GU.MATCH_RESULT_V6)
        {
            _remoteApi = remoteApi;
            _dialogManager = dialogManager;
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
        }

        protected override void DoHandle(MatchResult msg)
        {
            if (msg.result == ResultCode.LEAVE_WILL_PUNISH)
            {
                var room = _room.Read();
                _dialogManager.ShowDialog<PunishTipDialog>(DialogName.PunishTipDialog, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(
                            room,
                            "依然换桌",
                            () => _remoteApi.MatchTable(true)
                        );
                        dialog.Show();
                    });
            }
        }
    }
}