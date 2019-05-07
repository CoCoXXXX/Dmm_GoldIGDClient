using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class CheckinResultHandler : MessageHandlerAdapter<CheckinResult>
    {
        private readonly IDataContainer<CheckinConfig> _checkInConfig;

        private readonly IDataContainer<CheckinResult> _checkInResult;

        public CheckinResultHandler(IDataRepository dataRepository) : base(Server.HServer,
            Msg.CmdType.HU.CHECKIN_RESULT)
        {
            _checkInConfig = dataRepository.GetContainer<CheckinConfig>(DataKey.CheckinConfig);
            _checkInResult = dataRepository.GetContainer<CheckinResult>(DataKey.CheckinResult);
        }

        protected override void DoHandle(CheckinResult msg)
        {
            _checkInResult.Write(msg, Time.time);
            if (msg == null ||
                msg.res.code != ResultCode.OK)
                return;

            var config = _checkInConfig.Read();
            if (config != null)
            {
                config.continue_checkin_days = msg.continue_checkin_days;
            }

            UpdateCheckinItem(msg.checkin);
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