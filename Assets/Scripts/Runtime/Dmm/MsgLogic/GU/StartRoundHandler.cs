using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class StartRoundHandler : MessageHandlerAdapter<StartRound>
    {
        private readonly IDataContainer<Table> _currentTable;

        private readonly IDataContainer<User> _myUser;

        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<TTZStartBroadcast> _ttzStartBroadcast;

        private readonly IDataContainer<BRoundEnd> _bRoundEnd;

        private readonly IDataContainer<com.morln.game.gd.command.RoundEnd> _raceRoundEnd;

        private readonly IDataContainer<StartRound> _startRound;

        private readonly IDataContainer<BCounter> _bCounter;

        private readonly IDataContainer<BKickOutCounter> _kickOutCounter;

        private readonly IDataContainer<TableUserData> _tableUser;

        public StartRoundHandler(IDataRepository dataRepository)
            : base(Server.GServer, Msg.CmdType.GU.START_ROUND_V6)
        {
            _currentTable = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _ttzStartBroadcast = dataRepository.GetContainer<TTZStartBroadcast>(DataKey.TTZStartBroadcast);
            _bRoundEnd = dataRepository.GetContainer<BRoundEnd>(DataKey.BRoundEnd);
            _raceRoundEnd =
                dataRepository.GetContainer<com.morln.game.gd.command.RoundEnd>(DataKey.RaceRoundEnd);
            _startRound = dataRepository.GetContainer<StartRound>(DataKey.StartRound);
            _bCounter = dataRepository.GetContainer<BCounter>(DataKey.BCounter);
            _kickOutCounter = dataRepository.GetContainer<BKickOutCounter>(DataKey.BKickOutCounter);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        protected override void DoHandle(StartRound msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }

            var table = _currentTable.Read();
            if (table != null)
            {
                // 开局刷新一下局数和HostInfo。
                table.team1_host = msg.team1_host;
                table.team2_host = msg.team2_host;
                table.host_team = msg.host_team;

                table.round_count = msg.round_count;

                // 刷新桌面用户的数据。这里已经更新了MyUser
                DataUtil.UpdateUserPublic(msg.user1, table.user1);
                DataUtil.UpdateUserPublic(msg.user2, table.user2);
                DataUtil.UpdateUserPublic(msg.user3, table.user3);
                DataUtil.UpdateUserPublic(msg.user4, table.user4);

                _myUser.Invalidate(Time.time);

                // 更新桌子数据。
                // 可以刷新游戏的状态。
                _currentTable.Invalidate(Time.time);

                // 重置打牌数据。
                var playingData = _playingData.Read();
                if (playingData == null)
                {
                    playingData = new PlayingData();
                    playingData.ResetAll();
                    playingData.period = TablePeriod.StartRound;
                    _playingData.Write(playingData, Time.time);
                }

                //清空团团转数据
                _ttzStartBroadcast.ClearAndInvalidate(0);

                // 开局的时候清空结算数据。
                _bRoundEnd.ClearNotInvalidate();
                _raceRoundEnd.ClearNotInvalidate();

                // 设置开局时间。
                _startRound.Write(msg, Time.time);

                //清空倒计时时间
                _bCounter.ClearAndInvalidate(0);
                _kickOutCounter.ClearAndInvalidate(0);
            }
        }
    }
}