using com.morln.game.gd.command;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.CU
{
    public class SAddFriendResponseToSenderHandler : MessageHandlerAdapter<SAddFriendResponseToSender>
    {
        private readonly IDialogManager _dialogManager;

        public SAddFriendResponseToSenderHandler(IDialogManager dialogManager)
            : base(Server.GServer, Msg.CmdType.CU.ADD_FRIEND_RESPONSE_TO_SENDER_V6)
        {
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(SAddFriendResponseToSender msg)
        {
            if (msg.accept)
            {
                _dialogManager.ShowConfirmBox(string.Format("恭喜您成功添加 {0} 为好友！", msg.receiver_nickname));
            }
            else
            {
                _dialogManager.ShowConfirmBox(string.Format("很遗憾，{0} 拒绝了您的好友请求", msg.receiver_nickname));
            }
        }
    }
}