using com.morln.game.gd.command;
using Dmm.Msg;
using Dmm.Network;

namespace Dmm.MsgLogic.CU
{
    public class SFriendRemovedToReceiverHandler : MessageHandlerAdapter<SFriendRemovedToReceiver>
    {
        private readonly RemoteAPI _remoteApi;

        public SFriendRemovedToReceiverHandler(
            RemoteAPI remoteApi)
            : base(Server.GServer, Msg.CmdType.CU.FRIEND_REMOVED_TO_RECEIVER_V6)
        {
            _remoteApi = remoteApi;
        }

        protected override void DoHandle(SFriendRemovedToReceiver msg)
        {
            _remoteApi.RefreshFriendList();
        }
    }
}