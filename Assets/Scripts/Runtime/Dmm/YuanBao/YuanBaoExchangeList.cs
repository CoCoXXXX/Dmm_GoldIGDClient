using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.YuanBao
{
    public class YuanBaoExchangeList : ItemList<YuanBaoExchange>
    {
        #region Inject

        private YuanBaoExchangeItem.Factory _itemFactory;

        private IDataContainer<MyYuanBaoExchangeResult> _myYuanBaoExchangeResult;

        [Inject]
        public void Initialize(IDataRepository dataRepository, YuanBaoExchangeItem.Factory itemFactory)
        {
            _itemFactory = itemFactory;
            _myYuanBaoExchangeResult =
                dataRepository.GetContainer<MyYuanBaoExchangeResult>(DataKey.MyYuanBaoExchangeResult);
        }

        #endregion

        public override int SlotCount()
        {
            var data = _myYuanBaoExchangeResult.Read();
            if (data == null)
                return 0;

            var list = data.exchange;
            if (list == null)
                return 0;

            return list.Count;
        }

        public override Item<YuanBaoExchange> CreateItem()
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
            return _myYuanBaoExchangeResult.Timestamp;
        }

        public override int DataCount()
        {
            var data = _myYuanBaoExchangeResult.Read();
            if (data == null)
                return 0;

            var list = data.exchange;
            if (list == null)
                return 0;

            return list.Count;
        }

        public override YuanBaoExchange GetData(int index)
        {
            var data = _myYuanBaoExchangeResult.Read();
            if (data == null)
                return null;

            var list = data.exchange;
            if (list == null)
                return null;

            if (index < 0 || index >= list.Count)
                return null;

            return list[index];
        }

        public override void OnItemSelected(Item<YuanBaoExchange> item)
        {
        }
    }
}