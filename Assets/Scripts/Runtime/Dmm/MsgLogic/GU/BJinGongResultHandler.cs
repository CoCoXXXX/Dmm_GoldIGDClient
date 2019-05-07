using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BJinGongResultHandler : MessageHandlerAdapter<BJinGongResult>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public BJinGongResultHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.B_JINGONG_RESULT_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BJinGongResult msg)
        {
            var msgPLayingData = msg.playing_data;
            if (msgPLayingData != null)
            {
                _playingData.Write(msgPLayingData, Time.time);
            }
        }
    }
}