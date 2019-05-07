using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class ChooseTableResultHandler : MessageHandlerAdapter<ChooseTableResult>
    {
        private readonly IDataContainer<ChooseTableResult> _chooseTableResult;

        private readonly IDataContainer<User> _user;

        private readonly IDataContainer<TableUserData> _tableUser;

        private readonly IDataContainer<StartRound> _startRound;

        private readonly IDataContainer<com.morln.game.gd.command.RoundEnd> _raceRoundEnd;

        private readonly IDataContainer<BRoundEnd> _roundEnd;

        private readonly IDataContainer<BKickOutCounter> _kickOutCounter;

        public ChooseTableResultHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.CHOOSE_TABLE_RESULT_V6)
        {
            _chooseTableResult = dataRepository.GetContainer<ChooseTableResult>(DataKey.ChooseTableResult);

            _user = dataRepository.GetContainer<User>(DataKey.MyUser);

            _startRound = dataRepository.GetContainer<StartRound>(DataKey.StartRound);

            _raceRoundEnd =
                dataRepository.GetContainer<com.morln.game.gd.command.RoundEnd>(DataKey.RaceRoundEnd);

            _roundEnd = dataRepository.GetContainer<BRoundEnd>(DataKey.BRoundEnd);

            _kickOutCounter = dataRepository.GetContainer<BKickOutCounter>(DataKey.BKickOutCounter);

            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        protected override void DoHandle(ChooseTableResult msg)
        {
            _chooseTableResult.Write(msg, Time.time);

            if (msg != null && msg.result == ResultCode.OK)
            {
                // 选房成功的情况下，更新一下自己的数据。
                var tableUserData = _tableUser.Read();
                var user = _user.Read();
                var mySeat = tableUserData.MySeat;
                var msgUser = DataUtil.GetUser(msg.table, mySeat);
                if (DataUtil.UpdateUserPublic(msgUser, user))
                {
                    _user.Invalidate(Time.time);
                }

                // 进桌成功，清空开局和结算数据。
                _startRound.ClearAndInvalidate(0);
                _roundEnd.ClearNotInvalidate();
                _raceRoundEnd.ClearNotInvalidate();
            }
            // 收到选桌命令的话，就应该清空踢人计数器。
            _kickOutCounter.ClearAndInvalidate(Time.time);
        }
    }
}