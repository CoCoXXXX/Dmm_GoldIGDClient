using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BChuPaiHandler : MessageHandlerAdapter<BChuPai>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public BChuPaiHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.B_CHUPAI_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BChuPai msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}