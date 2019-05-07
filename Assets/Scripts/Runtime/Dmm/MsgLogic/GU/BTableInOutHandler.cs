using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BTableInOutHandler : MessageHandlerAdapter<BTableInOut>
    {
        private readonly IDataContainer<Table> _currentTable;

        private readonly IDataContainer<TableUserData> _tableUser;

        public BTableInOutHandler(IDataRepository dataRepository) :
            base(Server.GServer, Msg.CmdType.GU.B_TABLE_IN_OUT_V6)
        {
            _currentTable = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        //桌子逻辑
        protected override void DoHandle(BTableInOut msg)
        {
            var tableUser = _tableUser.Read();
            tableUser.SetUserAtSeat(msg.in_or_out == 1 ? msg.player : null, msg.seat);

            _tableUser.Write(tableUser, Time.time);

            // 出现桌子上的人员变化的时候，就需要重置桌子的状态了。
            var table = _currentTable.Read();
            if (table != null)
            {
                table.ResetTable();
                _currentTable.Invalidate(Time.time);
            }
        }
    }
}