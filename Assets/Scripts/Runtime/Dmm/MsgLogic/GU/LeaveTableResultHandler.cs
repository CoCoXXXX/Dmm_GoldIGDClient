using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class LeaveTableResultHandler : MessageHandlerAdapter<LeaveTableResult>
    {
        private readonly IDialogManager _dialogManager;

        private readonly IDataContainer<LeaveTableResult> _leaveTableResult;

        private readonly IDataContainer<Room> _currentRoom;

        private readonly IDataContainer<ChooseTableResult> _chooseTableResult;

        private readonly IDataContainer<StartRound> _startRound;

        private readonly IDataContainer<com.morln.game.gd.command.RoundEnd> _raceRoundEnd;

        private readonly IDataContainer<BRoundEnd> _roundEnd;

        private readonly RemoteAPI _remoteAPI;

        public LeaveTableResultHandler(
            IDataRepository dataRepository,
            RemoteAPI remoteAPI,
            IDialogManager dialogManager)
            : base(Server.GServer, Msg.CmdType.GU.LEAVE_TABLE_RESULT_V6)
        {
            _dialogManager = dialogManager;
            _leaveTableResult = dataRepository.GetContainer<LeaveTableResult>(DataKey.LeaveTableResult);
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _chooseTableResult = dataRepository.GetContainer<ChooseTableResult>(DataKey.ChooseTableResult);
            _startRound = dataRepository.GetContainer<StartRound>(DataKey.StartRound);
            _raceRoundEnd = dataRepository.GetContainer<com.morln.game.gd.command.RoundEnd>(DataKey.RaceRoundEnd);
            _roundEnd = dataRepository.GetContainer<BRoundEnd>(DataKey.BRoundEnd);
            _remoteAPI = remoteAPI;
        }

        protected override void DoHandle(LeaveTableResult msg)
        {
            if (msg == null)
            {
                return;
            }

            _leaveTableResult.Write(msg, Time.time);

            if (msg.result == ResultCode.OK)
            {
                // 离桌成功，则清空选桌数据。
                _chooseTableResult.ClearAndInvalidate(Time.time);

                // 离桌成功，清空开局和结算数据。
                _startRound.ClearAndInvalidate(0);
                _roundEnd.ClearNotInvalidate();
                _raceRoundEnd.ClearNotInvalidate();
            }

            // 离桌不成功，则什么都不做。
            _chooseTableResult.Invalidate(Time.time);

            if (msg.result == ResultCode.LEAVE_WILL_PUNISH)
            {
                var room = _currentRoom.Read();

                _dialogManager.ShowDialog<PunishTipDialog>(DialogName.PunishTipDialog, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(
                            room,
                            "依然退出",
                            () => _remoteAPI.LeaveTable(true)
                        );
                        dialog.Show();
                    });
            }
        }
    }
}