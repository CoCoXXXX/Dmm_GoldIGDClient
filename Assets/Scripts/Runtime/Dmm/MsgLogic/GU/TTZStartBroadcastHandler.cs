using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class TTZStartBroadcastHandler : MessageHandlerAdapter<TTZStartBroadcast>
    {
        private readonly IDataContainer<TTZStartBroadcast> _ttzStartBroadcast;

        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<Table> _table;


        public TTZStartBroadcastHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.TTZ_START_ROUND_V6)
        {
            _ttzStartBroadcast = dataRepository.GetContainer<TTZStartBroadcast>(DataKey.TTZStartBroadcast);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
        }

        protected override void DoHandle(TTZStartBroadcast msg)
        {
            _ttzStartBroadcast.Write(msg, Time.time);

            var table = _table.Read();
            if (table == null)
            {
                return;
            }

            var user1 = msg.user1;
            if (user1 != null)
            {
                table.user1 = user1;
            }

            var user2 = msg.user2;
            if (user2 != null)
            {
                table.user2 = user2;
            }

            var user3 = msg.user3;
            if (user3 != null)
            {
                table.user3 = user3;
            }

            var user4 = msg.user4;
            if (user4 != null)
            {
                table.user4 = user4;
            }

            _table.Invalidate(Time.time);

            var playingData = msg.playing_data;
            if (playingData != null)
            {
                _playingData.Write(playingData, Time.time);
            }
        }
    }
}