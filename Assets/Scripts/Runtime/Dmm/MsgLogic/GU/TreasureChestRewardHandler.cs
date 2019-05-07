using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Msg;

namespace Dmm.MsgLogic.GU
{
    public class TreasureChestRewardHandler : MessageHandlerAdapter<TreasureChestReward>
    {
        private readonly IDialogManager _dialogManager;

        public TreasureChestRewardHandler(IDialogManager dialogManager)
            : base(Server.GServer, Msg.CmdType.GU.TREASURE_CHEST_REWARD_V6)
        {
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(TreasureChestReward msg)
        {
            _dialogManager.ShowDialog<GetAwardDialog>(DialogName.GetAwardDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(
                        "开启神秘宝箱",
                        AwardType.TreasureChest,
                        msg.gift != null ? msg.gift.curreny : null);
                    dialog.Show();
                }
            );
        }
    }
}