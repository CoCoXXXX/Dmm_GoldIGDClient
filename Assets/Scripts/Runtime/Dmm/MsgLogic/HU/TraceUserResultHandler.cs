using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class TraceUserResultHandler : MessageHandlerAdapter<TraceUserResult>
    {
        private readonly IDataContainer<TraceUserResult> _traceUserResult;

        private readonly IDataContainer<Room> _currentRoom;

        private readonly IDialogManager _dialogManager;

        private readonly RemoteAPI _remoteAPI;

        public TraceUserResultHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI) :
            base(Server.HServer, Msg.CmdType.HU.TRACE_USER_RESULT)
        {
            _traceUserResult = dataRepository.GetContainer<TraceUserResult>(DataKey.TraceUserResult);
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
        }

        protected override void DoHandle(TraceUserResult msg)
        {
            _traceUserResult.Write(msg, Time.time);
            if (msg.result.code == ResultCode.LEAVE_WILL_PUNISH)
            {
                if (!string.IsNullOrEmpty(msg.username))
                {
                    _dialogManager.ShowDialog<PunishTipDialog>(DialogName.PunishTipDialog, false, false,
                        (dialog) =>
                        {
                            dialog.ApplyData(
                                _currentRoom.Read(),
                                "依然跟踪好友",
                                () => _remoteAPI.TraceUser(msg.username, true)
                            );
                        });
                }
            }
        }
    }
}