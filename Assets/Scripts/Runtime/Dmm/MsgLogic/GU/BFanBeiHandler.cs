using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BFanBeiHandler : MessageHandlerAdapter<BFanbei>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<BFanbei> _bFanbei;

        public BFanBeiHandler(IDataRepository dataRepository) : base(Server.GServer, Msg.CmdType.GU.B_FANBEI_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);

            _bFanbei = dataRepository.GetContainer<BFanbei>(DataKey.BFanbei);
        }

        protected override void DoHandle(BFanbei msg)
        {
            _bFanbei.Write(msg, Time.time);

            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}