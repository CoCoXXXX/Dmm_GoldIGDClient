using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class ExchangeYuanBaoResultHandler : MessageHandlerAdapter<ExchangeYuanBaoResult>
    {
        private readonly IDataContainer<ExchangeYuanBaoResult> _exchangeYuanBaoResult;

        private readonly IDataContainer<YuanBaoConfigResult> _yuanBaoConfigResult;

        private readonly IDataContainer<User> _userContainer;

        public ExchangeYuanBaoResultHandler(IDataRepository dataRepository) :
            base(Server.HServer, Msg.CmdType.HU.EXCHANGE_YUANBAO_RESULT)
        {
            _exchangeYuanBaoResult = dataRepository.GetContainer<ExchangeYuanBaoResult>(DataKey.ExchangeYuanBaoResult);
            _yuanBaoConfigResult =
                dataRepository.GetContainer<YuanBaoConfigResult>(DataKey.YuanBaoConfigResult);

            _userContainer = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        protected override void DoHandle(ExchangeYuanBaoResult msg)
        {
            _exchangeYuanBaoResult.Write(msg, Time.time);

            if (msg != null)
            {
                if (msg.yuan_bao_item != null)
                {
                    UpdateYuanBaoItem(msg.yuan_bao_item);
                }

                if (msg.res.code == ResultCode.OK)
                {
                    var user = _userContainer.Read();
                    GameUtil.SetMyCurrency(user, CurrencyType.YUAN_BAO, msg.my_left_yuan_bao);
                    _userContainer.Invalidate(Time.time);
                }
            }
        }

        public void UpdateYuanBaoItem(YuanBaoItem item)
        {
            if (item == null)
                return;

            var yuanBaoConfigResult = _yuanBaoConfigResult.Read();

            if (yuanBaoConfigResult == null)
            {
                return;
            }

            if (yuanBaoConfigResult.res == null)
            {
                return;
            }

            if (yuanBaoConfigResult.res.code != ResultCode.OK)
            {
                return;
            }

            var list = yuanBaoConfigResult.item;

            if (list == null || list.Count <= 0)
                return;

            var found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name == item.name)
                {
                    list[i] = item;
                    found = true;
                }
            }

            if (found)
            {
                _yuanBaoConfigResult.Invalidate(Time.time);
            }
        }
    }
}