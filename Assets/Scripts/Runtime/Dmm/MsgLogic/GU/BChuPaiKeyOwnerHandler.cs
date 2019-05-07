using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BChuPaiKeyOwnerHandler : MessageHandlerAdapter<BChuPaiKeyOwner>
    {
        private readonly IDataContainer<PlayingData> _playingData;


        public BChuPaiKeyOwnerHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.B_CHUPAI_KEY_OWNER_V6)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        protected override void DoHandle(BChuPaiKeyOwner msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }
        }
    }
}