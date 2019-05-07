using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class ReCheckinResultHandler : MessageHandlerAdapter<ReCheckinResult>
    {
        private readonly IDataContainer<CheckinConfig> _checkInConfig;

        private readonly IDataContainer<ReCheckinResult> _reCheckInResult;

        private readonly IDataContainer<User> _user;

        public ReCheckinResultHandler(IDataRepository dataRepository) : base(Server.HServer,
            Msg.CmdType.HU.RECHECKIN_RESULT)
        {
            _checkInConfig = dataRepository.GetContainer<CheckinConfig>(DataKey.CheckinConfig);
            _reCheckInResult = dataRepository.GetContainer<ReCheckinResult>(DataKey.ReCheckinResult);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(ReCheckinResult msg)
        {
            _reCheckInResult.Write(msg, Time.time);

            if (msg == null ||
                msg.res.code != ResultCode.OK)
                return;

            var invalidate = false;
            var config = _checkInConfig.Read();
            if (config != null)
            {
                config.need_recheckin_days = msg.need_recheckin_days;
                config.continue_checkin_days = msg.continue_checkin_days;
                invalidate = true;
            }

            var user = _user.Read();
            GameUtil.SetMyCurrency(user, CurrencyType.RECHECKIN_CARD, msg.recheckin_card_count);
            _user.Invalidate(Time.time);

            var items = msg.checkin;
            if (items != null && items.Count > 0)
            {
                foreach (var it in items)
                {
                    UpdateCheckinItem(it, false);
                }

                invalidate = true;
            }

            if (invalidate)
            {
                _checkInConfig.Invalidate(Time.time);
            }
        }

        private void UpdateCheckinItem(CheckinItem checkinItem, bool invalidate = true)
        {
            if (checkinItem == null)
            {
                return;
            }

            var config = _checkInConfig.Read();
            if (config == null)
            {
                return;
            }

            var found = false;
            var items = config.checkin_item;
            for (int i = 0; i < items.Count; i++)
            {
                var c = items[i];
                if (c.day == checkinItem.day)
                {
                    c.timestamp = checkinItem.timestamp;
                    c.status = checkinItem.status;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                items.Add(checkinItem);
            }

            if (invalidate)
            {
                _checkInConfig.Invalidate(Time.time);
            }
        }
    }
}