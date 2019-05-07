using com.morln.game.gd.command;
using Dmm.Checkin;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class CheckinConfigResultHandler : MessageHandlerAdapter<CheckinConfigResult>
    {
        private readonly CheckinConditionComparator _checkinConditionComparator = new CheckinConditionComparator();

        private readonly IDataContainer<CheckinConfigResult> _checkinConfigResult;

        public CheckinConfigResultHandler(IDataRepository dataRepository) :
            base(Server.HServer, Msg.CmdType.HU.CHECKIN_CONFIG)
        {
            _checkinConfigResult =
                dataRepository.GetContainer<CheckinConfigResult>(DataKey.CheckinConfigResult);
        }

        protected override void DoHandle(CheckinConfigResult msg)
        {
            _checkinConfigResult.Write(msg, Time.time);
            if (msg == null || msg.res.code != ResultCode.OK)
            {
                return;
            }

            var config = msg.checkin_config;
            if (config == null)
            {
                return;
            }

            var conditionList = config.condition;
            if (conditionList != null)
            {
                conditionList.Sort(_checkinConditionComparator);
            }
        }
    }
}