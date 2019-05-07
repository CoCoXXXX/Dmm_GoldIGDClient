using com.morln.game.gd.command;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.CU
{
    public class SAddFriendFailHandler : MessageHandlerAdapter<SAddFriendFail>
    {
        private readonly IDialogManager _dialogManager;

        public SAddFriendFailHandler(
            IDialogManager dialogManager)
            : base(Server.GServer, Msg.CmdType.CU.ADD_FRIEND_FAIL_V6)
        {
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(SAddFriendFail msg)
        {
            if (msg.result.code != ResultCode.OK)
            {
                // 出错的情况下，提示玩家出错了。
                if (!string.IsNullOrEmpty(msg.result.msg))
                {
                    _dialogManager.ShowMessageBox(msg.result.msg);
                }
            }
        }
    }
}