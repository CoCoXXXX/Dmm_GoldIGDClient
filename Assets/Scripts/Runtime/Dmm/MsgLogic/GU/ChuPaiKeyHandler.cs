using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class ChuPaiKeyHandler : MessageHandlerAdapter<ChuPaiKey>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<ChuPaiKey> _chuPaiKey;

        public ChuPaiKeyHandler(IDataRepository dataRepository) :
            base(Server.GServer, Msg.CmdType.GU.CHUPAI_KEY_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _chuPaiKey = dataRepository.GetContainer<ChuPaiKey>(DataKey.ChuPaiKey);
        }

        protected override void DoHandle(ChuPaiKey msg)
        {
            _chuPaiKey.Write(msg, Time.time);

            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}