using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class UserInfoResultHandler : MessageHandlerAdapter<UserInfoResult>
    {
        private readonly IDataContainer<User> _user;

        public UserInfoResultHandler(IDataRepository dataRepository) :
            base(Server.HServer, Msg.CmdType.HU.USER_INFO_RESULT)
        {
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(UserInfoResult msg)
        {
            if (msg.result != ResultCode.OK)
            {
                return;
            }

            if (msg.user == null)
            {
                return;
            }

            _user.Write(msg.user, Time.time);
        }
    }
}