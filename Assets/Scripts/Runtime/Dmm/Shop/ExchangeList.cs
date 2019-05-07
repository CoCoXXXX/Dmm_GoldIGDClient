using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Shop
{
    public class ExchangeList : ItemList<Exchange>
    {
        #region Inject

        private ExchangeItem.Factory _itemFactory;

        private IDataContainer<List<Exchange>> _exchangeList;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            ExchangeItem.Factory itemFactory)
        {
            _itemFactory = itemFactory;
            _exchangeList = dataRepository.GetContainer<List<Exchange>>(DataKey.ExchangeList);
        }

        #endregion

        public override int SlotCount()
        {
            var list = _exchangeList.Read();
            if (list == null)
            {
                return 0;
            }

            return list.Count;
        }

        public override Item<Exchange> CreateItem()
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
            return _exchangeList.Timestamp;
        }

        public override int DataCount()
        {
            var list = _exchangeList.Read();
            if (list == null)
            {
                return 0;
            }

            return list.Count;
        }

        public override Exchange GetData(int index)
        {
            var list = _exchangeList.Read();
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

        public override void OnItemSelected(Item<Exchange> item)
        {
            // nothing.
        }
    }
}