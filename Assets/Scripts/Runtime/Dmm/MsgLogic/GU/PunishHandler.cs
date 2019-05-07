using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Game;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class PunishHandler : MessageHandlerAdapter<Punish>
    {
        private readonly IDialogManager _dialogManager;

        private readonly IAnalyticManager _analyticManager;

        private readonly IDataContainer<Room> _room;

        private readonly IDataContainer<User> _user;

        public PunishHandler(
            IDialogManager dialogManager,
            IDataRepository dataRepository,
            IAnalyticManager analyticManager)
            : base(Server.GServer, Msg.CmdType.GU.PUNISH_V6)
        {
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(Punish msg)
        {
            var room = _room.Read();
            var user = _user.Read();
            GameUtil.SetMyCurrency(user, msg.currency_type, msg.current_count);
            _user.Invalidate(Time.time);

            _dialogManager.ShowConfirmBox(
                string.Format(
                    "没打到目标就逃跑了\n蛋员外<color=#ff6600>扣你{0}{1}</color>哦",
                    msg.money,
                    CurrencyType.LabelOf(room.currency_type))
            );

            _analyticManager.Buy("leave_punish", 1, DataUtil.CalculateGeValue(msg.currency_type, msg.money));
        }
    }
}