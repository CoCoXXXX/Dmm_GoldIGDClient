using com.morln.game.gd.command;
using Dmm.Msg;
using Dmm.Network;

namespace Dmm.MsgLogic.CU
{
    public class SNewFriendHanlder : MessageHandlerAdapter<SNewFriend>
    {
        private readonly RemoteAPI _remoteApi;

        public SNewFriendHanlder(RemoteAPI remoteApi)
            : base(Server.GServer, Msg.CmdType.CU.NEW_FRIEND_V6)
        {
            _remoteApi = remoteApi;
        }

        protected override void DoHandle(SNewFriend msg)
        {
            _remoteApi.RefreshFriendList();
        }
    }
}