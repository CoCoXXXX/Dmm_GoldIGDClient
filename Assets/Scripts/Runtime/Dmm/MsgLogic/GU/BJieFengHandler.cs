using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BJieFengHandler : MessageHandlerAdapter<BJieFeng>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public BJieFengHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.B_JIEFENG_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BJieFeng msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msg.playing_data != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}