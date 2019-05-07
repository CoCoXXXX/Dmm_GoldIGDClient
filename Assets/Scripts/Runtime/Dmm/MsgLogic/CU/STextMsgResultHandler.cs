using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.CU
{
    public class STextMsgResultHandler : MessageHandlerAdapter<STextMsgResult>
    {
        private readonly IDataContainer<BTextMsg> _container;

        private readonly IDataContainer<User> _user;

        public STextMsgResultHandler(
            IDataRepository dataRepository)
            : base(Server.GServer, Msg.CmdType.CU.TEXT_MSG_RESULT_V6)
        {
            _container = dataRepository.GetContainer<BTextMsg>(DataKey.BTextMsg);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(STextMsgResult msg)
        {
            if (msg.result.code == ResultCode.OK)
            {
                var user = _user.Read();
                var textMsg = new BTextMsg();
                textMsg.chat_channel = msg.chat_channel;
                textMsg.content = msg.content;
                textMsg.from_username = user.username;
                textMsg.from_nickname = user.nickname;
                textMsg.timestamp = msg.timestamp;

                _container.Write(textMsg, Time.time);
            }
        }
    }
}