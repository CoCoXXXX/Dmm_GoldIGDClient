using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.CU
{
    public class JianMengResultHandler : MessageHandlerAdapter<JianMengResult>
    {
        private readonly IDataContainer<BJianMeng> _container;

        private readonly IDataContainer<User> _user;

        public JianMengResultHandler(
            IDataRepository dataRepository)
            : base(Server.GServer, Msg.CmdType.CU.JIAN_MENG_RESULT_V6)
        {
            _container = dataRepository.GetContainer<BJianMeng>(DataKey.BJianMeng);

            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(JianMengResult msg)
        {
            if (msg.res.code == ResultCode.OK)
            {
                var user = _user.Read();
                var jianMeng = new BJianMeng();
                jianMeng.cmd = msg.cmd;
                jianMeng.from_username = user.username;
                jianMeng.from_nickname = user.nickname;
                jianMeng.timestamp = msg.timestamp;
                _container.Write(jianMeng, Time.time);
            }
        }
    }
}