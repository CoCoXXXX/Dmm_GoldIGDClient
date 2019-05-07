using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class BRoomInOutHandler : MessageHandlerAdapter<BRoomInOut>
    {
        private readonly IDataContainer<Queue<BRoomInOut>> _broomInOutQueue;

        private readonly IDataContainer<List<Room>> _room;

        public BRoomInOutHandler(IDataRepository dataRepository) :
            base(Server.HServer, Msg.CmdType.HU.ROOM_IN_OUT)
        {
            _broomInOutQueue = dataRepository.GetContainer<Queue<BRoomInOut>>(DataKey.BRoomInOut);
            _room = dataRepository.GetContainer<List<Room>>(DataKey.RoomList);
        }

        protected override void DoHandle(BRoomInOut msg)
        {
            // 先更房间数据。
            var rooms = _room.Read();
            if (rooms != null && rooms.Count > 0)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    var room = rooms[i];
                    if (room != null &&
                        room.room_id == msg.room_id)
                        room.current_player_num = msg.room_player_count;
                }
            }

            // 再更新RoomTable中的RoomBtn上显示的玩家数量。
            var roomInOueQueue = _broomInOutQueue.Read();
            if (roomInOueQueue == null)
            {
                roomInOueQueue = new Queue<BRoomInOut>();
            }
            roomInOueQueue.Enqueue(msg);
            _broomInOutQueue.Write(roomInOueQueue, Time.time);
        }
    }
}