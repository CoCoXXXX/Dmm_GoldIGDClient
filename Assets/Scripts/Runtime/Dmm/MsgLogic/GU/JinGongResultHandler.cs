using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class JinGongResultHandler : MessageHandlerAdapter<JinGongResult>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public JinGongResultHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.JINGONG_RESULT_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(JinGongResult msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}