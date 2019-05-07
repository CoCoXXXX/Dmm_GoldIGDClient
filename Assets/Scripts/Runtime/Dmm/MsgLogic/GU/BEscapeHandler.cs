using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Common;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BEscapeHandler : MessageHandlerAdapter<BEscape>
    {
        private readonly IDialogManager _dialogManager;

        private readonly IAnalyticManager _analyticManager;

        private readonly IDataContainer<User> _user;

        private readonly IDataContainer<Room> _room;

        public BEscapeHandler(IDialogManager dialogManager, IDataRepository dataRepository,
            IAnalyticManager analyticManager)
            : base(Server.GServer, Msg.CmdType.GU.B_ESCAPE_V6)
        {
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
        }

        protected override void DoHandle(BEscape msg)
        {
            var user = _user.Read();
            GameUtil.SetMyCurrency(user, msg.currency_type, msg.current_count);
            _user.Invalidate(Time.time);

            var escapeUser = msg.player != null ? msg.player.nickname : "玩家";
            _dialogManager.ShowConfirmBox(
                string.Format(
                    "<color=green>{0}</color>没打到{1}就逃走了\n<color=#ff6600>补偿您{2}{3}</color>",
                    escapeUser,
                    Dmm.PokerLogic.PokerNumType.LabelOf(CurrentRoomTargetHost()),
                    msg.compensation,
                    CurrencyType.LabelOf(msg.currency_type)
                )
            );

            _analyticManager.Bonus(DataUtil.CalculateGeValue(msg.currency_type, msg.compensation),
                AwardType.Compensation);
        }

        private int CurrentRoomTargetHost()
        {
            var room = _room.Read();
            if (room == null)
                return PokerLogic.PokerNumType.P2;

            return PokerLogicUtil.PokerNumTypeOf(room.target_host);
        }
    }
}