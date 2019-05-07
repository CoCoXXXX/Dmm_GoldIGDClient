using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Shop
{
    public class YuanBaoList : ItemList<com.morln.game.gd.command.YuanBaoItem>
    {
        #region [Inject]

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private IAnalyticManager _analyticManager;

        private YuanBaoItem.Factory _itemFactory;

        private IDataContainer<YuanBaoConfigResult> _yuanBaoConfigResult;

        private IDataContainer<User> _myUser;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            IAnalyticManager analyticManager,
            YuanBaoItem.Factory itemFactory)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _analyticManager = analyticManager;
            _itemFactory = itemFactory;
            _yuanBaoConfigResult =
                dataRepository.GetContainer<YuanBaoConfigResult>(DataKey.YuanBaoConfigResult);
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        #endregion

        public override void BeforeRefresh()
        {
            SelectEmpty();
        }

        public override int SlotCount()
        {
            return YuanBaoItemCount();
        }

        public override Item<com.morln.game.gd.command.YuanBaoItem> CreateItem()
        {
            return _itemFactory.Create();
        }

        public override bool HasDivider()
        {
            return false;
        }

        public override GameObject CreateDivider()
        {
            return null;
        }

        public override float DataUpdateTime()
        {
            return _yuanBaoConfigResult.Timestamp;
        }

        public override int DataCount()
        {
            return YuanBaoItemCount();
        }

        public override com.morln.game.gd.command.YuanBaoItem GetData(int index)
        {
            return GetYuanBaoItem(index);
        }

        public override void OnItemSelected(Item<com.morln.game.gd.command.YuanBaoItem> item)
        {
            var data = item.GetData();
            if (data != null)
            {
                var content = string.Format(
                    "尊敬的玩家：\n您是否要花费 <color=red>{0}兑奖券</color> \n兑换 <color=red>{1}</color> ？",
                    data.price, data.displayName);
                _dialogManager.ShowConfirmBox(
                    content,
                    true, "确认兑换", () => _remoteAPI.RequestExchangeYuanBao(data.name),
                    false, null, null,
                    true, false, true);

                var attrs = new Dictionary<string, string>();
                var myUser = _myUser.Read();
                attrs.Add("item", data.name);
                _analyticManager.EventValue("yuanbao_click_item", attrs,
                    (int) myUser.MyCurrency(CurrencyType.YUAN_BAO));
            }
            else
            {
                _dialogManager.ShowToast("数据错误，请重新登陆之后再尝试兑换", 2, true);
            }
        }

        public List<com.morln.game.gd.command.YuanBaoItem> YuanBaoItemList()
        {
            var data = _yuanBaoConfigResult.Read();
            if (data == null)
            {
                return null;
            }

            if (data.res == null)
            {
                return null;
            }

            if (data.res.code != ResultCode.OK)
            {
                return null;
            }

            return data.item;
        }

        public int YuanBaoItemCount()
        {
            var list = YuanBaoItemList();
            if (list == null)
            {
                return 0;
            }

            return list.Count;
        }

        public com.morln.game.gd.command.YuanBaoItem GetYuanBaoItem(int index)
        {
            var list = YuanBaoItemList();
            if (list == null)
            {
                return null;
            }

            if (index < 0 || index >= list.Count)
            {
                return null;
            }

            return list[index];
        }
    }
}