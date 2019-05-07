using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class HostInfoResultHandler : MessageHandlerAdapter<HostInfoResult>
    {
        private readonly IDataContainer<HostInfoResult> _hostInfoResult;

        private readonly IDataContainer<PlayingData> _playingData;

        public HostInfoResultHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.HOST_INFO_RESULT_V6)
        {
            _hostInfoResult = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);

            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(HostInfoResult msg)
        {
            _hostInfoResult.Write(msg, Time.time);
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}