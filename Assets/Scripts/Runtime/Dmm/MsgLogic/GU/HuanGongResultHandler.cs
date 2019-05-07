using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class HuanGongResultHandler : MessageHandlerAdapter<HuanGongResult>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public HuanGongResultHandler(IDataRepository dataRepository) :
            base(Server.GServer, Msg.CmdType.GU.HUANGONG_RESULT_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(HuanGongResult msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}