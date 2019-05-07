using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.App;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BRoundEndHandler : MessageHandlerAdapter<BRoundEnd>
    {
        private readonly IDataContainer<Table> _table;

        private readonly IDataContainer<User> _user;

        private readonly IAnalyticManager _analyticManager;

        private readonly IDataContainer<PlayingData> _playingData;

        private readonly IDataContainer<BRoundEnd> _bRoundEnd;

        private readonly IAppController _appController;

        private readonly IDataContainer<Room> _roomData;

        private readonly IDataContainer<TableUserData> _tableUserData;

        public BRoundEndHandler(
            IDataRepository dataRepository,
            IAnalyticManager analyticManager,
            IAppController appController)
            : base(Server.GServer, Msg.CmdType.GU.B_ROUND_END_V6)
        {
            _analyticManager = analyticManager;
            _appController = appController;
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _bRoundEnd = dataRepository.GetContainer<BRoundEnd>(DataKey.BRoundEnd);
            _roomData = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        protected override void DoHandle(BRoundEnd msg)
        {
            var msgPlayingData = msg.playing_data;
            if (msgPlayingData != null)
            {
                _playingData.Write(msgPlayingData, Time.time);
            }

            _bRoundEnd.Write(msg, Time.time);

            var tableUser = _tableUserData.Read();
            var table = _table.Read();
            var myUser = _user.Read();
            var room = _roomData.Read();

            // 更新一下桌子的数据。
            if (DataUtil.UpdateTable(msg.table, table))
            {
                _table.Invalidate(Time.time);
            }

            // 更新玩家自己的数据。
            var user = DataUtil.GetUser(msg.table, tableUser.MySeat);
            if (DataUtil.UpdateUserPublic(user, myUser))
            {
                _user.Invalidate(Time.time);
            }

            // 重置玩家的状态。
            ResetAllUserState();

            if (_appController.IsSingleGameMode())
            {
                return;
            }

            // 非单机模式下，进行统计。
            if (msg.total_multiple > 1)
            {
                _analyticManager.EventValue("round_end_fanbei", null, msg.total_multiple);
            }

            var type = room == null ? CurrencyType.GOLDEN_EGG : room.currency_type;
            var count = 0;

            var userName = user == null ? null : user.username;
            if (StringUtil.AreEqual(msg.username1, userName))
            {
                count = msg.final_money1;
            }
            else if (StringUtil.AreEqual(msg.username2, userName))
            {
                count = msg.final_money2;
            }
            else if (StringUtil.AreEqual(msg.username3, userName))
            {
                count = msg.final_money3;
            }
            else if (StringUtil.AreEqual(msg.username4, userName))
            {
                count = msg.final_money4;
            }

            var currentRoomTaxRate = room == null ? 0 : room.tax_rate;
            var currentRoomId = room == null ? -1 : room.room_id;
            if (count > 0)
            {
                _analyticManager.Bonus(
                    DataUtil.CalculateGeValue(type, count * (100 - currentRoomTaxRate) / 100),
                    AwardType.RoundEndWin);
            }
            else if (count < 0)
            {
                var geVal = (int) DataUtil.CalculateGeValue(type, Math.Abs(count));
                _analyticManager.Buy("round_end_lose", 1, geVal);

                // 统计结算带来的负金蛋流量。
                var attrs = new Dictionary<string, string>();
                attrs.Add("base_money", "" + currentRoomTaxRate);
                _analyticManager.EventValue("round_end_ge" + currentRoomId, attrs,
                    geVal * currentRoomTaxRate / 100);
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
            {
                return;
            }

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