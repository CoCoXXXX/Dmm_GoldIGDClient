using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BeenJinGongHandler : MessageHandlerAdapter<BeenJinGong>
    {
        private readonly IDataContainer<BeenJinGong> _container;

        private readonly IDataContainer<PlayingData> _playingData;

        public BeenJinGongHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.BEEN_JINGONG_V6)
        {
            _container = dataRepository.GetContainer<BeenJinGong>(DataKey.BeenJinGong);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BeenJinGong msg)
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