using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class UserReadyHandler : MessageHandlerAdapter<BUserReady>
    {
        private readonly IDataContainer<TableUserData> _tableUserData;

        public UserReadyHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.B_USER_READY_V6)
        {
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        protected override void DoHandle(BUserReady msg)
        {
            var seat = msg.seat;
            var readyOrNot = msg.ready_or_not;

            var tableUser = _tableUserData.Read();
            var user = tableUser.GetUserAtSeat(seat);
            if (user != null)
            {
                user.ready = readyOrNot;
                tableUser.SetUserAtSeat(user, seat);
                _tableUserData.Write(tableUser, Time.time);
            }
        }
    }
}