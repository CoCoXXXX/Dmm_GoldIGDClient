using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class RaceConfigHandler : MessageHandlerAdapter<RaceConfig>
    {
        private readonly IDataContainer<RaceConfigList> _raceConfigList;

        private readonly IDataContainer<RaceConfig> _raceConfig;

        public RaceConfigHandler(
            IDataRepository dataRepository) :
            base(Server.HServer, Msg.CmdType.HU.RACE_CONFIG)
        {
            _raceConfigList = dataRepository.GetContainer<RaceConfigList>(DataKey.RaceConfigList);
            _raceConfig = dataRepository.GetContainer<RaceConfig>(DataKey.RaceConfig);
        }

        protected override void DoHandle(RaceConfig msg)
        {
            _raceConfig.Write(msg, Time.time);
            var raceConfigList = _raceConfigList.Read();
            if (raceConfigList == null)
            {
                return;
            }

            var list = raceConfigList.config_list;
            if (list == null)
            {
                return;
            }
            if (msg == null)
            {
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                if (msg.race_id == list[i].race_id)
                {
                    list[i] = msg;
                    _raceConfigList.Invalidate(Time.time);
                    break;
                }
            }
        }
    }
}