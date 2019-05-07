using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;

namespace Dmm.MsgLogic.GU
{
    public class BUseCommodityHandler : MessageHandlerAdapter<BUseCommodity>
    {
        private readonly IDataContainer<TableUserData> _tableUser;

        private readonly IDataContainer<List<Commodity>> _commodityList;

        private readonly IDataContainer<Table> _table;

        public BUseCommodityHandler(IDataRepository dataRepository) : base(Server.GServer,
            Msg.CmdType.GU.B_USE_COMMODITY_V6)
        {
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _commodityList = dataRepository.GetContainer<List<Commodity>>(DataKey.CommodityList);
            _table = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
        }

        protected override void DoHandle(BUseCommodity msg)
        {
            var tableUser = _tableUser.Read();
            DataUtil.UseCommodity(
                tableUser.GetUserFormUserName(msg.username),
                GameUtil.GetCommodity(_commodityList.Read(), msg.cname),
                msg.use_or_not == 1);

            _table.Invalidate(Time.time);
        }
    }
}