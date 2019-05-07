using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class InteractionResultHandler : MessageHandlerAdapter<InteractionResult>
    {
        private readonly IDialogManager _dialogManager;

        private readonly IDataContainer<User> _user;

        public InteractionResultHandler(IDialogManager dialogManager, IDataRepository dataRepository)
            : base(Server.GServer, Msg.CmdType.GU.INTERACTION_RESULT_V6)
        {
            _dialogManager = dialogManager;
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(InteractionResult msg)
        {
            // 这里需要处理玩家交互的结果，主要是因为需要刷新玩家当前的金蛋数量。
            if (msg.res.code == ResultCode.OK)
            {
                var currentMoney = msg.left_currency;
                if (currentMoney != null)
                {
                    var user = _user.Read();
                    GameUtil.SetMyCurrency(user, currentMoney.type, currentMoney.count);
                    _user.Invalidate(Time.time);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(msg.res.msg))
                {
                    // 如果服务器端发了错误信息，才显示错误信息。
                    _dialogManager.ShowToast(msg.res.msg, 2, true);
                }
            }
        }
    }
}