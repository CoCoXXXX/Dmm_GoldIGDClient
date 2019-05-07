using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BPlayerChooseSeatHandler : MessageHandlerAdapter<BPlayerChooseSeat>
    {
        private readonly IDataContainer<TableUserData> _tableUser;

        private readonly IDataContainer<User> _user;

        private readonly IDataContainer<Table> _table;

        public BPlayerChooseSeatHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.B_PLAYER_CHOOSE_SEAT_V6)
        {
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
        }

        //玩家选择座位。
        protected override void DoHandle(BPlayerChooseSeat msg)
        {
            var user = _user.Read();
            var username = msg.username;
            var seat = msg.seat;
            var tableUser = _tableUser.Read();

            if (seat < 0 || seat > 3)
                return;

            var table = _table.Read();
            if (table == null)
                return;

            var empty = false;
            switch (seat)
            {
                case 0:
                    empty = table.user1 == null;
                    break;

                case 1:
                    empty = table.user2 == null;
                    break;

                case 2:
                    empty = table.user3 == null;
                    break;

                case 3:
                    empty = table.user4 == null;
                    break;
            }

            if (!empty)
                return;

            var originSeat = tableUser.GetSeatOfUser(username);
            if (originSeat == -1)
                return;

            var originUser = tableUser.GetUserAtSeat(originSeat);
            if (originUser == null)
                return;

            tableUser.SetUserAtSeat(originUser, seat);
            tableUser.SetUserAtSeat(null, originSeat);

            _tableUser.Write(tableUser, Time.time);
        }
    }
}