using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.App;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class RoundEndHandler : MessageHandlerAdapter<com.morln.game.gd.command.RoundEnd>
    {
        private readonly IDataContainer<Table> _table;

        private readonly IDataContainer<User> _user;

        private readonly IAnalyticManager _analyticManager;

        private readonly IDataContainer<com.morln.game.gd.command.RoundEnd> _raceRoundEnd;

        private readonly IAppController _appController;

        private readonly IDataContainer<HostInfoResult> _hostInfo;

        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<TableUserData> _tableUser;

        public RoundEndHandler(IDataRepository dataRepository, IAnalyticManager analyticManager,
            IAppController appController)
            : base(Server.GServer, Msg.CmdType.GU.R_ROUND_END_V6)
        {
            _analyticManager = analyticManager;
            _appController = appController;
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);

            _hostInfo = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);

            _raceRoundEnd =
                dataRepository.GetContainer<com.morln.game.gd.command.RoundEnd>(DataKey.RaceRoundEnd);

            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);

            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        protected override void DoHandle(com.morln.game.gd.command.RoundEnd msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }

            _raceRoundEnd.Write(msg, Time.time);

            var table = _table.Read();
            var myUser = _user.Read();
            var tableUser = _tableUser.Read();

            // 更新一下桌子的数据。
            if (DataUtil.UpdateTable(msg.table, table))
            {
                _table.Invalidate(Time.time);
            }

            if (msg.host_info != null)
            {
                _hostInfo.Write(msg.host_info, Time.time);
            }

            // 更新玩家自己的数据。
            var user = DataUtil.GetUser(msg.table, tableUser.MySeat);
            if (DataUtil.UpdateUserPublic(user, myUser))
            {
                _user.Invalidate(Time.time);
            }

            // 重置玩家的状态。
            ResetAllUserState();

            if (!_appController.IsSingleGameMode())
            {
                // 非单机模式下，进行统计。
                if (msg.total_multiple > 1)
                    _analyticManager.EventValue("round_end_fanbei", null, msg.total_multiple);
            }
        }

        /// <summary>
        /// 重置所有玩家的准备和暂离状态。
        /// </summary>
        public void ResetAllUserState()
        {
            var user = _user.Read();
            if (user != null)
            {
                user.ready = 2;
                user.temp_leave = 2;
            }

            var table = _table.Read();
            if (table == null)
                return;

            if (table.user1 != null)
            {
                table.user1.ready = 2;
                table.user1.temp_leave = 2;
            }

            if (table.user2 != null)
            {
                table.user2.ready = 2;
                table.user2.temp_leave = 2;
            }

            if (table.user3 != null)
            {
                table.user3.ready = 2;
                table.user3.temp_leave = 2;
            }

            if (table.user4 != null)
            {
                table.user4.ready = 2;
                table.user4.temp_leave = 2;
            }
        }
    }
}