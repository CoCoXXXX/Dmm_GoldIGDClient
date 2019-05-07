using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BTempLeaveHandler : MessageHandlerAdapter<BTempLeave>
    {
        private readonly IDataContainer<User> _myUser;

        public BTempLeaveHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.B_TEMP_LEAVE_V6)
        {
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(BTempLeave msg)
        {
            var user = _myUser.Read();
            if (user == null)
            {
                return;
            }

            if (StringUtil.AreEqual(user.username, msg.username))
            {
                user.temp_leave = msg.temp_leave_or_not;
                _myUser.Invalidate(Time.time);
            }
        }
    }
}