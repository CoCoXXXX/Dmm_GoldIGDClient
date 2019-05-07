using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class ChuPaiResultHandler : MessageHandlerAdapter<ChuPaiResult>
    {
        private readonly IDataContainer<PlayingData> _playingData;

        public ChuPaiResultHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.CHUPAI_RESULT_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(ChuPaiResult msg)
        {
            if (msg.result == ResultCode.OK)
            {
                // 出牌成功的时候，应该什么都不做。
                return;
            }

            // 出牌不成功的时候刷新playingData。
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}