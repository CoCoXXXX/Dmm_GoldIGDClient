using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class LeaveRoomResultHandler : MessageHandlerAdapter<LeaveRoomResult>
    {
        private readonly IDataContainer<LeaveRoomResult> _leaveRoomResult;

        private readonly IDataContainer<Room> _currentRoom;

        private readonly IDialogManager _dialogManager;

        private readonly IDataContainer<ChooseRoomResult> _chooseRoomResult;

        private readonly IDataContainer<ChooseRoomFail> _chooseRoomFail;

        private readonly IDataContainer<GLoginResult> _gLoginResult;

        private readonly IDataContainer<List<Room>> _roomList;

        private readonly RemoteAPI _remoteAPI;

        private readonly INetworkManager _network;

        public LeaveRoomResultHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            INetworkManager networkManager) :
            base(Server.HServer, Msg.CmdType.HU.LEAVE_ROOM_RESULT)
        {
            _leaveRoomResult = dataRepository.GetContainer<LeaveRoomResult>(DataKey.LeaveRoomResult);
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _chooseRoomResult = dataRepository.GetContainer<ChooseRoomResult>(DataKey.ChooseRoomResult);
            _chooseRoomFail = dataRepository.GetContainer<ChooseRoomFail>(DataKey.ChooseRoomFail);
            _gLoginResult = dataRepository.GetContainer<GLoginResult>(DataKey.GLoginResult);
            _roomList = dataRepository.GetContainer<List<Room>>(DataKey.RoomList);
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _network = networkManager;
        }

        protected override void DoHandle(LeaveRoomResult res)
        {
            _leaveRoomResult.Write(res, Time.time);

            if (res == null)
            {
                return;
            }

            if (res.result == ResultCode.OK)
            {
                UpdateRoom(res.room);

                _chooseRoomResult.ClearNotInvalidate();
                _chooseRoomFail.ClearNotInvalidate();

                _gLoginResult.ClearAndInvalidate(Time.time);
            }

            if (res.result == ResultCode.LEAVE_WILL_PUNISH)
            {
                _dialogManager.ShowDialog<PunishTipDialog>(DialogName.PunishTipDialog, false, false,
                    (dialog) =>
                    {
                        dialog.ApplyData(
                            _currentRoom.Read(),
                            "依然退出",
                            () => _remoteAPI.LeaveRoom(true)
                        );
                        dialog.Show();
                    });
            }

            if (_network.GetServer() == Server.GServer)
            {
                _network.StartConnectHServer();
            }
        }

        /// <summary>
        /// 更新房间数据。
        /// </summary>
        /// <param name="dataRepository"></param>
        /// <param name="roomList"></param>
        private void UpdateRoom(List<Room> roomList)
        {
            if (roomList == null || roomList.Count <= 0)
            {
                return;
            }

            var list = _roomList.Read();
            if (list == null)
            {
                return;
            }

            list.Clear();
            list.AddRange(roomList);
            _roomList.Invalidate(Time.time);
        }
    }
}