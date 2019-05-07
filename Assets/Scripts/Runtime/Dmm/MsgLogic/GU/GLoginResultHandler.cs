using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class GLoginResultHandler : MessageHandlerAdapter<GLoginResult>
    {
        private readonly IAnalyticManager _analyticManager;

        private readonly IDataContainer<Room> _room;

        private readonly IDataContainer<GLoginResult> _gloginResult;

        private readonly IDataContainer<User> _user;

        private readonly IDataContainer<int> _currentGameMode;

        public GLoginResultHandler(IAnalyticManager analyticManager, IDataRepository dataRepository)
            : base(Server.GServer, Msg.CmdType.GU.LOGIN_RESULT_V6)
        {
            _room = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _gloginResult = dataRepository.GetContainer<GLoginResult>(DataKey.GLoginResult);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _currentGameMode = dataRepository.GetContainer<int>(DataKey.CurrentGameMode);
            _analyticManager = analyticManager;
        }

        protected override void DoHandle(GLoginResult msg)
        {
            _gloginResult.Write(msg, Time.time);

            if (msg.room != null)
                _currentGameMode.Write(msg.room.game_mode, Time.time);

            // 统计进房相关的数据。
            var room = _room.Read();
            if (room == null)
            {
                return;
            }
            var eventId = string.Format("choose_room_ok_{0}", room.room_id);
            var attrs = new Dictionary<string, string>();
            attrs.Add("base_money", "" + room.base_money);
            attrs.Add("currency_type", CurrencyType.IdOf(room.currency_type));
            _analyticManager.EventValue(eventId, attrs,
                (int) DataUtil.GetCurrency(_user.Read(), room.currency_type));
        }
    }
}