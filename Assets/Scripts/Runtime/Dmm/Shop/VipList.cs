using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Shop
{
    public class VipList : ItemList<VipExchange>
    {
        #region Inject

        private IDataContainer<VipExchangeListResult> _vipExchangeListResult;

        private VipItem.Factory _itemFactory;

        [Inject]
        public void Initialize(IDataRepository dataRepository, VipItem.Factory itemFactory)
        {
            _itemFactory = itemFactory;
            _vipExchangeListResult =
                dataRepository.GetContainer<VipExchangeListResult>(DataKey.VipExchangeListResult);
        }

        #endregion

        public override int SlotCount()
        {
            return VipExchangeCount();
        }

        public override Item<VipExchange> CreateItem()
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
            return _vipExchangeListResult.Timestamp;
        }

        public override int DataCount()
        {
            return VipExchangeCount();
        }

        public override VipExchange GetData(int index)
        {
            return GetVipExchange(index);
        }

        public override void OnItemSelected(Item<VipExchange> item)
        {
        }

        public List<VipExchange> VipExchangeList()
        {
            var data = _vipExchangeListResult.Read();
            if (data == null)
                return null;

            return data.exchange;
        }

        public int VipExchangeCount()
        {
            var list = VipExchangeList();
            if (list == null)
                return 0;

            return list.Count;
        }

        public VipExchange GetVipExchange(int index)
        {
            var list = VipExchangeList();
            if (list == null)
                return null;

            if (index < 0 || index >= list.Count)
                return null;

            return list[index];
        }
    }
}