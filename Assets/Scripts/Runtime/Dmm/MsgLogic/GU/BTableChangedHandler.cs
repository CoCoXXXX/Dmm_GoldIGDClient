using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BTableChangedHandler : MessageHandlerAdapter<BTableChanged>
    {
        private readonly IDataContainer<Room> _room;

        public BTableChangedHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.B_TABLE_CHANGED_V6)
        {
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
        }

        //桌子逻辑
        protected override void DoHandle(BTableChanged tableChange)
        {
            if (tableChange == null)
                return;

            if (tableChange.table == null)
                return;

            var room = _room.Read();
            if (room == null)
                return;

            if (room.type != RoomType.Normal)
                return;

            var tables = room.table;
            if (tables == null || tables.Count <= 0)
                return;

            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].table_id == tableChange.table.table_id)
                {
                    tables[i] = tableChange.table;
                    _room.Invalidate(Time.time);
                    break;
                }
            }
        }
    }
}