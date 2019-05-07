using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BKangGongHandler : MessageHandlerAdapter<BKangGong>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<BKangGong> _bKangGong;

        public BKangGongHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.B_KANGGONG_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _bKangGong = dataRepository.GetContainer<BKangGong>(DataKey.BKangGong);
        }

        protected override void DoHandle(BKangGong msg)
        {
            _bKangGong.Write(msg, Time.time);
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}