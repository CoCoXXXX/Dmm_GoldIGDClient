using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BJinGongPokerHandler : MessageHandlerAdapter<BJinGongPoker>
    {
        private readonly IDataContainer<PlayingData> _playingDta;

        public BJinGongPokerHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.B_JINGONG_POKER_V6)
        {
            _playingDta = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BJinGongPoker msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingDta.Write(msgPlayingData, Time.time);
            }
        }
    }
}