using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BeenHuanGongHandler : MessageHandlerAdapter<BeenHuanGong>
    {
        private readonly IDataContainer<BeenHuanGong> _container;

        private readonly IDataContainer<PlayingData> _playingData;

        public BeenHuanGongHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.BEEN_HUANGONG_V6)
        {
            _container = dataRepository.GetContainer<BeenHuanGong>(DataKey.BeenHuanGong);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BeenHuanGong msg)
        {
            if (msg == null)
            {
                return;
            }

            _container.Write(msg, Time.time);

            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}